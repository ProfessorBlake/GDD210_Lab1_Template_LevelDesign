using UnityEngine;

namespace GameP
{
    public class PushableIcon : MonoBehaviour
    {
        [SerializeField] private Transform icon;
		[SerializeField] private Transform cam;
		[SerializeField] private float maxRange;

		private void Update()
		{
			bool show = false;
			if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxRange))
			{
				Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
				if (rb)
				{
					show = true;
					icon.position = hit.point + hit.normal * 0.04f;
					icon.forward = -hit.normal;
				}
			}
			icon.localScale = Vector3.Lerp(icon.localScale, show ? Vector3.one * (0.1f + Mathf.Sin(Time.time * 2) * 0.025f) : Vector3.zero, Time.deltaTime * 50f);
		}
	}
}