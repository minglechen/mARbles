using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Collections;
using UnityEngine.Serialization;

public class MarbleControl : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager trackedImageManager;
    [SerializeField]
    GameObject marblePrefab;
    
    [SerializeField] private GameObject debug;
    [SerializeField] private SerializableDictionary<string, string> levelMap;

    private GameObject _marble;
    private PlaneBase _plane;
    private TextMeshProUGUI _debugText;
    private Camera _camera;
    private bool _arEnabled = false;
    
    private void Start()
    {
        _debugText = debug.GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
    }
    

    private void FixedUpdate()
    {
        if (_arEnabled)
        {
            if ((_marble.transform.position - _plane.transform.position).magnitude > 5)
            {
                ResetMarble(GameObject.FindWithTag("Start").transform.position);
            }
        }
    }

    public void ResetMarble(Vector3 pos)
    {
        _marble.transform.position = pos;
        _marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;
    
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;
    
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            var name = newImage.referenceImage.name;
            if (levelMap.Dict.ContainsKey(name))
            {
                var level_async = Resources.LoadAsync<PlaneBase>(levelMap.Dict[name]);
                level_async.completed += op =>
                {
                    _plane = Instantiate((PlaneBase)level_async.asset);
                    _plane.TrackedImage = newImage;
                    _debugText.text += "add marble\n";
                    _marble = Instantiate(marblePrefab);
                    ResetMarble(GameObject.FindWithTag("Start").transform.position);
                    _arEnabled = true;
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

                _debugText.text += "remove marble\n";
                Destroy(_marble);
                _arEnabled = false;
        }
    }
}
