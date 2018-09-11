using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraController : MonoBehaviour
{
    private Vector3 _initialPosition;

    void Awake()
    {
        GameManager.Instance.WorldCamera = this;
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (!GameManager.Instance.PlayerController) return;

        transform.position = _initialPosition + GameManager.Instance.PlayerController.transform.position;
    }
}
