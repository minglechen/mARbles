
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
        // Reduce the gravity so that ball falls slower
        Physics.gravity *= 0.1f;
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
            MarbleControl.restartPoint = other.gameObject;
        }
    }
}
