using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private CharacterController characterController;
	private Animator animator;

	[SerializeField]
	private float forwardMoveSpeed = 3f;
	[SerializeField]
	private float backwardMoveSpeed = 1;
	[SerializeField]
	private float turnSpeed = 50f;

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
	}


	//move towards the mouse. I haven't connected this to the "keyCtrl". It's not cute, would be better to just turn the head part towards the mouse and not the whole body
	private void Update()
	{
		var horizontal = Input.GetAxis("Mouse X");
		var vertical = Input.GetAxis("Vertical");

		var movement = new Vector3(horizontal, 0, vertical);

		animator.SetFloat("Speed", vertical);

		transform.Rotate(Vector3.up, horizontal * turnSpeed * Time.deltaTime);

		if (vertical != 0)
		{
			float moveSpeedToUse = (vertical > 0) ? forwardMoveSpeed : backwardMoveSpeed;

			characterController.SimpleMove(transform.forward * moveSpeedToUse * vertical);
		}
	}
}