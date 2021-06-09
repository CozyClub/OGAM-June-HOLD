using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region variables
    private const float MIN_DEGREE = 0f;
    private const float MAX_DEGREE = 360f;
    private const float AVG_DEGREE = 180f;
    private const float CAMERA_LOOK_DISTANCE = 5f;
    [Header("Player movement")]
    [SerializeField]
    private float forwardMoveSpeed = 3f;
    [SerializeField]
    private float backwardMoveSpeed = 1;

    [Header("Camera rotations")]
    [SerializeField]
    private Transform playerEye = null;
    [SerializeField]
    private Transform playerPhysicalEye = null;
    [SerializeField]
    private Transform lookTarget = null;
    [SerializeField]
    [Range(0.1f, 0.99f)]
    private float lookSmoother = 0.2f;
    [SerializeField]
    [Range(1f, 20f)]
    private float rotationLimiter = 4f;
    [SerializeField]
    [Range(15f, 75f)]
    private float maxYRotation = 70f;

    private CharacterController characterController;
    private Animator animator;
    private float xAcc = 0f;
    private float yAcc = 0f;
    private float largeMinYRotation = 0f;
    private Vector2 mouseDelta = Vector2.zero;
    private Vector2 movementInput = Vector2.zero;

    #endregion

    #region input controlled functions
    public void GetDeltaInput(InputAction.CallbackContext context)
    {
        // Debug.LogWarning("got a delta" + context.ReadValue<Vector2>());
        if (Time.timeScale == 0f)
        {
            mouseDelta = Vector2.zero;
            return;
        }
        mouseDelta += context.ReadValue<Vector2>();
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
        {
            movementInput = Vector2.zero;
            return;
        }
        movementInput = context.ReadValue<Vector2>();
    }

    #endregion

    #region monobehaviour fns
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        largeMinYRotation = MAX_DEGREE - maxYRotation;
    }

    private void Start()
    {
        playerEye.localEulerAngles = Vector3.zero;
        lookTarget.parent = null;
    }

    //move towards the mouse. I haven't connected this to the "keyCtrl". It's not cute, would be better to just turn the head part towards the mouse and not the whole body
    private void Update()
    {
        animator.SetFloat("Speed", movementInput.y);
        // uses and empties mouseDelta to perform character + camera rotations
        mouseDelta = Rotations(mouseDelta);
        if (movementInput.y == 0f) return;
        float moveSpeedToUse = movementInput.y > 0 ? forwardMoveSpeed : -backwardMoveSpeed;
        // TODO when we have animations for moving left / right, should also move character in those directions
        characterController.Move(transform.forward * Time.deltaTime * moveSpeedToUse);
    }
    #endregion

    #region private fns
    private Vector2 Rotations(Vector2 mouseDelta)
    {
        xAcc = Mathf.Lerp(xAcc, mouseDelta.x / rotationLimiter, lookSmoother);
        yAcc = Mathf.Lerp(yAcc, mouseDelta.y / rotationLimiter, lookSmoother);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + xAcc, 0f);
        playerEye.eulerAngles = new Vector3(playerEye.eulerAngles.x + -yAcc, 0f, 0f);
        var localeuler = playerEye.eulerAngles.x;
        if (localeuler < AVG_DEGREE)
            localeuler = Mathf.Clamp(
                localeuler, MIN_DEGREE, maxYRotation);
        else
            localeuler = Mathf.Clamp(
                localeuler, largeMinYRotation, MAX_DEGREE);
        playerEye.eulerAngles = new Vector3(localeuler,
            transform.eulerAngles.y, 0f);
        lookTarget.position = transform.position + playerEye.forward * CAMERA_LOOK_DISTANCE;
        playerPhysicalEye.LookAt(lookTarget);
        return Vector2.zero;
    }
    #endregion
}