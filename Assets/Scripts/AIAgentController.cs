using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentController : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected int _selectedStationIndex; // TODO: Vaihda enumiksi

    protected SpriteRenderer[] _sprites;

    protected bool _active;
    protected Vector3 _target;
    protected float _deactivateDistance = 5.5f;

    protected Animator _anim;

    protected virtual void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.WorldCamera == null) return;

        // Make sprites align towards camera
        var cameraForwardOnZ = Vector3.ProjectOnPlane(GameManager.Instance.WorldCamera.transform.forward, Vector3.up);
        for (var i = 0; i < _sprites.Length; i++)
        {
            _sprites[i].transform.rotation =
                Quaternion.LookRotation(cameraForwardOnZ, Vector3.up);
        }

        // Deactivate when close enough to target. Enable UI
        var distance = Vector3.Distance(transform.position, _target);
        if (distance < _deactivateDistance && _active)
        {
            ReachedTarget();
            _active = false;
            _agent.isStopped = true;
        }
    }

    protected virtual void ReachedTarget()
    {
        _anim.SetBool("Walk", false);
    }

    public virtual void SetMoveTarget(Vector3 target)
    {
        _anim.SetBool("Walk", true);
        _active = true;
        _target = target;
        _agent.isStopped = false;
        _agent.SetDestination(target);
    }
}
