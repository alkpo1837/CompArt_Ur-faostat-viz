using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Elements")]
    public StatsPoster StatsPoster;
    public Transform ButtonParent;
    public Text InfosText;

    [Header("Prefab")]
    public GameObject ButtonPrefab;

    [Header("Data")]
    public TextAsset DataTextAsset;

    private JSONObject _dataJSON;

    void Start()
    {
        _dataJSON = new JSONObject(DataTextAsset.text);

        List<JSONObject> crops = _dataJSON.list;

        foreach (JSONObject crop in crops)
        {
            GameObject button = Instantiate(ButtonPrefab, ButtonParent);
            CropButtonBehaviour cropButtonBehaviour = button.GetComponent<CropButtonBehaviour>();

            cropButtonBehaviour.SetText(crop.keys[0]);
            cropButtonBehaviour.Button.onClick.AddListener(() => ClickOnCropButton(cropButtonBehaviour.Text.text));
        }

        StatsPoster.OnFlagMouseOver += OnFlagMouseOver;
    }

    public void OnFlagMouseOver(FlagIcon flagIcon)
    {
        InfosText.text = string.Format("{0} : {1} %", flagIcon.FlagName, flagIcon.Percentage);
    }

    private void ClickOnCropButton(string crop)
    {
        StatsPoster.DrawCropStats(crop);
    }
}
