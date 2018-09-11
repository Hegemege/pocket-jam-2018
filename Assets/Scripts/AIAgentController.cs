using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentController : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected int _selectedStationIndex; // TODO: Vaihda enumiksi

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetMoveTarget(Vector3 target)
    {
        _agent.SetDestination(target);
    }
}
