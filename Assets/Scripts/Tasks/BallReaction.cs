using UnityEngine;
using BCI;

namespace Tasks
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallReaction : MonoBehaviour
    {
        public float kickForce = 6f;
        public Vector3 kickDirection = Vector3.forward;

        private Rigidbody body;
        private Vector3 startPosition;
        private Quaternion startRotation;

        void Start()
        {
            body = GetComponent<Rigidbody>();
            startPosition = transform.position;
            startRotation = transform.rotation;

            if (TrialManager.Instance == null)
            {
                Debug.LogError("BallReaction: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnHit += HandleHit;
            TrialManager.Instance.OnRestStart += HandleReset;
        }

        void HandleHit()
        {
            body.AddForce(kickDirection.normalized * kickForce, ForceMode.Impulse);
        }

        void HandleReset()
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}