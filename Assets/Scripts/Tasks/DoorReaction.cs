using System.Collections;
using UnityEngine;
using BCI;

namespace Tasks
{
    public class DoorReaction : MonoBehaviour
    {
        public float openAngle = 90f;
        public float openSpeed = 65f;
        public float openDelay = 0f;

        private Quaternion closedRotation;
        private Quaternion targetRotation;
        private Coroutine rotateRoutine;

        void Start()
        {
            closedRotation = transform.localRotation;
            targetRotation = closedRotation;

            if (TrialManager.Instance == null)
            {
                Debug.LogError("DoorReaction: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnHit += HandleHit;
            TrialManager.Instance.OnRestStart += HandleReset;
        }

        void HandleHit()
        {
            if (rotateRoutine != null)
                StopCoroutine(rotateRoutine);
            rotateRoutine = StartCoroutine(OpenAfterDelay());
        }

        void HandleReset()
        {
            targetRotation = closedRotation;
            Restart();
        }

        IEnumerator OpenAfterDelay()
        {
            if (openDelay > 0f)
                yield return new WaitForSeconds(openDelay);

            targetRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
            yield return RotateTowardsTarget();
        }

        void Restart()
        {
            if (rotateRoutine != null)
                StopCoroutine(rotateRoutine);
            rotateRoutine = StartCoroutine(RotateTowardsTarget());
        }

        IEnumerator RotateTowardsTarget()
        {
            while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.5f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, openSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localRotation = targetRotation;
        }
    }
}