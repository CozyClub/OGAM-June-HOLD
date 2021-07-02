using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 inputVec;
    private bool jump;
    private bool crouch;
    public Transform forwardRef;

    [Header("Player Parameters")]
    public float speed;
    public float jumpSpeed;
    public float smoothFactor;
    public float smoothRotation;
    public AnimationCurve acceleration;
    private float timeWalk;

    [Header("Ground Detection")]
    public int groundRes;
    bool inGround;
    public LayerMask groundMask;
    float maxGroundDist = 1.3f;
    float playerRadious = .5f;
    Vector3 groundNormal;

    public bool show;
    private PModel modelAnim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        modelAnim = GetComponentInChildren<PModel>();
    }
    void FixedUpdate()
    {
        RotatePlayer();
        CheckMovement();
        Jump();
    }
    void RotatePlayer()
    {
        groundNormal = DetectGround();
        Vector3 transFor = Vector3.ProjectOnPlane(forwardRef.forward, groundNormal);
        Vector3 direction = transFor*inputVec.y+forwardRef.right*inputVec.x;
        if(inGround)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.FromToRotation(transform.forward, direction.normalized*speed)*transform.rotation, 
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
            trueForward*speed*inputClamped.magnitude*acceleration.Evaluate(timeWalk), 
            smoothFactor*Time.fixedDeltaTime);
        if(inGround)        
            rb.velocity = lerpedMovement+rb.velocity.y*Vector3.up;   
        
    }
    void Jump()
    {
        if(inGround && jump)
        {
            jump = false;
            rb.velocity =  new Vector3(rb.velocity.x, 0, rb.velocity.z)+groundNormal*jumpSpeed;
        }
        else
            jump = false;
    }

    Vector3 DetectGround()
    {
        Vector3 residual = Vector3.zero;
        Vector3 rotation = Vector3.forward*playerRadious;
        int found = 0;
        RaycastHit hit;
        for(int i = 0; i < groundRes; i++)
        {
            if(Physics.Raycast(transform.position+rotation, Vector3.down, out hit, maxGroundDist, groundMask))
            {
                residual = hit.normal;
                found += 1;
                if(show)
                    Debug.DrawRay(transform.position+rotation, -hit.normal, Color.red);
            }
            rotation = Quaternion.Euler(0,360/groundRes,0)*rotation;
        }
        inGround = (found >= 0.4*groundRes);    
        if(!inGround)
            residual = Vector3.up;
        else
            residual = residual/found;
        return residual;
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
}
