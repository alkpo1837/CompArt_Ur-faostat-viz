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
    public int SizeFlag = 64;

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

        float posX = currCol * SizeFlag;
        float posY = -currLine * SizeFlag;
        flagIconGo.transform.localPosition = new Vector2(posX, posY);

        if (fillAmount < 1.0f)
        {
            Debug.LogWarningFormat("Filling {0} with fillAMount {1}", country, fillAmount);
            flagIconBehaviour.SetFillAmount(fillAmount, fillOrigin);
        }

        flagIconGo.name = string.Format("{0} : {1} [Index = {2}]", country, fillAmount, currLine * NbrColumns + currCol);

        _flagsIcon.Add(flagIconBehaviour);

        flagIconBehaviour.OnMouseOver += OnFlagMouseOver;
    }

    public void InstantiateWorldFlag(int currCol, int currLine, float totalPercentage)
    {
        GameObject worldFlagIconGo = Instantiate(WorldFlagPrefab, FlagIconsParent);
        WorldFlagIcon worldFlagIconBehaviour = worldFlagIconGo.GetComponent<WorldFlagIcon>();

        float posX = currCol * (SizeFlag);
        float posY = -currLine * SizeFlag;

        worldFlagIconGo.transform.localPosition = new Vector2(posX, posY);
        worldFlagIconBehaviour.SetSize(NbrColumns - currCol);

        // Set Size

        worldFlagIconGo.name = string.Format("World: {0}", 100.0f - totalPercentage);
    }

    //public void FillFlags(List<JSONObject> listStats)
    //{
    //    int currLine = 0, currCol = 0;
    //    float totalPercentage = 0.0f;
    //    float currPercentage = 0.0f;
    //    int currIndex = 0;

    //    if (_flagsIcon.Count > 0)
    //    {
    //        foreach (FlagIcon flagIcon in _flagsIcon)
    //            Destroy(flagIcon.gameObject);

    //        _flagsIcon.Clear();
    //    }

    //    Debug.LogFormat("_ratioPerFlag = {0}", _ratioPerFlag);

    //    foreach (JSONObject statJSON in listStats)
    //    {
    //        string country = statJSON.keys[0];
    //        float percentage = statJSON[country].f;

    //        Debug.LogWarningFormat("We got {0} with {1} %", country, percentage);

    //        if (percentage > 1.0f)
    //        {
    //            for (; currPercentage < percentage + DECIMAL_MARGIN; currPercentage += _ratioPerFlag)
    //            {
    //                currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
    //                currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

    //                Debug.LogFormat("CurrIndex => {0}", currIndex);
    //                Debug.LogFormat("CurrCol = {0}, CurrLine = {1}", currCol, currLine);
    //                Debug.Log("CurrPercentage is " + currPercentage);

    //                if (currPercentage + _ratioPerFlag > percentage + DECIMAL_MARGIN)
    //                {
    //                    Debug.LogWarningFormat("At {0} ({1}) we should not fill completely", currPercentage, percentage);

    //                    InstantiateFlag(country, currCol, currLine, percentage - currPercentage, percentage - currPercentage);

    //                    totalPercentage += percentage - currPercentage;

    //                    break;
    //                }
    //                else
    //                {
    //                    InstantiateFlag(country, currCol, currLine, percentage);

    //                    totalPercentage += _ratioPerFlag;
    //                    currIndex++;
    //                }
    //            }

    //            Debug.LogErrorFormat("TotalPercentage {0}, TotalPer / ratioPerFlag is {1}", totalPercentage, totalPercentage % _ratioPerFlag);

    //            if (totalPercentage % _ratioPerFlag > DECIMAL_MARGIN)
    //            {
    //                float fillAmount = 1.0f - (totalPercentage % _ratioPerFlag);
    //                Debug.LogError("FILLING LEFT PART");
    //                InstantiateFlag(country, currCol, currLine, percentage, fillAmount, 1);

    //                // This is the percentage we're curently at
    //                currPercentage = fillAmount;
    //                totalPercentage += fillAmount;

    //                currIndex++;
    //            }
    //            else
    //            {
    //                currPercentage = 0.0f;
    //            }
    //        }
    //        else
    //        {
    //            //If percentage< 1.0f, we fill with the world icon

    //            InstantiateWorldFlag(currCol, currLine, totalPercentage);

    //            break;
    //        }
    //    }
    //}

    public void FillFlags(List<JSONObject> listStats)
    {
        if (_flagsIcon.Count > 0)
        {
            foreach (FlagIcon flagIcon in _flagsIcon)
                Destroy(flagIcon.gameObject);

            _flagsIcon.Clear();
        }

        Dictionary<string, float> nbrFlagsByCountries = new Dictionary<string, float>();

        foreach (JSONObject statJSON in listStats)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            if (percentage > 0.1f)
                nbrFlagsByCountries[country] = (NbrColumns * NbrLines) * (percentage / 100.0f);
        }

        int currLine = 0, currCol = 0;
        float totalPercentage = 0.0f;
        float currFlagFill = 0.0f;
        int currIndex = 0;

        foreach (KeyValuePair<string, float> nbrFlagByCountry in nbrFlagsByCountries)
        {
            float nbrFlags = nbrFlagByCountry.Value;

            Debug.Log(nbrFlagByCountry.Key + " => " + nbrFlagByCountry.Value);

            // That means that the previous filling was not complete
            if (currFlagFill > 0.0f && currFlagFill < 1.0f)
            {
                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value, 1 - currFlagFill, 1);
                currIndex++;

                Debug.Log("Previous currFlagFill is " + currFlagFill);

                nbrFlags -= (1 - currFlagFill);

                Debug.Log(nbrFlags);
            }

            for (int i = 0; i < Mathf.Floor(nbrFlags); i++)
            {
                currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value);

                totalPercentage += i;
                currIndex++;

                Debug.Log(i + " vs " + nbrFlags);

                currFlagFill = 1.0f;
            }

            // If there is a decimal part left, we should add this decimal part to the next flag icon
            if (nbrFlags - Math.Truncate(nbrFlags) > DECIMAL_MARGIN)
            {
                currFlagFill = nbrFlags - (float) Math.Truncate(nbrFlags);

                currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                //Debug.LogWarningFormat("At {0} ({1}) we should not fill completely", currPercentage, percentage);

                InstantiateFlag(nbrFlagByCountry.Key, currCol, currLine, nbrFlagByCountry.Value, currFlagFill);
            }

            //if (nbrFlagByCountry.Value > i)
        }

        //Debug.LogWarningFormat("We got {0} with {1} %", country, percentage);

        //if (percentage > 1.0f)
        //{
        //    for (; currPercentage < percentage + DECIMAL_MARGIN; currPercentage += _ratioPerFlag)
        //    {
        //        currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
        //        currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

        //        Debug.LogFormat("CurrIndex => {0}", currIndex);
        //        Debug.LogFormat("CurrCol = {0}, CurrLine = {1}", currCol, currLine);
        //        Debug.Log("CurrPercentage is " + currPercentage);

        //        if (currPercentage + _ratioPerFlag > percentage + DECIMAL_MARGIN)
        //        {
        //            Debug.LogWarningFormat("At {0} ({1}) we should not fill completely", currPercentage, percentage);

        //            InstantiateFlag(country, currCol, currLine, percentage - currPercentage, percentage - currPercentage);

        //            totalPercentage += percentage - currPercentage;

        //            break;
        //        }
        //        else
        //        {
        //            InstantiateFlag(country, currCol, currLine, percentage);

        //            totalPercentage += _ratioPerFlag;
        //            currIndex++;
        //        }
        //    }

        //    Debug.LogErrorFormat("TotalPercentage {0}, TotalPer / ratioPerFlag is {1}", totalPercentage, totalPercentage % _ratioPerFlag);

        //    if (totalPercentage % _ratioPerFlag > DECIMAL_MARGIN)
        //    {
        //        float fillAmount = 1.0f - (totalPercentage % _ratioPerFlag);
        //        Debug.LogError("FILLING LEFT PART");
        //        InstantiateFlag(country, currCol, currLine, percentage, fillAmount, 1);

        //        // This is the percentage we're curently at
        //        currPercentage = fillAmount;
        //        totalPercentage += fillAmount;

        //        currIndex++;
        //    }
        //    else
        //    {
        //        currPercentage = 0.0f;
        //    }
        //}
        //else
        //{
        //    //If percentage< 1.0f, we fill with the world icon

        //    InstantiateWorldFlag(currCol, currLine, totalPercentage);

        //    break;
        //}

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
