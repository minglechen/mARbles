
using System;
using TMPro;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Reduce the gravity to reduce sensitivity
        Physics.gravity *= 0.5f;
    }
    
    void FixedUpdate()
    {
        //Debug.Log(gyro.gravity);
        // rb.AddForce(transform.position + Vector3.down * 0.1f, ForceMode.Acceleration);
    }
    
}
