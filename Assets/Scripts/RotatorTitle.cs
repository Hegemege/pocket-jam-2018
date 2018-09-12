using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTitle : MonoBehaviour {

    // Use this for initialization



    // User Inputs
    public float speed;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    void Start () {
        // Store the starting position & rotation of the object
        posOffset = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right, 45 * Time.deltaTime * speed);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;

    }
}
