using UnityEngine;
using UnityEngine.UI;

namespace BCI
{
    [RequireComponent(typeof(Camera))]
    public class MirrorDisplayController : MonoBehaviour
    {
        public Text cueText;

        void Start()
        {
            if (SessionSettings.IsMirrored != true)
                return;

            if (cueText == null) // using cue text as a reference to find the right canvas to mirror
            {
                Debug.LogError("MirrorDisplayController: cueText not assigned - can't find the Canvas to mirror into.");
                return;
            }

            var camera = GetComponent<Camera>();
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

            camera.targetTexture = renderTexture;

            var displayFillerGameObject = new GameObject("DisplayFillerCamera");
            var displayFillerCamera = displayFillerGameObject.AddComponent<Camera>();
            displayFillerCamera.clearFlags = CameraClearFlags.SolidColor;
            displayFillerCamera.backgroundColor = Color.black;

            displayFillerCamera.cullingMask = 0;
            displayFillerCamera.depth = camera.depth - 1;

            Transform canvasTransform = cueText.transform.parent;

            var mirrorGameObject = new GameObject("MirroredCameraView");
            mirrorGameObject.transform.SetParent(canvasTransform, false);
            mirrorGameObject.transform.SetAsFirstSibling();

            var rawImage = mirrorGameObject.AddComponent<RawImage>();
            rawImage.texture = renderTexture;

            var rt = rawImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = new Vector3(-1f, 1f, 1f);

        }
    }
}