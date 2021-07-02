using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PModel : MonoBehaviour
{
    public float smoothRotation;
    public void UpdateRotation(Vector3 direction)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, 
        Quaternion.FromToRotation(transform.forward, direction)*transform.rotation, 
        smoothRotation*Time.deltaTime);
    }
}
