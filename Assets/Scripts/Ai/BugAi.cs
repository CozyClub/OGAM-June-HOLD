using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public class BugAi : MonoBehaviour
{
    private Transform Player;
    public float TurnSpeed;
    public LayerMask GroundLayer, PlayerLayer;
    public bool FlyEscape;  //Can this bug fly?
    bool Flying = false;
    public float IdleTime; //How long it takes a break before moving again

    Animator BugAnim;
    NavMeshAgent BugAgent;
    Vector3 destination;

    public float WalkRange;
    public float SightRange;

    bool PlayerInSight;
    Animator PlayerAnim;

    List<Vector3> WalkPoints = new List<Vector3>();
    private int currentPoint = 0;
    float timer;
    bool Onbreak = false;
    bool Flee;


    public void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        BugAnim = gameObject.GetComponent<Animator>();
        PlayerAnim = Player.GetComponentInChildren<Animator>();
        BugAgent = gameObject.GetComponent<NavMeshAgent>();

    }


    private void Start()
    {
        Vector3 Wpoint = Vector3.zero;

        for (int i = 0; WalkPoints.Count < 5; i++)
        {
            Wpoint = NewPosition(WalkRange);

            if (Wpoint != Vector3.zero)
            {
                WalkPoints.Add(Wpoint);

            }

        }

        timer = IdleTime;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < WalkPoints.Count; i++)
            Handles.Label(WalkPoints[i], i.ToString());

    }


    // Update is called once per frame
    void Update()
    {

        //Is the player closeby?
        PlayerInSight = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);

        //If player is too close, and walks too fast, it runs away
        if (PlayerAnim.GetBool("sneak")== false && PlayerInSight) 
            flee();



        if (!BugAgent.pathPending)
        {

            if (BugAgent.remainingDistance <= BugAgent.stoppingDistance)
            {

                if (!BugAgent.hasPath || BugAgent.velocity.sqrMagnitude == 0f)
                {

                    //Go to next point
                    currentPoint += 1;

                    //If we're at the last point, loop
                    if (currentPoint >= WalkPoints.Count)
                    {
                        currentPoint = 0;
                    }

                    //TakeBreak;

                    Onbreak = true;


                }
            }
        }


        //If player is not around, we stroll
        if (!PlayerInSight || PlayerAnim.GetBool("sneak") == true && !Onbreak)
            Stroll(WalkPoints[currentPoint]);

        BugAnim.SetFloat("speed", (BugAgent.velocity.sqrMagnitude));

        //BreakTimer
        if(Onbreak)
        {

            timer -= Time.deltaTime;
            Debug.Log(timer);
            if (timer <= 0)
            {
                Debug.Log("Lets go");

                Onbreak = false;
                timer = IdleTime;
            }

        }
    }


    private void Stroll(Vector3 destination)
    {
        this.destination = destination;

        BugAgent.isStopped = Onbreak;
        BugAgent.enabled = !Onbreak;

        Debug.Log(currentPoint);

        //To slow down when closer, but I haven't made a functiuon for that yet
        float distance = Vector3.Distance(destination, transform.position);

        //to move agent more like a charactercontroller
        Vector3 movement = transform.forward * Time.deltaTime * TurnSpeed;

        BugAgent.Move(movement);
        BugAgent.SetDestination(destination);
    }


    private void flee()
    {
        Debug.Log("HELP!");
        //Go faster temporary away from player

    }

    private void FlyOff()
    {
        //Add some type of jump with slow return to gravity, and then return to a walkpoint

    }

    private Vector3 NewPosition(float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, range, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;

    }

}

