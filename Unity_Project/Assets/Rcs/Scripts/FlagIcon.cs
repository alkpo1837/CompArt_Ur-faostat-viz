using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlagIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _icon;

    private string _flagName;
    private float _percentage;

    public string FlagName { get { return _flagName; } }
    public float Percentage { get { return _percentage; } }

    public Action<FlagIcon> OnMouseOver;

    private void Awake()
    {
        _icon = GetComponent<Image>();

    }

    public void SetInformations(string flagName, float percentage)
    {
        _flagName = flagName;
        _percentage = percentage;

        Sprite flag = Resources.Load<Sprite>(string.Format("Flags/{0}", flagName));

        if (flag != null)
        {
            _icon.sprite = flag;
        }
        else
        {
            Debug.LogErrorFormat("Flag {0} does not exist", flagName);
        }
    }

    public void SetFillAmount(float fillAmount, int fillOrigin)
    {
        _icon.fillAmount = fillAmount;
        _icon.fillOrigin = fillOrigin;
    }

    //public void OnMouseOver()
    //{
        //Debug.Log("ON MOUSE OVER");

    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    { }

}
