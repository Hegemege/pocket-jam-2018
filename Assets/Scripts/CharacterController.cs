using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected int _selectedStationIndex; // TODO: Vaihda enumiksi

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected void SetMoveTarget(Vector3 target)
    {
        _agent.SetDestination(target);
    }
}
