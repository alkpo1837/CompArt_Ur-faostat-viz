using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PosterSceneManager : MonoBehaviour
{
    [Header("Elements")]
    //public Text Title;
    public GameObject CropButtonsParent;
    public StatsPoster StatsPoster;

    [Header("Prefab")]
    public GameObject CropButtonPrefab;
    public GameObject StatEntryPrefab;

    [Header("Values")]
    public TextAsset DataJSON;
    
    private JSONObject _jsonObject;

    private void Awake()
    {
        _jsonObject = new JSONObject(DataJSON.text);
    }

    void Start()
    {
        CropButtonsParent.SetActive(true);
        StatsPoster.gameObject.SetActive(false);

        CreateCropButtons();
        //StatsPoster.DrawCropStats(Crop);

        //DisplayListStats(Crop);
    }

    private void CreateCropButtons()
    {
        List<JSONObject> crops = _jsonObject.list;

        foreach (JSONObject crop in crops)
        {
            GameObject button = Instantiate(CropButtonPrefab, CropButtonsParent.transform);
            CropButtonBehaviour cropButtonBehaviour = button.GetComponent<CropButtonBehaviour>();

            cropButtonBehaviour.SetText(crop.keys[0]);
            cropButtonBehaviour.Button.onClick.AddListener(() => ClickOnCropButton(cropButtonBehaviour.Text.text));
        }
    }

    private void ClickOnCropButton(string crop)
    {
        CropButtonsParent.SetActive(false);
        StatsPoster.gameObject.SetActive(true);

        StatsPoster.DrawCropStats(crop);
    }

    //public List<JSONObject> GetDataCrop(string crop)
    //{
    //    List<JSONObject> dataCrop = null;
    //    List<JSONObject> _cropsJSON = _jsonObject.list;

    //    foreach (JSONObject jsonObject in _cropsJSON)
    //    {
    //        if (jsonObject.HasField(crop))
    //        {
    //            dataCrop = jsonObject.GetField(crop).list;

    //            break;
    //        }
    //    }

    //    if (dataCrop == null)
    //        Debug.LogErrorFormat("Error : crop {0} does not exist", crop);

    //    return dataCrop;
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CropButtonsParent.SetActive(true);
            StatsPoster.gameObject.SetActive(false);
        }
    }
}
