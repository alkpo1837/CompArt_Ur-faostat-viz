using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PosterSceneManager : MonoBehaviour
{
    [Header("Elements")]
    public Text Title;
    public StatsPoster StatsPoster;
    public VerticalLayoutGroup StatsLayoutGroup;

    [Header("Prefab")]
    public GameObject StatEntryPrefab;

    [Header("Values")]
    public TextAsset DataJSON;
    public string Crop = "Tomatoes";

    private JSONObject _jsonObject;

    private void Awake()
    {
        _jsonObject = new JSONObject(DataJSON.text);
    }

    void Start()
    {
        Title.text = string.Format("Production quantity of {0} per countries, as of 2018", Crop);

        StatsPoster.DrawCropStats(Crop);

        //DisplayListStats(Crop);
    }

    public void DisplayListStats(string crop)
    {
        List<JSONObject> dataCropList = GetDataCrop(crop);

        foreach (JSONObject statJSON in dataCropList)
        {
            string country = statJSON.keys[0];
            float percentage = statJSON[country].f;

            if (percentage > 1.0f)
            {
                GameObject statEntryGo = Instantiate(StatEntryPrefab, StatsLayoutGroup.transform);
                StatEntry statEntry = statEntryGo.GetComponent<StatEntry>();

                statEntry.Init(country, percentage);
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
