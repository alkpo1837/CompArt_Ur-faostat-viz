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
    public int NbrColumns = 10;
    public int SizeFlag = 64;

    [Header("Prefab")]
    public GameObject FlagIconPrefab;
    public GameObject WorldFlagPrefab;

    public Action<FlagIcon> OnFlagMouseOver;

    private List<FlagIcon> _flagsIcon = new List<FlagIcon>();
    private JSONObject _jsonObject;

    public void Awake()
    {
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

        float posX = currCol * (SizeFlag - 1.5f);
        float posY = -currLine * (42.1f);
        flagIconGo.transform.localPosition = new Vector2(posX, posY);

        if (fillAmount < 1.0f)
        {
            flagIconBehaviour.SetFillAmount(fillAmount, fillOrigin);
        }

        flagIconGo.name = string.Format("{0} : {1}", country, fillAmount);

        _flagsIcon.Add(flagIconBehaviour);

        flagIconBehaviour.OnMouseOver += OnFlagMouseOver;
    }

    public void InstantiateWorldFlag(int currCol, int currLine, float totalPercentage)
    {
        GameObject worldFlagIconGo = Instantiate(WorldFlagPrefab, FlagIconsParent);
        WorldFlagIcon worldFlagIconBehaviour = worldFlagIconGo.GetComponent<WorldFlagIcon>();

        float posX = currCol * (SizeFlag - 1.5f);
        float posY = -currLine * (42.1f);

        worldFlagIconGo.transform.localPosition = new Vector2(posX, posY);
        worldFlagIconBehaviour.SetSize(NbrColumns - currCol);
        // Set Size

        worldFlagIconGo.name = string.Format("World: {0}", 100.0f - totalPercentage);
    }

    public void FillFlags(List<JSONObject> listStats)
    {
        int currLine = 0, currCol = 0;
        float totalPercentage = 0.0f;
        float currPercentage = 0.0f;

        if (_flagsIcon.Count > 0)
        {
            foreach (FlagIcon flagIcon in _flagsIcon)
                Destroy(flagIcon.gameObject);
            
            _flagsIcon.Clear();
        }

        foreach (JSONObject statJSON in listStats)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            //Debug.LogFormat("We got {0} during {1}", country, percentage);

            if (percentage > 1.0f)
            {
                if (totalPercentage % 1 != 0)
                {
                    InstantiateFlag(country, currCol, currLine, percentage, Mathf.Ceil(totalPercentage) - totalPercentage, 1);

                    currPercentage = Mathf.Ceil(totalPercentage) - totalPercentage;
                    totalPercentage = Mathf.Ceil(totalPercentage);

                    //Debug.LogFormat("Instantiate India : {0} = {1} - {2}", currPercentage, Mathf.Ceil(totalPercentage), totalPercentage);

                    currCol = Mathf.FloorToInt(totalPercentage) % NbrColumns;
                    currLine = (int)Mathf.Floor(Mathf.FloorToInt(totalPercentage) / NbrColumns);
                }
                else
                {
                    currPercentage = 0.0f;
                }

                for (; currPercentage < percentage; currPercentage++)
                {
                    if (currPercentage + 1.0f > percentage)
                    {
                        //Debug.Log("CurrPercentage is " + currPercentage);
                        //Debug.LogFormat("At {0} ({1}) we should not fill completely", currPercentage, percentage);

                        InstantiateFlag(country, currCol, currLine, percentage - currPercentage);

                        totalPercentage += percentage - currPercentage;
                    }
                    else
                    {
                        InstantiateFlag(country, currCol, currLine, percentage);

                        totalPercentage++;

                        currCol = Mathf.FloorToInt(totalPercentage) % NbrColumns;
                        currLine = (int)Mathf.Floor(Mathf.FloorToInt(totalPercentage) / NbrColumns);
                    }
                }
            }
            else
            {
                //If percentage< 1.0f, we fill with the world icon

                InstantiateWorldFlag(currCol, currLine, totalPercentage);

                break;
            }
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
