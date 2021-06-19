using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    #region enums
    [Serializable]
    public enum MovementType
    {
        LRRotations,
        MouseRotations,
    }
    #endregion

    #region variables
    private const float MIN_DEGREE = 0f;
    private const float MAX_DEGREE = 360f;
    private const float AVG_DEGREE = 180f;
    private const float CAMERA_LOOK_DISTANCE = 5f;
    private const float ANIMATION_SPEED = 1f;
    // an attempt to remove gliding movement, unfortunately the stride length is small
    // for how fast we want the character to move
    private const float WALK_ANIMATION_MULT = 0.6f; // scaling factor for the animator speed

    [Header("Player movement")]
    [SerializeField]
    private float acceleration = 25f;
    [SerializeField]
    private float forwardMoveSpeed = 3f;
    [Tooltip("Ignored when character rotation is not controlled by mouse movement")]
    [SerializeField]
    private float backwardMoveSpeed = 1f;
    [SerializeField]
    private float crouchingMoveSpeed = 1f;
    [Tooltip("Only used when character rotation is not controlled by mouse movement")]
    [SerializeField]
    private float turnSpeed = 30f;

    [Header("Camera rotations")]
    [SerializeField]
    private Transform playerEye = null;
    [SerializeField]
    private Transform playerPhysicalEye = null;
    [SerializeField]
    private Transform lookTarget = null;
    [SerializeField]
    private Transform transposeCamTarget = null;
    [Tooltip("Set to 1f to disable mouse acceleration")]
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float lookSmoother = 0.2f;
    [Tooltip("Set below 1f to speed up rotations (generally for gamepads) or above " +
        "1f to slow down rotations (generally for mouse)")]
    [SerializeField]
    [Range(0.001f, 400f)]
    private float rotationXDivider = 4f;
    [Tooltip("Set below 1f to speed up rotations (generally for gamepads) or above " +
        "1f to slow down rotations (generally for mouse)")]
    [SerializeField]
    [Range(0.001f, 400f)]
    private float rotationYDivider = 4f;
    [Tooltip("Also effects the ability to zoom in / out")]
    [SerializeField]
    [Range(15f, 75f)]
    private float maxYRotation = 70f;

    [Header("Jumping")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private const float airManeuverability = 0.45f;
    [SerializeField]
    private float GroundDistance = 0.2f;
    [SerializeField]
    private LayerMask Ground;
    [SerializeField]
    private Transform groundPosition;

    public bool CaptureInput { get; set; } = true;

    public bool IsSneaking { get { return crouchInput; } }
    private MovementType movementInputType = MovementType.LRRotations;

    public MovementType MovementMode
    {
        get { return movementInputType; }
        set
        {
            movementInputType = value;
            // reset eye rotation on movement mode swap

            playerEye.localEulerAngles = new Vector3(
                0f,
                playerEye.localEulerAngles.y,
                playerEye.localEulerAngles.z);
            playerPhysicalEye.localEulerAngles = new Vector3(
                  0f,
                  playerPhysicalEye.localEulerAngles.z,
                  playerPhysicalEye.localEulerAngles.z);
        }
    }

    private Rigidbody rbody;
    private Animator animator;
    private float xAcc = 0f;
    private float yAcc = 0f;
    private float largeMinYRotation = 0f;
    private Vector2 mouseDelta = Vector2.zero;
    private Vector2 movementInput = Vector2.zero;
    private bool jumpInput;
    private bool isGrounded = false;
    private bool crouchInput = false;


    #endregion

    #region input controlled functions
    public void GetDeltaInput(InputAction.CallbackContext context)
    {
        if (TimeManager.IsGamePaused || !CaptureInput)
        {
            mouseDelta = Vector2.zero;
            return;
        }
        mouseDelta += context.ReadValue<Vector2>();
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        if (TimeManager.IsGamePaused || !CaptureInput)
        {
            movementInput = Vector2.zero;
            return;
        }
        movementInput = context.ReadValue<Vector2>();
    }

    public void JumpInput()
    {
        if (TimeManager.IsGamePaused || !CaptureInput)
        {
            jumpInput = false;
            return;
        }
        jumpInput = true;
    }

    public void CrouchInput()
    {
        if (TimeManager.IsGamePaused || !CaptureInput)
        {
            crouchInput = false;
            return;
        }
        crouchInput = !crouchInput;
    }

    #endregion

    #region monobehaviour fns
    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        largeMinYRotation = MAX_DEGREE - maxYRotation;
    }

    private void Start()
    {

        // handle positions and rotations of these manually
        // when player location's updated
        // ensuring its actual position / rotation are exactly overlapped when necessary
        playerEye.localRotation = Quaternion.identity;
        lookTarget.localRotation = Quaternion.identity;
        transposeCamTarget.localRotation = Quaternion.identity;
        transposeCamTarget.localPosition = Vector3.zero;
        lookTarget.parent = null;
        transposeCamTarget.parent = null;
    }

    private void FixedUpdate()
    {
        MouseRotations();
        Movements();
    }
    #endregion

    #region private fns
    // uses and empties mouseDelta to perform character + camera rotations
    private void MouseRotations()
    {
        DetermineRotationInput(ref xAcc, ref yAcc);
        mouseDelta = Vector2.zero;
    }

    private void DetermineRotationInput(ref float xAcc, ref float yAcc)
    {
        if (mouseDelta.x != 0f)
            xAcc = Mathf.Lerp(xAcc, mouseDelta.x / rotationXDivider, lookSmoother);
        else
            xAcc = 0f;
        if (mouseDelta.y != 0f)
            yAcc = Mathf.Lerp(yAcc, mouseDelta.y / rotationYDivider, lookSmoother);
        else
            yAcc = 0f;
        RotateLocalTransform(xAcc, yAcc);
    }

    private void CommonRotations(float yAcc, out Vector3 pos, out Vector3 pos2)
    {
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
        pos = transform.position + playerEye.forward * CAMERA_LOOK_DISTANCE;
        pos2 = transform.position + transposeCamTarget.forward * CAMERA_LOOK_DISTANCE;
    }

    private void RotateLocalTransform(float xAcc, float yAcc)
    {
        Vector3 pos, pos2;
        switch (movementInputType)
        {
            case MovementType.MouseRotations:
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + xAcc, 0f);
                CommonRotations(yAcc, out pos, out pos2);
                lookTarget.position = pos;
                playerPhysicalEye.LookAt(pos);
                break;
            case MovementType.LRRotations:
                // this is the camera's orbital transpose follow target
                transposeCamTarget.eulerAngles = new Vector3(
                    transposeCamTarget.eulerAngles.x,
                    transposeCamTarget.eulerAngles.y + xAcc,
                    transposeCamTarget.eulerAngles.z);
                if (movementInput.x != 0f || movementInput.y != 0f)
                {
                    var lookRot = Quaternion.LookRotation(new Vector3(
                        movementInput.x, 0f, movementInput.y), transform.up);
                    lookRot *= transposeCamTarget.rotation;
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        lookRot, turnSpeed * Time.fixedDeltaTime);
                }
                CommonRotations(yAcc, out pos, out pos2);
                lookTarget.position = new Vector3(pos2.x, pos.y, pos2.z);
                lookTarget.eulerAngles = new Vector3(
                    playerPhysicalEye.eulerAngles.x,
                    transposeCamTarget.eulerAngles.y,
                    0f);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    // only the character is directly moved, uses rigidbody
    private void Movements()
    {
        // drag should be set above 0 or it will slide and we'll need extra logic
        // for when y == 0f
        DetermineMovementInput(movementInput, out var maxSpeed, out var accel, out var horizAccel);
        // always a frame late
        animator.SetFloat("Speed", maxSpeed);
        animator.speed = ANIMATION_SPEED;
        isGrounded = Physics.CheckSphere(groundPosition.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        AttemptJump();
        if (maxSpeed != 0f)
        {
            // TODO when we have animations for moving left / right, should also move character in those directions
            // alternatively set s/d to turn the character left and right, with mouse controls for the topdown 
            // camera
            accel *= isGrounded ? 1f : airManeuverability;
            horizAccel *= isGrounded ? 1f : airManeuverability;
            rbody.AddForce(transform.forward * accel);
            rbody.AddForce(transform.right * horizAccel);
            var horiz = new Vector2(rbody.velocity.x, rbody.velocity.z);
            
            if (horiz.sqrMagnitude > maxSpeed * maxSpeed)
            {
                horiz = horiz.normalized * maxSpeed;
                rbody.velocity = new Vector3(horiz.x, rbody.velocity.y, horiz.y);
            }
        }
        animator.speed = ANIMATION_SPEED * WALK_ANIMATION_MULT * rbody.velocity.magnitude;
        transposeCamTarget.position = transform.position;
    }

    private void AttemptJump()
    {
        if (!jumpInput)
            return;
        jumpInput = false;
        if (!isGrounded)
            return;
        rbody.velocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);
        rbody.AddForce(transform.up * jumpForce);
        isGrounded = false;
    }

    private void DetermineMovementInput(Vector2 movementInput, out float maxSpeed, out float accel, out float horizAccel)
    {
        switch (movementInputType)
        {
            case MovementType.MouseRotations:
                maxSpeed = forwardMoveSpeed;
                if (crouchInput)
                {
                    maxSpeed = crouchingMoveSpeed;
                }
                else if (movementInput.y < 0f)
                {
                    maxSpeed = backwardMoveSpeed;
                }
                else if (movementInput == Vector2.zero)
                {
                    maxSpeed = 0f;
                }
                accel = acceleration * movementInput.y;
                horizAccel = acceleration * movementInput.x;
                break;
            // want the highest possible value, whether it's the actual magnitude or the x / y
            // we're always moving forward
            case MovementType.LRRotations:
                if (movementInput.x == 0f && movementInput.y == 0f)
                {
                    maxSpeed = 0f;
                }
                else if (crouchInput)
                {
                    maxSpeed = crouchingMoveSpeed;
                }
                else
                {
                    maxSpeed = forwardMoveSpeed;
                }
                horizAccel = 0f;
                accel = acceleration;
                break;
            default:
                throw new NotImplementedException();
        }
    }
    #endregion
}