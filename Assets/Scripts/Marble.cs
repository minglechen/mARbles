
using System;
using TMPro;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private Rigidbody rb;
    public MarbleControl MarbleControl;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            // show something to the player
            MarbleControl.debugText.text = "You win!";
        } 
        else if (other.CompareTag("Respawn"))
        {
            MarbleControl.debugText.text += "Set respawn point\n";
            MarbleControl.restartPoint = other.gameObject;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bouncer"))
        {
            Debug.Log("bouncer trigger stay");
            var direction = rb.velocity.normalized;
            var normal = Vector3.ProjectOnPlane(transform.position - other.transform.position, MarbleControl.GetPlaneUp()).normalized;
            var newDirection = Vector3.Reflect(direction, normal);
            rb.AddForce(newDirection * 5);
        }
    }
}
