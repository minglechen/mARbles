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
            if (!_trackedImage) return;
            var trackedTransform = _trackedImage.transform;
            transform.position = trackedTransform.position;
            transform.rotation = trackedTransform.rotation;
            _isTracking = true;

        }
    }

    private ARTrackedImage _trackedImage;
    private bool _isTracking;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (!_isTracking) return;
        // Use a rolling average to smooth out movement
        var trackedTransform = _trackedImage.transform;
        var currentPosition = trackedTransform.position;
        if (!_startInterpolation)
        {
            _startInterpolation = true;
            _previousPosition = currentPosition;
        }
        var avgPos = (currentPosition + _previousPosition) / 2;
        _previousPosition = currentPosition;
        _rb.Move(avgPos, _trackedImage.transform.rotation);
    }
}
