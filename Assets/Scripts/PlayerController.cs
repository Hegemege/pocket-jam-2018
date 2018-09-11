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
        GameManager.Instance.CloseToTarget = true;
    }
}
