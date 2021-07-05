using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPaused : MonoBehaviour
{
    private Animator animat;

    private void Awake()
    {
        animat = GetComponent<Animator>();
    }

    public void StopPoint()
    {
        animat.speed = 0f;
    }
}
