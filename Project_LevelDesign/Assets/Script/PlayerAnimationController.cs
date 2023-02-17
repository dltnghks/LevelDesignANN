using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnRun()
    {
        OffWalk();
        animator.SetBool("isRun", true);
    }

    public void OffRun()
    {
        animator.SetBool("isRun", false);
    }

    public void OnWalk()
    {
        OffRun();
        animator.SetBool("isWalk", true);
    }
    public void OffWalk()
    {
        animator.SetBool("isWalk", false);
    }
    public void OnIdle()
    {
        OffRun();
        OffWalk();
    }
    public void OnShoot()
    {
        animator.SetTrigger("Shoot");
    }

    public void OnDie()
    {
        animator.SetTrigger("Die");
    }
}
