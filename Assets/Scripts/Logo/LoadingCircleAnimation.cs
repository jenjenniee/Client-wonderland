using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircleAnimation : MonoBehaviour
{
    public Animator animator;
    public float cycleOffset;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.SetFloat("CycleOffset", cycleOffset);
        }
        else
        {
            Debug.LogError("Animator component is missing on this game object.");
        }
    }
}
