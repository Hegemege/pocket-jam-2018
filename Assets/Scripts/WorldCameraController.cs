using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraController : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.WorldCamera = this;
    }
}
