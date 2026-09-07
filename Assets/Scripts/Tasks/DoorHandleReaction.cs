using System.Collections;
using UnityEngine;
using BCI;

namespace Tasks
{
    public class DoorHandleReaction : MonoBehaviour
    {
        public Vector3 swingAxis = Vector3.right;
        public float swingAngle = 45f;
        public float swingSpeed = 300f; // degrees per second
        public float holdTime = 0.15f;
        public float startDelay = 0f;

        private Quaternion restLocalRotation;
        private Coroutine turnRoutine;

        void Start()
        {
            restLocalRotation = transform.localRotation;

            if (TrialManager.Instance == null)
            {
                Debug.LogError("DoorHandleReaction: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnHit += HandleHit;
            TrialManager.Instance.OnRestStart += HandleReset;
        }

        void HandleHit()
        {
            if (turnRoutine != null)
                StopCoroutine(turnRoutine);
            turnRoutine = StartCoroutine(TurnAndReturn());
        }

        void HandleReset()
        {
            if (turnRoutine != null)
                StopCoroutine(turnRoutine);
            transform.localRotation = restLocalRotation;
        }

        IEnumerator TurnAndReturn()
        {
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);
            Quaternion target = restLocalRotation * Quaternion.AngleAxis(swingAngle, swingAxis);
            yield return RotateTo(target);
            yield return new WaitForSeconds(holdTime);
            yield return RotateTo(restLocalRotation);
        }

        IEnumerator RotateTo(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, swingSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localRotation = target;
        }
    }
}