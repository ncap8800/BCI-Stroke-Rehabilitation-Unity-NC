using System.Collections;
using UnityEngine;
using BCI;

namespace Tasks
{
    public class BoneRotateTo : MonoBehaviour
    {
        public Vector3 targetLocalEulerAngles;
        public float rotateSpeed = 300f; // degrees per second
        public float holdTime = 0.15f;
        public float startDelay = 0f; // for sequencing multiple bones one after another

        private Quaternion restLocalRotation;
        private Coroutine routine;

        void Start()
        {
            restLocalRotation = transform.localRotation;

            if (TrialManager.Instance == null)
            {
                Debug.LogError("BoneRotateTo: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnHit += HandleHit;
            TrialManager.Instance.OnRestStart += HandleReset;
        }

        void HandleHit()
        {
            if (routine != null)
                StopCoroutine(routine);
            routine = StartCoroutine(MoveToTargetAndReturn());
        }

        void HandleReset()
        {
            if (routine != null)
                StopCoroutine(routine);
            transform.localRotation = restLocalRotation;
        }

        IEnumerator MoveToTargetAndReturn()
        {
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            Quaternion target = Quaternion.Euler(targetLocalEulerAngles);
            yield return RotateTo(target);
            yield return new WaitForSeconds(holdTime);
            yield return RotateTo(restLocalRotation);
        }

        IEnumerator RotateTo(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, rotateSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localRotation = target;
        }
    }
}