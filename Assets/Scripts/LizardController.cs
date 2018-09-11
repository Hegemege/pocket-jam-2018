using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardController : AIAgentController
{
    //public StockType PreferredStockType;

    public GameObject SpriteContainer;

    public float IdleAtTargetSeconds;
    public float IdleAtTargetSecondsRandomness;

    public float BaseScale;
    public float ScaleRandomness;
    private Vector3 _randomizedScale;

    private float _idleTimer;
    private bool _atTarget;

    protected override void Awake()
    {
        base.Awake();

        GetNewIdleWaitTime();

        Animator anim = GetComponentInChildren<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));

        StartCoroutine(Idle(0.5f + Random.Range(-0.25f, 0.25f))); // Some initial pause

        _randomizedScale = Random.Range(BaseScale - ScaleRandomness, BaseScale + ScaleRandomness) * Vector3.one;
        SpriteContainer.transform.localScale = _randomizedScale;
    }

    protected override void ReachedTarget()
    {
        base.ReachedTarget();

        if (_atTarget) return;

        _atTarget = true;
        StartCoroutine(Idle(_idleTimer));
    }

    private IEnumerator Idle(float idleDuration)
    {
        yield return new WaitForSeconds(idleDuration);
        GetNewIdleWaitTime();

        GetNewTarget();
    }

    private void GetNewTarget()
    {
        // Go to one of the top 3 most volatile stocks
        _atTarget = false;
        var stocks = new List<Stock>();
        stocks.AddRange(StockManager.Instance.Stocks);
        stocks.Sort((a, b) => a.Volatility.CompareTo(b.Volatility));
        stocks.Reverse();

        var randomStock = stocks[Random.Range(0, 3)];
        var targetPosition = GameManager.Instance.GetStockStationPosition(randomStock.Name);
        SetMoveTarget(targetPosition);
    }

    private void GetNewIdleWaitTime()
    {
        _idleTimer = IdleAtTargetSeconds + Random.Range(-IdleAtTargetSecondsRandomness, IdleAtTargetSecondsRandomness);
    }
}
