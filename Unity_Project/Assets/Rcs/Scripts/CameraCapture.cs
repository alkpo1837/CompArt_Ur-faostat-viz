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
        //RenderTexture activeRenderTexture = RenderTexture.active;
        //RenderTexture.active = _camera.targetTexture;
        //camera.targetTexture = renderTexture;

        //_camera.Render();

        //Texture2D image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height);
        //image.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
        //image.Apply();
        //RenderTexture.active = activeRenderTexture;

        //byte[] bytes = image.EncodeToPNG();
        //Destroy(image);

        //File.WriteAllBytes(Application.dataPath + "/Captures/" + Time.time.ToString("0.00") + ".png", bytes);

        //Debug.Log("Screenshot taken !");

        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Captures/TheScreenshot.png");
    }
}
