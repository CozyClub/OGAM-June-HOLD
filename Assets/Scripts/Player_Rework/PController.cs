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

    [Header("Ground Detection")]
    public int groundRes;
    public bool inGround;
    public LayerMask groundMask;
    public float maxGroundDist = 1.3f;
    public float playerRadious = .5f;

    public bool show;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 groundNormal = DetectGround();
        Vector3 transFor = Vector3.ProjectOnPlane(forwardRef.forward, groundNormal);
        Vector3 direction = transFor*inputVec.y+forwardRef.right*inputVec.x;
        rb.velocity = direction.normalized*speed+rb.velocity.y*Vector3.up;
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
        return residual/found;
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
