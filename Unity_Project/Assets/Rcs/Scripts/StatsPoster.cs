using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatsPoster : MonoBehaviour
{
    [Header("References")]
    public TextAsset DataJSON;

    [Header("Elements")]
    public Transform FlagIconsParent;

    [Header("Values")]
    public int NbrLines = 20;
    public int NbrColumns = 10;
    public float WidthFlag = 63.0f;
    public float HeightFlag = 43.0f;
    public float FlagSizeRatio = 1.0f;

    [Header("Prefab")]
    public GameObject FlagIconPrefab;
    public GameObject WorldFlagPrefab;

    public Action<FlagIcon> OnFlagMouseOver;

    private List<FlagIcon> _flagsIcon = new List<FlagIcon>();
    private JSONObject _jsonObject;

    private float _ratioPerFlag = 0.0f;

    public const float DECIMAL_MARGIN = 0.1f;

    public void Awake()
    {
        _ratioPerFlag = 100.0f / (NbrLines * NbrColumns);
        // Only 2 decimals
        _ratioPerFlag = Mathf.Round(_ratioPerFlag * 100.0f) / 100.0f;

        _jsonObject = new JSONObject(DataJSON.text);
    }

    public void DrawCropStats(string cropText)
    {
        List<JSONObject> dataCropTomatoes = GetDataCrop(cropText);

        FillFlags(dataCropTomatoes);
    }

    public void InstantiateFlag(string country, int currCol, int currLine, float percentage, float fillAmount = 1.0f, int fillOrigin = 0)
    {
        GameObject flagIconGo = Instantiate(FlagIconPrefab, FlagIconsParent);
        FlagIcon flagIconBehaviour = flagIconGo.GetComponent<FlagIcon>();

        flagIconBehaviour.SetInformations(country, percentage);
        flagIconBehaviour.SetRatioSize(FlagSizeRatio);

        float posX = currCol * WidthFlag;
        float posY = -currLine * HeightFlag;
        flagIconGo.transform.localPosition = new Vector2(posX, posY);

        if (fillAmount < 1.0f)
        {
            flagIconBehaviour.SetFillAmount(fillAmount, fillOrigin);
        }

        flagIconGo.name = string.Format("{0} : {1} [Index = {2}]", country, fillAmount, currLine * NbrColumns + currCol);

        _flagsIcon.Add(flagIconBehaviour);

        flagIconBehaviour.OnMouseOver += OnFlagMouseOver;
    }

    //public void InstantiateWorldFlag(int currCol, int currLine, float totalPercentage)
    //{
    //    GameObject worldFlagIconGo = Instantiate(WorldFlagPrefab, FlagIconsParent);
    //    WorldFlagIcon worldFlagIconBehaviour = worldFlagIconGo.GetComponent<WorldFlagIcon>();

    //    float posX = currCol * (SizeFlag);
    //    float posY = -currLine * SizeFlag;

    //    worldFlagIconGo.transform.localPosition = new Vector2(posX, posY);
    //    worldFlagIconBehaviour.SetSize(NbrColumns - currCol);

    //    worldFlagIconGo.name = string.Format("World: {0}", 100.0f - totalPercentage);
    //}

    public void FillFlags(List<JSONObject> listStats)
    {
        int currLine = 0, currCol = 0;
        float currFlagFill = 0.0f;
        int currIndex = 0;
        Dictionary<string, float> nbrFlagsByCountries = new Dictionary<string, float>();
        float flagsRemaining = 100.0f;

        ClearAllFlags();

        foreach (JSONObject statJSON in listStats)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            if (percentage > 0.1f)
            {
                nbrFlagsByCountries[country] = (NbrColumns * NbrLines) * (percentage / 100.0f);
                flagsRemaining -= nbrFlagsByCountries[country];
            }
        }

        foreach (KeyValuePair<string, float> nbrFlagByCountry in nbrFlagsByCountries)
        {
            float nbrFlags = nbrFlagByCountry.Value;

            // That means that the previous filling was not complete
            if (currFlagFill > 0.0f && currFlagFill < 1.0f)
            {
                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value, 1 - currFlagFill, 1);
                currIndex++;

                nbrFlags -= (1 - currFlagFill);
            }

            for (int i = 0; i < Mathf.Floor(nbrFlags); i++)
            {
                currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value);

                currIndex++;
                currFlagFill = 1.0f;
            }

            // If there is a decimal part left, we should add this decimal part to the next flag icon
            if (nbrFlags - Math.Truncate(nbrFlags) > DECIMAL_MARGIN)
            {
                currFlagFill = nbrFlags - (float)Math.Truncate(nbrFlags);

                currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value, currFlagFill);
            }
        }

        if (flagsRemaining > DECIMAL_MARGIN)
        {
            Debug.Log("There is still " + flagsRemaining + " flags remaining");
        }
    }

    private void ClearAllFlags()
    {
        if (_flagsIcon.Count > 0)
        {
            foreach (FlagIcon flagIcon in _flagsIcon)
                Destroy(flagIcon.gameObject);

            _flagsIcon.Clear();
        }
    }


    public List<JSONObject> GetDataCrop(string crop)
    {
        List<JSONObject> dataCrop = null;
        List<JSONObject> _cropsJSON = _jsonObject.list;

        foreach (JSONObject jsonObject in _cropsJSON)
        {
            if (jsonObject.HasField(crop))
            {
                dataCrop = jsonObject.GetField(crop).list;

                break;
            }
        }

        if (dataCrop == null)
            Debug.LogErrorFormat("Error : crop {0} does not exist", crop);

        return dataCrop;
    }
}
