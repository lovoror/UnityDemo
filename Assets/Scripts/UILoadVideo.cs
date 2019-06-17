using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

public class UILoadVideo : MonoBehaviour
{
    public string[] m_IdArray;
    private int m_Index = 0;
    public static UILoadVideo Instance;
    public GameObject m_Quad;
    public bool IsFinished
    {
        get
        {
            return m_Index == 3;
        }
    }
    public VideoPlayer m_videoPlayer;
    private void Start()
    {
        Instance = this;
        float scale = ((float)Screen.width / Screen.height) / (1920f / 1080f);
        if (scale > 0)
            Camera.main.orthographicSize /= scale;
        m_videoPlayer = GetComponent<VideoPlayer>();
        m_videoPlayer.prepareCompleted += OnPrepareCompleted;
        StartCoroutine(DelayPlay());
    }
    private IEnumerator DelayPlay()
    {
        yield return new WaitForSeconds(3);
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
        source.Prepare();
    }
    public void OnStop(VideoPlayer source)
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
    private void Update()
    {
        if (m_videoPlayer == null || !m_videoPlayer.isPrepared)
            return;
        if (!m_videoPlayer.isPlaying)
        {
            OnStop(m_videoPlayer);
            return;
        }
        if (Input.touchCount > 0)
        {
            OnStop(m_videoPlayer);
        }
    }
    private void OnPrepareCompleted(VideoPlayer source)
    {
        m_Quad.SetActive(false);
        source.Play();
    }
}