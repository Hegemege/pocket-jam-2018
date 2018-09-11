using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardController : AIAgentController
{
    public StockType PreferredStockType;

    protected override void Awake()
    {
        base.Awake();

        Animator anim = GetComponentInChildren<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    protected override void ReachedTarget()
    {
        // TODO: update station lizard count?
    }
}
