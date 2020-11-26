using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PosterSceneManager : MonoBehaviour
{
    [Header("Elements")]
    public GameObject CropSelectorRoot;
    public GridLayout CropButtonsParent;
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
        CropSelectorRoot.SetActive(true);
        StatsPoster.gameObject.SetActive(false);

        StatsPoster.SetJSONData(_jsonObject);

        CreateCropButtons();
    }

    private void CreateCropButtons()
    {
        List<JSONObject> cropsJSON = _jsonObject.list;

        List<string> crops = new List<string>();

        foreach (JSONObject crop in cropsJSON)
        {
            crops.Add(crop.keys[0]);
        }

        crops.Sort();

        foreach (string crop in crops)
        {
            GameObject button = Instantiate(CropButtonPrefab, CropButtonsParent.transform);
            CropButtonBehaviour cropButtonBehaviour = button.GetComponent<CropButtonBehaviour>();

            button.name = crop;
            cropButtonBehaviour.SetText(crop);
            cropButtonBehaviour.Button.onClick.AddListener(() => ClickOnCropButton(cropButtonBehaviour.Text.text));
        }
    }

    private void ClickOnCropButton(string crop)
    {
        CropSelectorRoot.SetActive(false);
        StatsPoster.gameObject.SetActive(true);

        StatsPoster.DrawCropStats(crop);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CropSelectorRoot.SetActive(true);

            StatsPoster.ClearAllFlags();
            StatsPoster.gameObject.SetActive(false);
        }
    }
}
