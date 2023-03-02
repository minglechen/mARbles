using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Collections;

public class MarbleControl : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    [SerializeField]
    GameObject m_marble_prefab;
    
    [SerializeField] private GameObject m_debug;
    [SerializeField] private SerializableDictionary<string, string> level_map;

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
        if (ar_enabled)
        {
            if ((marble.transform.position - plane.transform.position).magnitude > 5)
            {
                ResetMarble(plane.transform.position + plane.transform.up * marble.transform.localScale.x / 2);
            }
        }
    }

    public void ResetMarble(Vector3 pos)
    {
        marble.transform.position = pos;
        marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;
    
    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            var name = newImage.referenceImage.name;
            if (level_map.Dict.ContainsKey(name))
            {
                var level_async = Resources.LoadAsync<GameObject>(level_map.Dict[name]);
                level_async.completed += op =>
                {
                    plane = Instantiate((GameObject) level_async.asset);
                    plane.transform.position = newImage.transform.position;
                    plane.transform.rotation = newImage.transform.rotation;
                    plane.transform.parent = newImage.transform;
                    debug_text.text += "add marble\n";
                    marble = Instantiate(m_marble_prefab);
                    marble.transform.position = newImage.gameObject.transform.position;
                    ar_enabled = true;
                };

            }
        }
    
        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            // m_debug.GetComponent<TextMeshProUGUI>().text += updatedImage.referenceImage.name + ": updated\n";
        }
    
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event

                debug_text.text += "remove marble\n";
                Destroy(marble);
                ar_enabled = false;
        }
    }
}
