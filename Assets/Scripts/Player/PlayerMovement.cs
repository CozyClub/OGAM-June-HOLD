using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    #region variables
    private const float MIN_DEGREE = 0f;
    private const float MAX_DEGREE = 360f;
    private const float AVG_DEGREE = 180f;
    private const float CAMERA_LOOK_DISTANCE = 5f;
    [Header("Player movement")]
    [SerializeField]
    private float acceleration = 25f;
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

    private Rigidbody rbody;
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
        rbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        largeMinYRotation = MAX_DEGREE - maxYRotation;
    }

    private void Start()
    {
        playerEye.localEulerAngles = Vector3.zero;
        lookTarget.parent = null;
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Speed", movementInput.y);

        mouseDelta = Rotations(mouseDelta);

        Movements();
    }
    #endregion

    #region private fns
    // uses and empties mouseDelta to perform character + camera rotations
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

    // only the character is directly moved, uses rigidbody
    private void Movements()
    {
        // drag should be set above 0 or it will slide and we'll need extra logic
        // for when y == 0f
        if (movementInput.y == 0f) return;
        float movespeed;
        float accel;
        if (movementInput.y > 0)
        {
            accel = acceleration;
            movespeed = forwardMoveSpeed;
        }
        else
        {
            accel = -acceleration;
            movespeed = backwardMoveSpeed;
        }
        // TODO when we have animations for moving left / right, should also move character in those directions
        // alternatively set s/d to turn the character left and right, with mouse controls for the topdown 
        // camera
        rbody.AddForce(transform.forward * accel);
        if (rbody.velocity.sqrMagnitude > movespeed * movespeed)
        {
            rbody.velocity = rbody.velocity.normalized * movespeed;
        }
    }
    #endregion
}