using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PCameraController : MonoBehaviour
{
    public float XMinRotation = -45;
    public float XMaxRotation = 45;
    [Range(1.0f, 10.0f)]
    public float Xsensitivity;
    [Range(1.0f, 10.0f)]
    public float Ysensitivity;
    public float rotAroundX, rotAroundY;
    private CinemachineVirtualCamera virtualCamera;
    public bool firstPerson;
    private Coroutine smootIn;
    public Transform model;
    // Start is called before the first frame update

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    void FixedUpdate()
    {
        if(firstPerson)
        {
            rotAroundX = Mathf.Clamp(rotAroundX, XMinRotation, XMaxRotation);
            transform.parent.rotation = Quaternion.Euler(-rotAroundX, rotAroundY, 0); // rotation of Camera
        }
        model.transform.rotation = transform.parent.rotation;
    }

    public void CameraMovement(InputAction.CallbackContext context)
    {
        if(firstPerson)
        {
            Vector2 mouseInput = context.ReadValue<Vector2>();
            rotAroundX += mouseInput.y*Xsensitivity/100;
            rotAroundY += mouseInput.x*Ysensitivity/100;
        }
    }
    void OnEnable()
    {
        rotAroundY = transform.parent.eulerAngles.y;
        rotAroundX = 0;
        smootIn = StartCoroutine(smoothIn());
    }
    void OnDisable()
    {
        firstPerson = false;
        if(smootIn != null)
            StopCoroutine(smootIn);
    }
    IEnumerator smoothIn()
    {
        float time = 0;
        firstPerson = true;
        while(time < 1)
        {
            time += Time.deltaTime;
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.Euler(-rotAroundX, rotAroundY, 0), time); // rotation of Camera
            yield return null;
        }
    }
}
