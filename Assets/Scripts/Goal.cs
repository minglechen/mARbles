using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private MarbleControl _marbleControl;
    // Start is called before the first frame update
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _marbleControl.OnGameWin();
        _marbleControl.ClearDebugLog();
        _marbleControl.DebugLog("you win!");
    }
}
