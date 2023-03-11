using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneBase : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _previousPosition;
    private bool _startInterpolation;
    public ARTrackedImage TrackedImage
    {
        set
        {
            _trackedImage = value;
            if (_trackedImage)
            {
                transform.position = _trackedImage.transform.position;
                transform.rotation = _trackedImage.transform.rotation;
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
        // Use a rolling average to smooth out movement
        var currentPosition = _trackedImage.transform.position;
        if (!_startInterpolation)
        {
            _startInterpolation = true;
            _previousPosition = currentPosition;
        }
        var avg = (currentPosition + _previousPosition) / 2;
        _previousPosition = currentPosition;
        _rb.Move(avg, _trackedImage.transform.rotation);
    }
}
