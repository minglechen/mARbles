using System;
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
    Marble marblePrefab;
    
    [SerializeField] private GameObject debug;
    [SerializeField] private SerializableDictionary<string, string> levelMap;
    [NonSerialized]
    public TextMeshProUGUI debugText;
    [NonSerialized]
    public GameObject restartPoint;
    private Marble _marble;
    private PlaneBase _plane;

    private string _currentImageName;

    private Camera _camera;
    private bool _arEnabled;

    
    private void Start()
    {
        debugText = debug.GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
    }
    

    private void FixedUpdate()
    {
        if (_arEnabled)
        {
            if ((_marble.transform.position - _plane.transform.position).magnitude > 5)
            {
                ResetMarble(restartPoint.transform.position);
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
            if (!levelMap.Dict.ContainsKey(name)) continue;
            var level_async = Resources.LoadAsync<PlaneBase>(levelMap.Dict[name]);
            if (_arEnabled)
            {
                _arEnabled = false;
                Destroy(_plane.gameObject);
            }
 
            _currentImageName = name;
            
            level_async.completed += op =>
            {
                _plane = Instantiate((PlaneBase)level_async.asset);
                _plane.TrackedImage = newImage;
                if (!_marble)
                {
                    debugText.text += "add marble\n";
                    _marble = Instantiate(marblePrefab);
                    _marble.MarbleControl = this;
                }
                restartPoint = GameObject.FindWithTag("Start");
                ResetMarble(restartPoint.transform.position);
                _arEnabled = true;
            };
        }
    
        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            // m_debug.GetComponent<TextMeshProUGUI>().text += updatedImage.referenceImage.name + ": updated\n";
        }
    
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event

                debugText.text += "remove marble\n";
                Destroy(_marble);
                _arEnabled = false;
        }
    }
}
