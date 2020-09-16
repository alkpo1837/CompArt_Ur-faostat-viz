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

    private List<FlagIcon> _flagsIcon = new List<FlagIcon>();

    private JSONObject _jsonObject;

    public void Awake()
    {
        _jsonObject = new JSONObject(DataJSON.text);

;
    }

    public void Start()
    {
        List<JSONObject> dataCropTomatoes = GetDataCrop("Tomatoes");

        FillFlags(dataCropTomatoes);
    }

    public void InstantiateFlag(string country, int currCol, int currLine)
    {
        GameObject flagIconGo = Instantiate(FlagIconPrefab, FlagIconsParent);
        FlagIcon flagIconBehaviour = flagIconGo.GetComponent<FlagIcon>();

        flagIconBehaviour.SetFlag(country);

        flagIconGo.transform.localPosition = new Vector2(currCol * SizeFlag, -currLine * SizeFlag);

        _flagsIcon.Add(flagIconBehaviour);
    }

    public void FillFlags(List<JSONObject> listStats)
    {
        int currLine = 0, currCol = 0;
        int currFlagNumber = 0;

        foreach (JSONObject statJSON in listStats)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            Debug.LogFormat("We got {0} during {1}", country, percentage);

            if (percentage > 1.0f)
            {
                for (int i = 0; i < Mathf.FloorToInt(percentage); i++)
                {
                    InstantiateFlag(country, currCol, currLine);

                    currFlagNumber++;

                    currCol = currFlagNumber % NbrColumns;
                    currLine = (int)Mathf.Floor(currFlagNumber / NbrColumns);
                }
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
