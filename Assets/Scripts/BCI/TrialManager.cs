using System;
using System.Collections;
using UnityEngine;

namespace BCI
{
    public enum TrialPhase { Rest, Fixation, Cue, Imagery, Feedback }

    public class TrialResult
    {
        public int trialIndex;
        public float cueTime;
        public float imageryStartTime;
        public bool hit;
        public string rawMarkerValue;
        public float reactionTime; // -1 if no action was detected
    }

    public class TrialManager : MonoBehaviour
    {
        public static TrialManager Instance { get; private set; }

        [Header("Phase duration (seconds)")]
        public float restDuration = 3f;
        public float fixationDuration = 2f;
        public float cueDuration = 2f;
        public float imageryDuration = 4f;
        public float feedbackDuration = 2f;

        [Tooltip("Text shown during the Cue phase.")]
        public string cueText = "Zamisljajte pokret.";
        [Tooltip("How many trials before the scenario finishes.")]
        public int totalTrials = 4;


        public TrialPhase CurrentPhase { get; private set; } = TrialPhase.Rest;

        public int CurrentTrialIndex { get; private set; } = 0;


        public event Action OnRestStart;
        public event Action OnFixationStart;
        public event Action<string> OnCueStart;
        public event Action OnImageryStart;
        public event Action OnHit;
        public event Action OnMiss;
        public event Action<TrialResult> OnTrialEnd;

        private bool imageryListening = false;
        private bool markerHitThisTrial = false;
        private string lastRawMarker = "";
        private float imageryStartTime;
        private float reactionTime = -1f;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (LSLInlet.Instance != null)
                LSLInlet.Instance.OnMarkerReceived += HandleMarker;
            else
                Debug.LogError("TrialManager: no LSLInlet found in the scene.");
            StartCoroutine(RunOneTrial());
        }

        void OnDestroy()
        {
            if (LSLInlet.Instance != null)
                LSLInlet.Instance.OnMarkerReceived -= HandleMarker;
        }

        void HandleMarker(MarkerLabel label, string raw)
        {
            if (!imageryListening || markerHitThisTrial)
                return;

            if (label == MarkerLabel.Action)
            {
                markerHitThisTrial = true;
                reactionTime = Time.time - imageryStartTime;
            }
            lastRawMarker = raw;
        }

        IEnumerator RunOneTrial()
        {
            while (CurrentTrialIndex < totalTrials)
            {
                CurrentTrialIndex++;

                CurrentPhase = TrialPhase.Rest;
                //Debug.Log("Rest");
                OnRestStart?.Invoke();
                yield return new WaitForSeconds(restDuration);

                CurrentPhase = TrialPhase.Fixation;
                //Debug.Log("Fixation");
                OnFixationStart?.Invoke();
                yield return new WaitForSeconds(fixationDuration);

                CurrentPhase = TrialPhase.Cue;
                //Debug.Log("Cue");
                OnCueStart?.Invoke(cueText);
                yield return new WaitForSeconds(cueDuration);

                CurrentPhase = TrialPhase.Imagery;
                markerHitThisTrial = false;
                reactionTime = -1f;
                lastRawMarker = "";
                imageryStartTime = Time.time;
                imageryListening = true;
                Debug.Log($"Trial {CurrentTrialIndex}: imagery window OPEN");
                OnImageryStart?.Invoke();
                yield return new WaitForSeconds(imageryDuration);
                imageryListening = false;
                Debug.Log($"Trial {CurrentTrialIndex}: imagery window CLOSED - {(markerHitThisTrial ? "HIT" : "MISS")}");

                CurrentPhase = TrialPhase.Feedback;
                //Debug.Log("Feedback");
                if (markerHitThisTrial)
                    OnHit?.Invoke();
                else
                    OnMiss?.Invoke();
                yield return new WaitForSeconds(feedbackDuration);

                OnTrialEnd?.Invoke(new TrialResult
                {
                    trialIndex = CurrentTrialIndex,
                    cueTime = imageryStartTime - cueDuration,
                    imageryStartTime = imageryStartTime,
                    hit = markerHitThisTrial,
                    rawMarkerValue = lastRawMarker,
                    reactionTime = reactionTime,
                });
            }
        }
    }
}