using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerScript : MonoBehaviour
{
    public Animator animator;

    public string animationString;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger(animationString);
        }
    }
}
