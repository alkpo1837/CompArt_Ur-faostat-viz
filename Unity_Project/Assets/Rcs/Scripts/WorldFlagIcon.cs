using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldFlagIcon : MonoBehaviour
{
    [Header("Elements")]
    public Image Borders;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = Borders.GetComponent<RectTransform>();
    }
    public void SetSize(int nbrColumns)
    {
        Borders.GetComponent<RectTransform>().sizeDelta = new Vector2(nbrColumns * 64, _rectTransform.sizeDelta.y);
    }
}
