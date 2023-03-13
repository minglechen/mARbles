using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Material activeMaterial;
    public Material inactiveMaterial;
    public GameObject button;
    private MarbleControl _marbleControl;
    private bool _isStart;

    // Start is called before the first frame update
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
        _isStart = CompareTag("Start");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_marbleControl.SetCheckpoint(this))
        {
            SetButtonColor(true);
            GetComponent<AudioSource>().Play();
        }
    }

    public void SetButtonColor(bool active)
    {
        if (_isStart) return;
        var material = active ? activeMaterial : inactiveMaterial;
        button.GetComponent<MeshRenderer>().material = material;
    }
}
