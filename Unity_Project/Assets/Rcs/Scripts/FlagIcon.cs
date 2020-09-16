using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlagIcon : MonoBehaviour
{
    private Image _icon;
    
    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void SetFlag(string flagName)
    {
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
}
