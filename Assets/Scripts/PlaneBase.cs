using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneBase : MonoBehaviour
{
    private Rigidbody _rb;

    public ARTrackedImage TrackedImage
    {
        set
        {
            _trackedImage = value;
            if (_trackedImage)
            {
                transform.position = _trackedImage.transform.position;
                transform.rotation = _trackedImage.transform.rotation * Quaternion.Euler(-90, 0, 0);
                isTracking = true;
            }

        }
    }

    private ARTrackedImage _trackedImage;
    private bool isTracking;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (!isTracking) return;
        _rb.Move(_trackedImage.transform.position, _trackedImage.transform.rotation * Quaternion.Euler(-90,0,0));
    }
}
