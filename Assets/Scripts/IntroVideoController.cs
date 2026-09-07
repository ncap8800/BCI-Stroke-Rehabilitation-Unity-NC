using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class IntroVideoController : MonoBehaviour
{
    public GameObject introVideoPanel;
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public Button openIntroVideoButton;
    public Button closeButton;

    private RenderTexture renderTexture;

    void Start()
    {
        renderTexture = new RenderTexture(1280, 720, 0);
        videoDisplay.texture = renderTexture;

        introVideoPanel.SetActive(false);

        openIntroVideoButton.onClick.AddListener(OpenIntroVideo);
        closeButton.onClick.AddListener(CloseIntroVideo);
    }

    void OpenIntroVideo()
    {
        introVideoPanel.SetActive(true);

        openIntroVideoButton.gameObject.SetActive(false);

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        videoPlayer.Play();
    }

    void CloseIntroVideo()
    {
        videoPlayer.Stop();
        introVideoPanel.SetActive(false);
        openIntroVideoButton.gameObject.SetActive(true);
    }
}
