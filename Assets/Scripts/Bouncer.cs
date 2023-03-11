using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    private MarbleControl _marbleControl;
    
    // Start is called before the first frame update
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        var direction = rb.velocity.normalized;
        var normal = Vector3.ProjectOnPlane(transform.position - other.transform.position, _marbleControl.GetPlaneUp()).normalized;
        var newDirection = Vector3.Reflect(direction, normal);
        rb.AddForce(newDirection * 10);
    }
}
