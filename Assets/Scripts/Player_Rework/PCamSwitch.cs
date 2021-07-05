using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class PCamSwitch : MonoBehaviour
{
    public bool cameraEnabled;
    public CinemachineVirtualCamera firstPerson;
    public CinemachineFreeLook thirdPerson;
    public Transform head;
    private Coroutine moveCor;
    public bool hideMouse;
    void Start()
    {
        if(hideMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void OpenCam(InputAction.CallbackContext context)
    {
        cameraEnabled = true;
        firstPerson.gameObject.SetActive(true);
        thirdPerson.gameObject.SetActive(false);
        if(moveCor != null)
            StopCoroutine(moveCor);
    }
    public void CloseCam(InputAction.CallbackContext context)
    {
        cameraEnabled = false;
        thirdPerson.gameObject.SetActive(true);
        firstPerson.gameObject.SetActive(false);
        if(moveCor != null)
            StopCoroutine(moveCor);
        moveCor = StartCoroutine(smoothExit());
    }
    IEnumerator smoothExit()
    {
        float time = 0;
        yield return new WaitForSeconds(.25f);
        while(time < 2)
        {
            time += Time.deltaTime;
            head.transform.localRotation = Quaternion.Slerp(head.transform.localRotation, Quaternion.identity, time/2);
            firstPerson.transform.parent.localRotation = Quaternion.Slerp(firstPerson.transform.parent.localRotation, Quaternion.identity, time/4);
            yield return null;
        }
    }
}
