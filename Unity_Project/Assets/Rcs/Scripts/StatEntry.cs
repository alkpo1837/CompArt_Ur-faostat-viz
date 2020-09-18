using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatEntry : MonoBehaviour
{
    [Header("Elements")]
    public Image CountryFlag;
    public Text DataText;

    public void Init(string country, float percentage)
    {
        CountryFlag.sprite = Resources.Load<Sprite>(string.Format("Flags/{0}", country));
        DataText.text = string.Format("{0} : {1} %", country.Replace('-', ' '), percentage);
    }
}
