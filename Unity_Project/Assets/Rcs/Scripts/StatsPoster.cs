using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatsPoster : MonoBehaviour
{
    [Header("References")]
    public TextAsset DataJSON;

    [Header("Elements")]
    public Transform FlagSlotsParent;
    public Transform FlagIconsParent;

    [Header("Values")]
    public int NbrLines = 20;
    public int NbrColumns = 10;
    public float WidthFlag = 63.0f;
    public float HeightFlag = 43.0f;
    public float FlagSizeRatio = 1.0f;

    [Header("Prefab")]
    public GameObject FlagIconPrefab;
    public GameObject FlagSlotPrefab;

    public Action<FlagIcon> OnFlagMouseOver;

    private List<FlagIcon> _flagsIcon = new List<FlagIcon>();
    private JSONObject _jsonObject;

    public const float DECIMAL_MARGIN = 0.001f;
    
    public struct Flag
    {
        public string Country;
        public float PercentageValue;
        public float X;
        public float Y;
        public int CurrentIndex;
        public float FillAmount;

        public Flag(string country, float percentageValue, float x, float y, int currentIndex, float fillAmount = 1.0f)
        {
            Country = country;
            PercentageValue = percentageValue;
            X = x;
            Y = y;
            CurrentIndex = currentIndex;

            FillAmount = fillAmount;
        }

        public override string ToString()
        {
            return string.Format("{0} : X = {1}, Y = {2} [INDEX = {3}], FillAmount = {4} & Value = {5}", Country, X, Y, CurrentIndex, FillAmount, PercentageValue);
        }
    }

    private List<Flag> _flags;

    public void Awake()
    {
        _jsonObject = new JSONObject(DataJSON.text);

        _flags = new List<Flag>();
    }

    public void DrawCropStats(string cropText)
    {
        List<JSONObject> dataCropTomatoes = GetDataCrop(cropText);

        for (int i = 0; i < NbrColumns * NbrLines; i++)
        {
            GameObject flagSlot = Instantiate(FlagSlotPrefab, FlagSlotsParent);

            int currCol = Mathf.FloorToInt(i) % NbrColumns;
            int currLine = (int)Mathf.Floor(Mathf.FloorToInt(i) / NbrColumns);

            flagSlot.gameObject.name = string.Format("{0}, currCol = {1}, currLine = {2}", i, currCol, currLine);
            flagSlot.transform.localPosition = new Vector2(currCol * WidthFlag, -currLine * HeightFlag);

        }

        FillFlags(dataCropTomatoes);
    }

    public void InstantiateFlag(Flag flag)
    {
        GameObject flagIconGo = Instantiate(FlagIconPrefab, FlagIconsParent);
        FlagIcon flagIconBehaviour = flagIconGo.GetComponent<FlagIcon>();

        flagIconBehaviour.SetInformations(flag.Country, flag.PercentageValue);
        flagIconBehaviour.SetRatioSize(FlagSizeRatio);

        flagIconGo.transform.localPosition = new Vector2(flag.X, flag.Y);

        if (flag.FillAmount < 1.0f)
        {
            flagIconBehaviour.SetFillAmount(flag.FillAmount, 0);
        }

        flagIconGo.name = flag.ToString();

        _flagsIcon.Add(flagIconBehaviour);

        //flagIconBehaviour.OnMouseOver += OnFlagMouseOver;
    }

    public void FillFlags(List<JSONObject> listStats)
    {
        int currLine = 0, currCol = 0;
        float currFlagFill = 0.0f;
        int currIndex = 0;
        Dictionary<string, float> nbrFlagsByCountries = new Dictionary<string, float>();
        float flagsRemaining = NbrColumns * NbrLines;

        ClearAllFlags();

        float percentageTotal = 0.0f;

        foreach (JSONObject statJSON in listStats)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            if (percentage > DECIMAL_MARGIN)
            {
                nbrFlagsByCountries[country] = (NbrColumns * NbrLines) * (percentage / 100.0f);

                flagsRemaining -= nbrFlagsByCountries[country];

                percentageTotal += percentage;
            }
        }

        Debug.Log("Percentage is " + percentageTotal);

        foreach (KeyValuePair<string, float> nbrFlagByCountry in nbrFlagsByCountries)
        {
            float nbrFlags = nbrFlagByCountry.Value;

            //if (nbrFlags < 1.0f)
            Debug.LogFormat("Doing {0} with {1}", nbrFlagByCountry.Key, nbrFlagByCountry.Value);

            // That means that the previous filling was not complete
            if (currFlagFill > 0.0f)
            {
                //Debug.Log("The previous filling was not complete, it's currently at " + currFlagFill);
                //Debug.LogFormat("Calculus => {0} + {1} = {2}", currFlagFill, nbrFlagByCountry.Value, currFlagFill + nbrFlagByCountry.Value);
                
                float x = (currCol + currFlagFill) * WidthFlag;

                // If the currentCountry percetange + the previousFilling is more than one :
                // We have to fill the unfinished flag
                if (currFlagFill + nbrFlagByCountry.Value >= 1.0f - DECIMAL_MARGIN)
                {
                    float areaToFill = (float) Math.Round(1.0f - currFlagFill, 2);

                    //Debug.Log("FILLING COMPLETELY the flag");
                    _flags.Add(new Flag(nbrFlagByCountry.Key, nbrFlagByCountry.Value, x, -currLine * HeightFlag, currIndex, areaToFill));

                    currIndex++;
                    currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                    currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                    nbrFlags -= areaToFill;
                    currFlagFill = 0.0f;
                }
                // The next data will not fill completely
                else
                {
                    //Debug.Log("Filling the data, flag is not finished");
                    _flags.Add(new Flag(nbrFlagByCountry.Key, nbrFlagByCountry.Value, x, -currLine * HeightFlag, currIndex, nbrFlagByCountry.Value));

                    nbrFlags = 0;
                    currFlagFill += nbrFlagByCountry.Value;
                }
            }

            // Otherwise, if the previous is filled BUT the current is < 1.0f, we have to advance 
            if (currFlagFill == 0.0f && nbrFlags < 1.0f)
            {
                _flags.Add(new Flag(nbrFlagByCountry.Key, nbrFlagByCountry.Value, currCol * WidthFlag, -currLine * HeightFlag, currIndex, nbrFlags));

                currFlagFill = nbrFlags;
            }

            // Simple case
            if (nbrFlags > 1.0f)
            {
                for (int i = 0; i < Mathf.Floor(nbrFlags); i++)
                {
                    _flags.Add(new Flag(nbrFlagByCountry.Key, nbrFlagByCountry.Value, currCol * WidthFlag, -currLine * HeightFlag, currIndex, 1.0f));

                    currIndex++;
                    currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                    currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                    currFlagFill = 0.0f;
                }

                // If there is a decimal part left, we should add this decimal part to the next flag icon
                if (nbrFlags - Math.Truncate(nbrFlags) > DECIMAL_MARGIN)
                {
                    double decimalValue = (double)(nbrFlags - (double)Math.Truncate(nbrFlags));

                    currFlagFill = (float) Math.Round(decimalValue, 2);

                    currCol = Mathf.FloorToInt(currIndex) % NbrColumns;
                    currLine = (int)Mathf.Floor(Mathf.FloorToInt(currIndex) / NbrColumns);

                    _flags.Add(new Flag(nbrFlagByCountry.Key, nbrFlagByCountry.Value, currCol * WidthFlag, -currLine * HeightFlag, currIndex, currFlagFill));
                }
            }
        }

        foreach (Flag flag in _flags)
        {
            InstantiateFlag(flag);
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
