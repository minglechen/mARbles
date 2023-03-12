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

    public float respawnHeightOffset;
    public float respawnTriggerDistance;
    private Marble _marble;
    private PlaneBase _plane;

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
            if ((_marble.transform.position - _plane.transform.position).magnitude > respawnTriggerDistance)
            {
                var restartTransform = restartPoint.transform;
                ResetMarble(restartTransform.position + restartTransform.up * respawnHeightOffset);
            }
        }
    }

    public void ResetMarble(Vector3 pos)
    {
        GetComponent<AudioSource>().Play();
        _marble.transform.position = pos;
        _marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public Vector3 GetPlaneUp()
    {
        return _plane.transform.up;
    }

    private void LoadLevelAsync(string levelPath, ARTrackedImage newImage)
    {
        if (_arEnabled)
        {
            _arEnabled = false;
            // Maybe add an object pool?
            Destroy(_plane.gameObject);
        }

        var level_async = Resources.LoadAsync<PlaneBase>(levelPath);
        level_async.completed += op =>
        {
            _plane = Instantiate((PlaneBase)level_async.asset);
            _plane.TrackedImage = newImage;
            if (!_marble)
            {
                debugText.text += "add marble\n";
                _marble = Instantiate(marblePrefab);
            }
            restartPoint = GameObject.FindWithTag("Start");
            ResetMarble(restartPoint.transform.position);
            _arEnabled = true;
        };
    }
    
    void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;
    
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;
    
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            var name = newImage.referenceImage.name;
            debugText.text += $"{name}\n";
            if (!levelMap.Dict.ContainsKey(name)) continue;
            debugText.text += "load level";
            LoadLevelAsync(levelMap.Dict[name], newImage);
            return;
        }
    
        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            // m_debug.GetComponent<TextMeshProUGUI>().text += updatedImage.referenceImage.name + ": updated\n";
            // var name = updatedImage.referenceImage.name;
            // if (name == _currentImageName) continue;
            // debugText.text += name;
            // if (!levelMap.Dict.ContainsKey(name)) continue;
            // LoadLevelAsync(levelMap.Dict[name], name, updatedImage);
        }
    
        // I've never seen this being called in practice
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event

                debugText.text += "remove marble\n";
                Destroy(_marble);
                _arEnabled = false;
        }
    }
}
