using UnityEngine;
using UnityEngine.UI;

public class PosterSceneManager : MonoBehaviour
{
    [Header("Elements")]
    public Text Title;
    public StatsPoster StatsPoster;

    [Header("Values")]
    public string Crop = "Tomatoes";

    void Start()
    {
        Title.text = Crop;

        StatsPoster.DrawCropStats(Crop);
    }
}
