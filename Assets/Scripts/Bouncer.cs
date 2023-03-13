using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    private MarbleControl _marbleControl;
    private Animator _bouncerAnimator;

    public float bounceMagnitude;
    // Start is called before the first frame update
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
        _bouncerAnimator = GetComponent<Animator>();
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        var direction = rb.velocity.normalized;
        var normal = Vector3.ProjectOnPlane(other.transform.position - transform.position, _marbleControl.GetPlaneUp()).normalized;
        // var newDirection = Vector3.Reflect(direction, normal);
        rb.AddForce(normal * bounceMagnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _bouncerAnimator.SetTrigger("Bounced");
    }
}
