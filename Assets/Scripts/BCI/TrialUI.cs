using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BCI
{
    public class TrialUI : MonoBehaviour
    {
        public Text cueText;
/*        public Image startIcon;
        public Image stopIcon;
        public float flashDuration = 0.5f;
        public float feedbackTextDelay = 0.57f;

        private Coroutine startFlashRoutine;
        private Coroutine stopFlashRoutine;*/

        void Start()
        {
            if (TrialManager.Instance == null)
            {
                Debug.LogError("TrialUI: no TrialManager found in the scene.");
                return;
            }

            var trialManager = TrialManager.Instance;

            trialManager.OnRestStart += () => SetText("");
            trialManager.OnFixationStart += () => SetText("+");
            trialManager.OnCueStart += text => SetText(text);
            trialManager.OnImageryStart += () => SetText("...");
            trialManager.OnHit += () => SetText("Odmarajte.");
            trialManager.OnMiss += () => SetText("Odmarajte.");

            /*//trialManager.OnImageryStart += () => Flash(startIcon, ref startFlashRoutine);
            trialManager.OnHit += () =>
            {
                //Flash(stopIcon, ref stopFlashRoutine);
                StartCoroutine(DelayedFeedbackText());
            };
            trialManager.OnMiss += () =>
            {
                //Flash(stopIcon, ref stopFlashRoutine);
                StartCoroutine(DelayedFeedbackText());
            };

            startIcon.gameObject.SetActive(false);
            stopIcon.gameObject.SetActive(false);
            */
        }
        /*
        IEnumerator DelayedFeedbackText()
        {
            yield return new WaitForSeconds(feedbackTextDelay);
            SetText("Odmarajte.");
        }

        void Flash(Image icon, ref Coroutine routine)
        {
            if (routine != null)
                StopCoroutine(routine);
            routine = StartCoroutine(FlashRoutine(icon));
        }

        IEnumerator FlashRoutine(Image icon)
        {
            icon.gameObject.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            icon.gameObject.SetActive(false);
        }
        */

           void SetText(string text)
        {
            if (cueText != null)
                cueText.text = text;
        }



    }
}