using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : AIAgentController
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.PlayerController = this;
    }

    protected override void ReachedTarget()
    {
        base.ReachedTarget();

        GameManager.Instance.CloseToTarget = true;
    }

    protected override void Update()
    {
        base.Update();

        var distance = Vector3.Distance(transform.position, _target);
    }

    public override void SetMoveTarget(Vector3 target)
    {
        base.SetMoveTarget(target);

        GameManager.Instance.CloseToTarget = false;
    }
}
