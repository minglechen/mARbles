
using System;
using TMPro;
using UnityEngine;

public class marble : MonoBehaviour
{
    private Rigidbody rb;
    private Gyroscope gyro;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gyro = Input.gyro;
        gyro.enabled = true;
        // Reduce the gravity so that ball falls slower
        Physics.gravity *= 0.1f;
    }
    
    void FixedUpdate()
    {
        //Debug.Log(gyro.gravity);
        // rb.AddForce(transform.position + Vector3.down * 0.1f, ForceMode.Acceleration);
    }
}
