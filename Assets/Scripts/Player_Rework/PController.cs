using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum playerState{Walk, Sneak, Idle}
public class PController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 inputVec;
    private bool jump;
    private bool crouch;
    public Transform forwardRef;
    public playerState state = playerState.Idle;

    [Header("Player Parameters")]
    public float normalSpeed = 1.8f;
    public float crouchSpeed;
    public float smoothFactor;
    public float smoothRotation;
    public AnimationCurve acceleration;
    private float timeWalk;
    private PAnimatorControl animatorControl;
    private bool canMove;
    private float desiredSpeed;

    [Header("Jump Control")]
    public float delayJump;
    private Coroutine lastCall;
    public float jumpSpeed;

    [Header("Ground Detection")]
    public int groundRes;
    public bool inGround;
    public LayerMask groundMask;
    float maxGroundDist = .58f;
    float playerRadious = .5f;
    Vector3 groundNormal;
    public float extraGravity;

    public bool show;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animatorControl = GetComponent<PAnimatorControl>();
        canMove = true;
    }
    void FixedUpdate()
    {
        SwitchStateMachine();
        RotatePlayer();
        if(canMove)
            CheckMovement();
        Jump();
        animatorControl.UpdateStates(rb.velocity, crouch, state, desiredSpeed);
    }
    void SwitchStateMachine()
    {
        if(crouch && inGround)
        {
            state = playerState.Sneak;
            desiredSpeed = crouchSpeed;
        }
        else if(rb.velocity.sqrMagnitude != 0 && inGround)
        {
            state = playerState.Walk;
            desiredSpeed = normalSpeed;
        }
        else
        {
            state = playerState.Idle;
            desiredSpeed = 1;
        }
    }
    void RotatePlayer()
    {
        groundNormal = DetectGround();
        Debug.DrawRay(transform.position, groundNormal*2, Color.green);
        Vector3 transFor = Vector3.ProjectOnPlane(forwardRef.forward, groundNormal).normalized;
        Vector3 transRig = Vector3.ProjectOnPlane(forwardRef.right, groundNormal).normalized;
        Vector3 direction = transFor*inputVec.y+transRig*inputVec.x;
        Debug.DrawRay(transform.position, direction*2, Color.blue);
        if(inGround)
        {
            Vector3 real;
            if(direction.sqrMagnitude > 0)
                real = direction.normalized;
            else
                real = transform.forward;
            
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(real, groundNormal),                
                smoothRotation*Time.fixedDeltaTime);
        }
        else
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.velocity, Vector3.up).normalized, Vector3.up),                
                smoothRotation*Time.fixedDeltaTime);

    }
    void CheckMovement()
    {
        Vector3 trueForward = -Vector3.Cross(groundNormal, transform.right).normalized;
        Vector3 inputClamped = Vector3.ClampMagnitude(inputVec, 1);
        if(inputClamped.sqrMagnitude < 0.1f)
            timeWalk = 0;
        else
            timeWalk += Time.fixedDeltaTime;
        Vector3 lerpedMovement = Vector3.Lerp(rb.velocity-rb.velocity.y*Vector3.up, 
            trueForward*desiredSpeed*inputClamped.magnitude*acceleration.Evaluate(timeWalk), 
            smoothFactor*Time.fixedDeltaTime);
        if(inGround)        
            rb.velocity = lerpedMovement+rb.velocity.y*Vector3.up;   
    }
    void Jump()
    {
        if(inGround && jump)
        {
            jump = false;
            animatorControl.Jump();
            if(lastCall != null)
                StopCoroutine(lastCall);
            lastCall = StartCoroutine(jumpCoroutine());
        }
        else
            jump = false;

        if(rb.velocity.y < 0.2f && !inGround)
            rb.velocity += Vector3.down*extraGravity;
    }

    Vector3 DetectGround()
    {
        Vector3 residual = Vector3.zero;
        Vector3 rotation = Vector3.forward*playerRadious;
        int found = 0;
        RaycastHit hit;
        for(int i = 0; i < groundRes; i++)
        {
            if(Physics.Raycast(transform.position+rotation, Vector3.down*maxGroundDist, out hit, maxGroundDist, groundMask))
            {
                residual = hit.normal;
                found += 1;
                if(show)
                    Debug.DrawRay(transform.position+rotation, -Vector3.up*maxGroundDist, Color.red);
            }
            rotation = Quaternion.Euler(0,360/groundRes,0)*rotation;
        }
        inGround = (found >= 0.4*groundRes);    
        if(!inGround)
            residual = Vector3.up;
        else
            residual = residual/found;
        return residual.normalized;
    }
    public void Move(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
            jump = true;
    }
    
    public void Crouch(InputAction.CallbackContext context)
    {
        if(context.performed)
            crouch = !crouch;
    }
    IEnumerator jumpCoroutine()
    {
        canMove = false;
        yield return new WaitForSeconds(delayJump);
        rb.velocity =  new Vector3(rb.velocity.x, 0, rb.velocity.z)+groundNormal*jumpSpeed;
        canMove = true;
    }
}