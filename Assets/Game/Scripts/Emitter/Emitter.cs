using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[RequireComponent (typeof (LineRenderer))]
	public class Emitter : MonoBehaviour
	{
		public bool IsSecondary{ get; set; }
		public Color Col => col;
		public LayerMask HitLayers => hitLayers;

		[SerializeField] private Color col;
		[SerializeField] private LayerMask hitLayers;

		private LineRenderer lineRenderer;

		private void Awake()
		{
			//if (isSecondary)
			//{
			//	DestroyImmediate(gameObject);
			//}
		}

		private void Start()
		{
			// Get LR
			lineRenderer = GetComponent<LineRenderer>();

			//Set color
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(col, 0.0f), new GradientColorKey(col, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
			);
			lineRenderer.colorGradient = gradient;

			//Get points
			UpdatePoints();
		}

		private void LateUpdate()
		{
			UpdatePoints();
		}

		private void UpdatePoints()
		{
			int bounces = 10;
			List<Vector3> hitPoints = new List<Vector3>() { transform.position};
			Vector3 lastPoint = transform.position;
			Vector3 lastDir = transform.forward;
			while(bounces > 0)
			{
				bounces--;
				if(Physics.Raycast(lastPoint, lastDir, out RaycastHit hit, 9999f, hitLayers))
				{
					// Check Player hit
					PlayerCrouch player = hit.transform.GetComponent<PlayerCrouch>();
					if (player != null)  // Hit player
					{
						hitPoints.Add(hit.point);
						lastPoint = hit.point;
						bounces = 0;
						player.ResetPlayer();
						goto Show;
					}

					//Check Receiver
					Receiver hitRec = hit.collider.GetComponent<Receiver>();
					if(hitRec != null)
					{
						//Debug.Log("HIT REC");
						hitRec.SignalReceived(this);
						hitPoints.Add(hit.point);
						lastPoint = hit.point;
						bounces = 0;
						goto Show;
					}

					// Check Reflector
					Reflector hitReflector = hit.collider.GetComponent<Reflector>();
					if (hitReflector != null)  // Hit reflector
					{
						Vector3 reflect = Vector3.Reflect(lastDir, hit.normal);
						// Bounce
						if (Vector3.Dot(lastDir, reflect) < 0.999 && Vector3.Dot(lastDir, reflect) > -0.999)
						{
							hitPoints.Add(hit.point);
							lastPoint = hit.point;
							lastDir = Vector3.Reflect(lastDir, hit.normal);
							continue;
						}
						else  // reflected back so stop
						{
							hitPoints.Add(hit.point);
							lastPoint = hit.point;
							bounces = 0;
							goto Show;
						}
					}

					// Hit something else
					hitPoints.Add(hit.point);
					lastPoint = hit.point;
					bounces = 0;
					goto Show;
				}
				// Hit nothing
				hitPoints.Add(transform.position + transform.forward * 1000);
				bounces = 0;
				goto Show;
			}
			Show:
				lineRenderer.positionCount = hitPoints != null ? hitPoints.Count : 0;
				lineRenderer.SetPositions(hitPoints.ToArray());
		}

		public void EmitterActive(bool active)
		{
			if(active)
				UpdatePoints();
			else
			{
				lineRenderer.positionCount = 0;
				lineRenderer.SetPositions(new Vector3[0]);
			}

		}
	}
}