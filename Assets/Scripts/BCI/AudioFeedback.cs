using UnityEngine;

namespace BCI
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioFeedback : MonoBehaviour
    {

        public AudioClip hitSound;
        //public AudioClip missSound;

        private AudioSource audioSource;
        private AudioClip cueTone;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            cueTone = ToneGenerator.CreateTone("CueBeep", 880f, 0.15f, 0.4f);
        }

        void Start()
        {
            if (TrialManager.Instance == null)
            {
                Debug.LogError("AudioFeedback: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnCueStart += _ => audioSource.PlayOneShot(cueTone);
            TrialManager.Instance.OnHit += () => audioSource.PlayOneShot(hitSound);
            //TrialManager.Instance.OnMiss += () => audioSource.PlayOneShot(missSound);
        }
    }
}