using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        if (collision.gameObject.CompareTag("Player"))
        {
            // MarbleControl.debugText.text += "on collision enter\n";
            Debug.Log("bouncer on collision enter");

        }
    }
}
