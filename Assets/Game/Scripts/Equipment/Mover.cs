using UnityEngine;

namespace Game
{
    /// <summary>
    /// Triggerable movement component.
    /// </summary>
    public class Mover : MonoBehaviour
    {
        [Tooltip("Distance and direction to move")]
		[SerializeField] private Vector3 moveVector;
        [Tooltip("Move speed")]
		[SerializeField] private float speed;
        [Tooltip("Time before move should head back to start position once triggered")]
        [SerializeField] private float resetTime;
        [Tooltip("Don't wait for a trigger, always move")]
        [SerializeField] private bool alwaysTriggered;

        private bool isTriggered;
		private Vector3 startPos;
        private float resetDelay;
        private Vector3 vel;

		private void Start()
		{
			startPos = transform.position;
		}

		private void Update()
		{
            if (isTriggered)
            {
                if (resetTime >= 0 && resetDelay < Time.time && Vector3.Distance(transform.position,startPos+moveVector) < 0.01f)
                {
                    isTriggered = false;
					if (resetTime >= 0)
						resetDelay = Time.time + resetTime;
				}
                transform.position = Vector3.SmoothDamp(transform.position,startPos + moveVector, ref vel, speed * Time.deltaTime, speed);
            }
            else
            {
				if (alwaysTriggered && resetDelay < Time.time && Vector3.Distance(transform.position, startPos) < 0.01f)
				{
					isTriggered = true;
					if (resetTime >= 0)
						resetDelay = Time.time + resetTime;
				}
				transform.position = Vector3.SmoothDamp(transform.position, startPos, ref vel, speed * Time.deltaTime, speed);
			}
		}

        public void Trigger()
        {
            isTriggered = true;
            if(resetTime >= 0)
                resetDelay = Time.time + resetTime;
        }

		private void OnDrawGizmosSelected()
		{
            Gizmos.DrawLine(transform.position, transform.position + moveVector);
		}
	}
}