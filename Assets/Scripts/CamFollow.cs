using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    // Here's where the bot transform goes
    public Transform Target;
    Transform camTransform;
    // offset between the camera and bot. Just set to whatever it is in scene view
    Vector3 Offset;
    // how smooth the camera moves
    public float SmoothTime = 0.3f;

    // start velocity should be zero or all goes wonky
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        camTransform = gameObject.transform;
        Offset = camTransform.position - Target.position;
    }

    private void LateUpdate()
    {
        // update position
        Vector3 targetPosition = Target.position + Offset;
        camTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

        // update rotation
        transform.LookAt(Target);
    }
}