using UnityEngine;
using System.Collections;
using BCI;


namespace Tasks
{
    public class ElevatorDoorReaction : MonoBehaviour
    {
        public float openLocation;
        public float openSpeed = 65f;
        public float openDelay = 0f;

        private Vector3 closedPosition;
        private Vector3 targetPosition;
        private Coroutine moveRoutine;

        void Start()
        {
            closedPosition = transform.localPosition;
            targetPosition = closedPosition;

            if (TrialManager.Instance == null)
            {
                Debug.LogError("ButtonElevatorReaction: no TrialManager found in the scene.");
                return;
            }

            TrialManager.Instance.OnHit += HandleHit;
            TrialManager.Instance.OnRestStart += HandleReset;
        }

        void HandleHit()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(OpenAfterDelay());
        }

        void HandleReset()
        {
            targetPosition = closedPosition;
            Restart();
        }

        IEnumerator OpenAfterDelay()
        {
            if (openDelay > 0f)
                yield return new WaitForSeconds(openDelay);

            targetPosition = closedPosition + new Vector3(openLocation, 0f, 0f);
            yield return MoveTowardsTarget();
        }

        void Restart()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(MoveTowardsTarget());
        }

        IEnumerator MoveTowardsTarget()
        {
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.00005f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, openSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = targetPosition;
        }
    }

}
