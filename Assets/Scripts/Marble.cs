
using System;
using TMPro;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private Rigidbody rb;
    private MarbleControl _marbleControl;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
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
            _marbleControl.debugText.text = "You win!";
        } 
        else if (other.CompareTag("Respawn"))
        {
            // MarbleControl.debugText.text += "Set respawn point\n";
            _marbleControl.restartPoint = other.gameObject;
        }

    }
}
