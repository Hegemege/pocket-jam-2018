using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : AIAgentController
{
    private LineRenderer _directionLine;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.PlayerController = this;
        _directionLine = GetComponentInChildren<LineRenderer>();
        _directionLine.enabled = false;
    }

    protected override void ReachedTarget()
    {
        base.ReachedTarget();

        GameManager.Instance.CloseToTarget = true;
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.Instance.PlayerSelectedStation == -1) return;

        _directionLine.enabled = true;
        _directionLine.SetPosition(0, transform.position + Vector3.up * -0.5f);
        _directionLine.SetPosition(1, _target);
    }

    public override void SetMoveTarget(Vector3 target)
    {
        base.SetMoveTarget(target);

        GameManager.Instance.CloseToTarget = false;
    }
}
