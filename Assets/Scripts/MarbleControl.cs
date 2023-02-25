using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MarbleControl : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    [SerializeField]
    GameObject m_marble_prefab;
    
    [SerializeField] private GameObject m_debug;

    private GameObject marble;
    private GameObject plane;
    private TextMeshProUGUI debug_text;
    private Camera _camera;
    private bool ar_enabled = false;
    private void Start()
    {
        debug_text = m_debug.GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
    }
    

    private void FixedUpdate()
    {
        var diff = Quaternion.Inverse(Input.gyro.attitude) * _camera.transform.localRotation;
        debug_text.text = diff.eulerAngles.ToString();
        if (ar_enabled)
        {
            if ((marble.transform.position - plane.transform.position).magnitude > 5)
            {
                marble.transform.position = plane.transform.position + plane.transform.up * 0.01f;
                marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;
    
    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            debug_text.text += newImage.referenceImage.name;
            if (newImage.referenceImage.name == "squid")
            {
                m_debug.GetComponent<TextMeshProUGUI>().text += " add marble\n";
                marble = Instantiate(m_marble_prefab);
                marble.transform.position = newImage.gameObject.transform.position;
                plane = newImage.gameObject;
                ar_enabled = true;
            }
        }
    
        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
        }
    
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            if (removedImage.referenceImage.name == "squid")
            {
                m_debug.GetComponent<TextMeshProUGUI>().text += " remove marble\n";
                Destroy(marble);
                ar_enabled = false;
            }
        }
    }
}
