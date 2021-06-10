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
    [Header("Player movement")]
    [SerializeField]
    private MovementType movementInputType = MovementType.LRRotations;
    [SerializeField]
    private float acceleration = 25f;
    [SerializeField]
    private float forwardMoveSpeed = 3f;
    [Tooltip("Ignored when character rotation is not controlled by mouse movement")]
    [SerializeField]
    private float backwardMoveSpeed = 1;
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
        xAcc = Mathf.Lerp(xAcc, mouseDelta.x / rotationLimiter, lookSmoother);
        yAcc = Mathf.Lerp(yAcc, mouseDelta.y / rotationLimiter, lookSmoother);
        RotateLocalTransform(xAcc);
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

    private void RotateLocalTransform(float xAcc)
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
        DetermineMovementInput(movementInput, out var movespeed, out var accel);
        animator.SetFloat("Speed", movespeed);
        if (movespeed == 0f) return;
        // TODO when we have animations for moving left / right, should also move character in those directions
        // alternatively set s/d to turn the character left and right, with mouse controls for the topdown 
        // camera
        rbody.AddForce(transform.forward * accel);
        if (rbody.velocity.sqrMagnitude > movespeed * movespeed)
        {
            rbody.velocity = rbody.velocity.normalized * movespeed;
        }
        transposeCamTarget.position = transform.position;
    }

    private void DetermineMovementInput(Vector2 movementInput, out float movespeed, out float accel)
    {
        switch (movementInputType)
        {
            case MovementType.MouseRotations:
                if (movementInput.y > 0f)
                {
                    movespeed = forwardMoveSpeed;
                    accel = acceleration;
                }
                else if (movementInput.y < 0f)
                {
                    movespeed = backwardMoveSpeed;
                    accel = -acceleration;
                }
                else
                {
                    accel = acceleration;
                    movespeed = 0f;
                }
                break;
            // want the highest possible value, whether it's the actual magnitude or the x / y
            // we're always moving forward
            case MovementType.LRRotations:
                if (movementInput.x == 0f && movementInput.y == 0f)
                {
                    movespeed = 0f;
                }
                else
                {
                    movespeed = movementInput.magnitude;
                }
                accel = acceleration;
                break;
            default:
                throw new NotImplementedException();
        }
    }
    #endregion
}