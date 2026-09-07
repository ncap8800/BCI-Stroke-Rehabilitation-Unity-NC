using UnityEngine;
using System.Collections;
using BCI;


namespace Tasks
{
    public class ButtonElevatorReaction : MonoBehaviour
    {
        public float pressLocation;
        public float pressSpeed = 65f;
        public float pressDelay = 0f;

        private Vector3 turnedOffPosition;
        private Vector3 targetPosition;
        private Coroutine moveRoutine;

        public Color litColour = Color.blue;
        public Color unlitColour = Color.black;
        private Renderer buttonRenderer;

        void Start()
        {
            turnedOffPosition = transform.localPosition;
            targetPosition = turnedOffPosition;

            buttonRenderer = GetComponent<Renderer>();

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

            buttonRenderer.material.EnableKeyword("_EMISSION");
            buttonRenderer.material.SetColor("_EmissionColor", litColour);
        }

        void HandleReset()
        {
            targetPosition = turnedOffPosition;
            Restart();

            buttonRenderer.material.SetColor("_EmissionColor", unlitColour);
        }

        IEnumerator OpenAfterDelay()
        {
            if (pressDelay > 0f)
                yield return new WaitForSeconds(pressDelay);

            targetPosition = turnedOffPosition + new Vector3(0f, 0f, pressLocation);
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
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.0005f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, pressSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = targetPosition;
        }
    }

}
