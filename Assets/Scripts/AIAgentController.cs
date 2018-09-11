using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentController : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected int _selectedStationIndex; // TODO: Vaihda enumiksi

    protected SpriteRenderer[] _sprites;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.WorldCamera == null) return;

        var cameraForwardOnZ = Vector3.ProjectOnPlane(GameManager.Instance.WorldCamera.transform.forward, Vector3.up);

        for (var i = 0; i < _sprites.Length; i++)
        {
            _sprites[i].transform.rotation =
                Quaternion.LookRotation(cameraForwardOnZ, Vector3.up);
        }
    }

    public void SetMoveTarget(Vector3 target)
    {
        _agent.SetDestination(target);
    }
}
