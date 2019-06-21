using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CaptureAndSaveManager : MonoBehaviour
{

    int nPictureRealWith = 1280;
    int nPictureRealHeight = 720;

    public delegate void OnCaptureMainCameraAndUICameraFinishedCallback();
    public OnCaptureMainCameraAndUICameraFinishedCallback onCaptureMainCameraAndUICameraFinishedCallback;
    public GameObject lefttop;
    public GameObject leftbottom;
    public GameObject rightbottom;

    public Texture2D tex2DMainCameraAndUICamerasSreenShot = null;
    public void OnClickSave()
    {
        CaptureCameraExt(UICamera.currentCamera, lefttop, rightbottom, leftbottom);
    }

    List<Camera> cameras = null;
    public void ClearScreenShot()
    {
        if(tex2DMainCameraAndUICamerasSreenShot != null)
        {
            Destroy(tex2DMainCameraAndUICamerasSreenShot);
            tex2DMainCameraAndUICamerasSreenShot = null;
        }
    }


    void SaveTexture(Texture2D tex2D, string path)
    {
#if !UNITY_ANDROID
        try
        {
            byte[] bytes = tex2D.EncodeToPNG();
            string fullPath = GetFullPath();
            File.WriteAllBytes(fullPath, bytes);
            //GC.Collect();
            //Resources.UnloadUnusedAssets();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("CaptureAndSave", "SaveTexture(Texture2D tex2D, string path) Error: {0}", ex.Message);
        }
#endif
    }

    string GetFullPath()
    {
        string fileName = GetFileName();
        string text = Application.persistentDataPath + "/ScreenShots/" + fileName;
        string pathTemp = Application.persistentDataPath + "/ScreenShots/";
        string strDir = Path.GetDirectoryName(pathTemp);
        if (!Directory.Exists(strDir))
        {
            try
            {
                Directory.CreateDirectory(strDir);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("CaptureAndSave", "CreateDirectory Error: {0}", ex.Message);
            }
        }
        return text;
    }

    string GetFileName()
    {
        return string.Format("screen_{0}x{1}_{2}.png", nPictureRealWith, nPictureRealHeight, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")); ;
    }

    IEnumerator CaptureCameraSaveToGallery(List<Camera> cameras, Rect rect)
    {
        yield return new WaitForEndOfFrame();

        nPictureRealWith = (int)rect.width;
        nPictureRealHeight = (int)rect.height;

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        for (int i = 0; i < cameras.Count; i++)
        {
            Camera camera = cameras[i];
            camera.targetTexture = rt;
            camera.Render();
        }

        RenderTexture.active = rt;
        Texture2D screenShot = GetScreenShotTexture2D(rect);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        for (int i = 0; i < cameras.Count; i++)
        {
            Camera camera = cameras[i];
            camera.targetTexture = null;
        }

        RenderTexture.active = currentRT;

        RenderTexture.ReleaseTemporary(rt);

        //tex2DMainCameraAndUICamerasSreenShot = screenShot;

        if (onCaptureMainCameraAndUICameraFinishedCallback != null)
        {
            onCaptureMainCameraAndUICameraFinishedCallback();
        }



        //         RenderTexture currentRT = RenderTexture.active;
        //         RenderTexture.active = camera.targetTexture;
        //         camera.Render();
        //         Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        //         image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        //         image.Apply();
        //         RenderTexture.active = currentRT;
        //         SaveTextureToGallery(image);
    }

    private Texture2D GetScreenShotTexture2D(Rect rect)
    {
        if(tex2DMainCameraAndUICamerasSreenShot != null)
        {
            if(tex2DMainCameraAndUICamerasSreenShot.height == rect.height && tex2DMainCameraAndUICamerasSreenShot.width == rect.width)
            {
                return tex2DMainCameraAndUICamerasSreenShot;
            }
            else
            {
                Destroy(tex2DMainCameraAndUICamerasSreenShot);
                tex2DMainCameraAndUICamerasSreenShot = null;
            }
        }


        tex2DMainCameraAndUICamerasSreenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        return tex2DMainCameraAndUICamerasSreenShot;

    }
    IEnumerator CaptureCamera(List<Camera> cameras, Rect rect, Action<Texture2D> finishActionFunc = null)
    {
        yield return new WaitForEndOfFrame();

        int ratio = 1;
        rect.x *= ratio;
        rect.y *= ratio;
        rect.width *= ratio;
        rect.height *= ratio;

        nPictureRealWith = (int)rect.width;
        nPictureRealHeight = (int)rect.height;

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture rt = RenderTexture.GetTemporary(Screen.width * ratio, Screen.height * ratio, 24);

        for (int i = 0; i < cameras.Count; i++)
        {
            Camera camera = cameras[i];
            camera.targetTexture = rt;
            camera.Render();
        }

        RenderTexture.active = rt;

        Texture2D screenShot = GetScreenShotTexture2D(rect); 
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        for (int i = 0; i < cameras.Count; i++)
        {
            Camera camera = cameras[i];
            camera.targetTexture = null;
        }

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(rt);

        if (onCaptureMainCameraAndUICameraFinishedCallback != null)
        {
            onCaptureMainCameraAndUICameraFinishedCallback();
            onCaptureMainCameraAndUICameraFinishedCallback = null;
        }

        if (finishActionFunc != null)
        {
            finishActionFunc.Invoke(screenShot);
            finishActionFunc = null;
            //Destroy(screenShot);
        }
    }
    
    public static float DisPoint2Surface(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var a = (p2.y - p1.y) * (p3.z - p1.z) - (p2.z - p1.z) * (p3.y - p1.y);
        var b = (p2.z - p1.z) * (p3.x - p1.x) - (p2.x - p1.x) * (p3.z - p1.z);
        var c = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
        var d = 0 - (a * p1.x + b * p1.y + c * p1.z);
        return DisPoint2Surface(point, a, b, c, d);
    }

    static Vector3[] GetCorners(Camera theCamera, float distance)
    {
        Vector3[] corners = new Vector3[4];
        var tx = theCamera.transform;
        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = theCamera.aspect;
        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;
        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;
        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;
        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up         * height;
        corners[2] += tx.forward * distance;
        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;
        return corners;
    }

    static Vector3 GetPosition(Camera camera, float distance, Vector3 upperLeft)
    {
        var tf = camera.transform;
        float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float aspect = camera.aspect;
        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;
        var position = upperLeft - tf.forward * distance;
        position -= tf.up * height;
        position += tf.right * width;
        return position;
    }

    public static float DisPoint2Surface(Vector3 pt, float a, float b, float c, float d)
    {
        return Mathf.Abs(a * pt.x + b * pt.y + c * pt.z + d) / Mathf.Sqrt(a * a + b * b + c * c);
    }

    public void CaptureCameraExt(Camera cam, GameObject lefttop, GameObject rightbottom, GameObject obj)
    {
        var ltpos = lefttop.transform.position;
        var rbpos = rightbottom.transform.position;
        var sltpos = cam.WorldToScreenPoint(ltpos);
        var srbpos = cam.WorldToScreenPoint(rbpos);
        nPictureRealWith = Mathf.CeilToInt(srbpos.x - sltpos.x);
        nPictureRealHeight = Mathf.CeilToInt(sltpos.y - srbpos.y);
        var distance = DisPoint2Surface(cam.transform.position, ltpos, rbpos, obj.transform.position);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        RenderTexture.active = rt;
        var lines = nPictureRealWith / Screen.width;
        if (lines * Screen.width < nPictureRealWith)
            lines++;
        var columns = nPictureRealHeight / Screen.height;
        var offseth = nPictureRealHeight % Screen.height;
        if (offseth > 0)
            columns++;
        Texture2D png = new Texture2D(nPictureRealWith, nPictureRealHeight, TextureFormat.RGB24, false);
        var o = cam.transform.position;
        cam.transform.position = GetPosition(cam, distance, lefttop.transform.position);
        for (int i = 0; i < lines; i++)
        {
            var old = cam.transform.position;
            var w = Screen.width;
            var h = Screen.height;
            var offsetw = w * i;
            if (i == lines - 1)
            {
                w = nPictureRealWith - offsetw;
            }
            for(int j = 0;j < columns; j++)
            {
                cam.Render();
                var fixh = (columns - 1 - j) * h;
                if (j != columns - 1)
                {
                    fixh -= (h - offseth);
                    png.ReadPixels(new Rect(0, 0, w, h), offsetw, fixh);
                }
                else
                {
#if UNITY_EDITOR
                    png.ReadPixels(new Rect(0, 0, w, offseth), offsetw, 0);
#else
                    png.ReadPixels(new Rect(0, h - offseth, w, offseth), offsetw, 0);
#endif
                }
                var cors = GetCorners(cam, distance);
                cam.transform.position = GetPosition(cam, distance, cors[2]);
            }
            png.Apply();
            cam.transform.position = old;
            var corners = GetCorners(cam, distance);
            cam.transform.position = GetPosition(cam, distance, corners[1]);
        }
        cam.transform.position = o;
        cam.targetTexture = null;
        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(rt);
        byte[] bytes = png.EncodeToPNG();
        File.WriteAllBytes(GetFullPath(), bytes);
        Destroy(png);
        png = null;
    }

    
    public void CaptureFullScreen(int x, int y, int width, int height)
    {
        Rect rect = new Rect(x, y, width, height);
        StartCoroutine(InterceptFullScreen(rect));
    }

    IEnumerator InterceptFullScreen(Rect rect, Action<Texture2D> finishActionFunc = null)
    {
        yield return new WaitForEndOfFrame();

        nPictureRealWith = (int)rect.width;
        nPictureRealHeight = (int)rect.height;
        Texture2D screenShot = GetScreenShotTexture2D(rect);
       // Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        //tex2DMainCameraAndUICamerasSreenShot = screenShot;

        if (onCaptureMainCameraAndUICameraFinishedCallback != null)
        {
            onCaptureMainCameraAndUICameraFinishedCallback();
            onCaptureMainCameraAndUICameraFinishedCallback = null;
        }

        if (finishActionFunc != null)
        {
            finishActionFunc.Invoke(screenShot);
            finishActionFunc = null;
            //Destroy(screenShot);
        }
    }

    //屏幕左上角是(0,0)
    /*
    (0,0)------- +Y
    |
    |
    |
    +X
    */
    public void CaptureAndSaveToAlbum(int x, int y, int width, int height)
    {

    }
    
}
