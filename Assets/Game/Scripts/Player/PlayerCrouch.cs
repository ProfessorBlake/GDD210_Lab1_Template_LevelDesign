using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
	public class PlayerCrouch : MonoBehaviour
	{
		[Header("Components")]
		[SerializeField] private Transform camTransform;
		[SerializeField] private CharacterController charController;

		[Header("Player Movement")]
		[SerializeField] private float mouseSensitivity;
		[SerializeField] private float moveSpeedStand;
		[SerializeField] private float moveSpeedCrouch;
		[SerializeField] private float gravity = -9.8f;
		[SerializeField] private float jumpSpeed;
		[SerializeField] private float yReset;
		private Vector3 startPosition;
		private Quaternion startRotation;

		[Header("Camera Movement")]
		[Tooltip("Camera offset when standing")]
		[SerializeField] private float camStandY;
		[Tooltip("Camera offset when crouched")]
		[SerializeField] private float camCrouchY;
		[Tooltip("Speed to lerp toward camera offset")]
		[SerializeField] private float camMoveSpeed;
		[Tooltip("Distance camera should be below top of collider")]
		[SerializeField] private float camOffsetFromTop;

		[Header("UI")]
		[SerializeField] private Image damageFlashImage;
		[SerializeField] private Timer timer;

		private float camOffset;
		private float camRotation = 0f;

		private float verticalSpeed;
		private bool isCrouching;
		private bool wasCrouching;

		private float colliderHeightTarget;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
			wasCrouching = false;
			camOffset = camStandY;
			charController.height = 2;
			charController.center = new Vector3(0, 0f, 0);
			startPosition = transform.position;
			startRotation = transform.rotation;
		}

		private void Update()
		{
			//Fell off map
			if (transform.position.y <= yReset) ResetPlayer();

			//Fade damage indicator
			if (damageFlashImage.color.a > 0.01f) damageFlashImage.color = Color.Lerp(damageFlashImage.color, Color.clear, Time.deltaTime);

			float mouseInputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
			camRotation -= mouseInputY;
			camRotation = Mathf.Clamp(camRotation, -90f, 90f);
			camTransform.localRotation = Quaternion.Euler(camRotation, 0f, 0f);

			float mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
			transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, mouseInputX));

			//Push
			if (Input.GetMouseButton(0))
			{
				if(Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, 4f))
				{
					Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
					if (rb)
					{
						rb.AddForceAtPosition(-hit.normal * 1, hit.point);
						//rb.transform.eulerAngles = new Vector3(0, rb.transform.eulerAngles.y, 0);
					}
				}

			}

			isCrouching = Input.GetKey(KeyCode.LeftControl);

			charController.height = camTransform.localPosition.y + camOffsetFromTop;
			charController.center = new Vector3(0, charController.height / 2, 0);

			// Start crouch
			if (isCrouching && !wasCrouching)
			{
				wasCrouching = true;
				camOffset = camCrouchY;
			}
			//End crouch
			else if (wasCrouching && !isCrouching)
			{
				if (!Physics.SphereCast(camTransform.position, charController.radius / 2, Vector3.up, out RaycastHit hit, camStandY + camOffsetFromTop - charController.radius * 2.5f))
				{
					wasCrouching = false;
					camOffset = camStandY;
				}
				else
				{
					isCrouching = true;
				}
			}


			float walkSpeed;
			if (isCrouching)
				walkSpeed = moveSpeedCrouch;
			else
				walkSpeed = moveSpeedStand;

			Vector3 movement = new Vector3();

			// X/Z movement
			float forwardMovement = Input.GetAxis("Vertical") * walkSpeed * Time.deltaTime;
			float sideMovement = Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime;

			movement += (transform.forward * forwardMovement) + (transform.right * sideMovement);

			// Y movement
			if (!isCrouching && (charController.isGrounded || Physics.Raycast(transform.position, Vector3.down, 0.1f)) && Input.GetKeyDown(KeyCode.Space))
			{
				verticalSpeed = jumpSpeed;
			}
			else if (charController.isGrounded)
			{
				verticalSpeed = 0f;
			}

			verticalSpeed += (gravity * (verticalSpeed > 0f ? 1f : 1.5f) * Time.deltaTime);
			movement += (transform.up * verticalSpeed * Time.deltaTime);

			//Camera offset
			camTransform.localPosition =
				Vector3.Lerp(camTransform.localPosition,
				new Vector3(camTransform.localPosition.x, camOffset, camTransform.localPosition.z),
				Time.deltaTime * camMoveSpeed);

			charController.Move(movement);
		}

		public void ResetPlayer()
		{
			verticalSpeed = 0;
			transform.position = startPosition; 
			transform.rotation = startRotation;
			damageFlashImage.color = new Color(0.8f, 0f, 0f, 0.5f);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Button button = hit.collider.GetComponent<Button>();
			if (button) 
			{
				button.Press();
			}

			Goal goal = hit.collider.GetComponent<Goal>();
			if (goal)
			{
				timer.Stop();
			}
		}	
	}
}