using UnityEngine;
using UnityEngine.UI;

public class CropButtonBehaviour : MonoBehaviour
{
    public Button Button;
    public Text Text;

    public void SetText(string text)
    {
        Text.text = text;
    }
}
