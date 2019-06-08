using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class UILoadVideo : MonoBehaviour
{
    public string[] m_IdArray;
    public int m_LastReduceFrameCount;
    private int m_Index = 0;
    public static UILoadVideo Instance;
    public bool IsFinished
    {
        get
        {
            return m_Index == 3;
        }
    }
    public VideoPlayer m_videoPlayer;
    private void Awake()
    {
        Instance = this;
        int width = Screen.width;
        int height = Screen.height;
        int designWidth = 960;
        int designHeight = 640;
        float s1 = (float)designWidth / (float)designHeight;
        float s2 = (float)width / (float)height;
        if (s1 < s2)
        {
            designWidth = Mathf.FloorToInt(designHeight * s2);
        }
        else if (s1 > s2)
        {
            designHeight = Mathf.FloorToInt(designWidth / s2);
        }
        float contentScale = (float)designWidth / width;
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(designWidth, designHeight);
        }
        m_videoPlayer = GetComponent<VideoPlayer>();

        //m_videoPlayer.loopPointReached += OnLoopPointReached;
        m_videoPlayer.sendFrameReadyEvents = true;
        m_videoPlayer.prepareCompleted += OnPrepareCompleted;
        PlayVideo(m_videoPlayer);
    }
    private void PlayVideo(VideoPlayer source)
    {
        var uiVideoPathTables = m_IdArray[m_Index++];
        string ppath = Application.persistentDataPath + "/PayLoad/" + uiVideoPathTables;
        if (File.Exists(ppath))
        {
            source.url = ppath;
        }
        else
        {
            source.url = string.Format("{0}/{1}", Application.streamingAssetsPath, uiVideoPathTables);
        }
        m_frame = 0;
        source.Prepare();
    }
    public void OnLoopPointReached(VideoPlayer source)
    {
        if (m_Index == 1)
        {
            source.Pause();
            PlayVideo(source);
        }
        else
        {
            source.Stop();
            m_Index = 3;
        }
    }
    private ulong m_frame = 0;
    private void Update()
    {
        if (m_videoPlayer == null || !m_videoPlayer.isPrepared)
            return;
        if (m_frame == m_videoPlayer.frameCount)
        {
            OnLoopPointReached(m_videoPlayer);
            m_frame++;
            return;
        }
        if (m_frame > m_videoPlayer.frameCount)
            return;
        m_frame++;
    }
    private void OnPrepareCompleted(VideoPlayer source)
    {
        var rawImage = GetComponent<RawImage>();
        rawImage.texture = source.texture;
        source.Play();
    }
}