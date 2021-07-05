using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAi : MonoBehaviour
{
    private Transform Player;
    public float speed;
    public LayerMask GroundLayer, PlayerLayer;
    public bool FlyEscape;
    bool Flying = false;
    Animator BugAnim;
    Rigidbody BugBody;

    public float WalkRange;
    float stopDistance = 0.1f;

    public float SightRange;
    bool PlayerInSight;
    public float PlayerTooFast;
    Animator PlayerAnim;

    List<Vector3> WalkPoints = new List<Vector3>();
    private int currentPoint = 0;

    bool Flee;


    public void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        BugAnim = gameObject.GetComponent<Animator>();
        PlayerAnim = Player.GetComponentInChildren<Animator>();
        BugBody = gameObject.GetComponent<Rigidbody>();

    }


    private void Start()
    {
        Vector3 point = Vector3.zero;

        for (int i = 0; WalkPoints.Count < 5; i++)
        {
            point = NewPosition(WalkRange);

            if (point != Vector3.zero)
                WalkPoints.Add(NewPosition(WalkRange));

        }


    }

    // Update is called once per frame
    void Update()
    {
        //How far has it walked
        float distance = (WalkPoints[currentPoint] - transform.position).magnitude;

        //Is the player closeby?
        PlayerInSight = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);

        //If player is too close, and walks too fast, it runs away
        if (PlayerAnim.GetFloat("Speed") >= PlayerTooFast && PlayerInSight) 
            flee();

        if (distance <= stopDistance)
        {
            //Go to next point
            currentPoint += 1;

            //If we're at the last point, loop
            if (currentPoint >= WalkPoints.Count)
            {
                currentPoint = 0;
            }

        }

        //If player is not around, we stroll
        if (PlayerAnim.GetFloat("Speed") < PlayerTooFast && !PlayerInSight)
            Stroll();

        BugAnim.SetFloat("speed", BugBody.velocity.magnitude);

    }


    private void Stroll()
    {
        Vector3 direction = (WalkPoints[currentPoint] - transform.position).normalized;
        Debug.Log(currentPoint);
        //Turn Y towards target
        Quaternion LookAtRotation = Quaternion.LookRotation(direction);
        Quaternion LookAtTarget = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = LookAtTarget;

        BugBody.AddForce(direction * speed);
    }


    private void flee()
    {
        Debug.Log("HELP!");

        Vector3 direction = (Player.transform.position - transform.position).normalized;

        BugBody.AddForce(direction * speed);
    }

    private void FlyOff()
    {
        //Add some type of jump and then return to a walkpoint

    }


    private Vector3 NewPosition(float range)
    {
        float X = Random.Range(-range, range);
        float Z = Random.Range(-range, range);
        Vector3 Position = new Vector3(transform.position.x + X, transform.position.y, transform.position.z + Z);

        if (Physics.Raycast(Position, -transform.up, 2f, GroundLayer))
        return Position;


        else
            return Vector3.zero;
    }

}

