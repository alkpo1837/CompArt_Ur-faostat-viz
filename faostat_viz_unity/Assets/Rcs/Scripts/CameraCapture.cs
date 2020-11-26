using System;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    public KeyCode screenshotKey;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            if (_camera != null)
                Capture();
            else
                Debug.Log("CAMERA IS NULL");
        }
    }

    public void Capture()
    {
        DateTime time = DateTime.Now;
        string screenshotName = $"TheScreenshot_{time.ToShortDateString().Replace("/", "_")}_{time.Second}";

        ScreenCapture.CaptureScreenshot(Application.dataPath + $"/Captures/TheScreenshot_{screenshotName}.png");

        Debug.Log($"{screenshotName} taken");
    }
}
