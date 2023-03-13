using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Collections;
using UnityEngine.Serialization;

public class MarbleControl : MonoBehaviour
{
    
    public ARTrackedImageManager trackedImageManager;
    public Marble marblePrefab;
    
    [FormerlySerializedAs("levelMap")] 
    public SerializableDictionary<string, string> hiddenLevelMap;

    public string trackedImageName;
    public List<string> levels;
    public int currentLevel;

    [SerializeField] private GameObject debug;

    public float respawnHeightOffset;
    public float respawnTriggerDistance;
    
    private Checkpoint _restartPoint;
    private Marble _marble;
    private PlaneBase _plane;
    private TextMeshProUGUI _debugText;
    private ARTrackedImage _currentTrackedImage;
    private bool _arEnabled;

    
    private void Start()
    {
        _debugText = debug.GetComponent<TextMeshProUGUI>();
    }
    

    private void FixedUpdate()
    {
        if (_arEnabled)
        {
            if ((_marble.transform.position - _plane.transform.position).magnitude > respawnTriggerDistance)
            {
                var restartTransform = _restartPoint.transform;
                ResetMarble(restartTransform.position + restartTransform.up * respawnHeightOffset);
            }
        }
    }

    public void SelectLevel(int l)
    {
        if (currentLevel == l || l < 0 || levels.Count <= l) return;
        currentLevel = l;
        LoadLevelAsync(levels[currentLevel], _currentTrackedImage);
    }
    public bool SetCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == _restartPoint) return false;
        
        _restartPoint.SetButtonColor(false);
        _restartPoint = checkpoint;
        return true;
    }

    public void RestartLevel()
    {
        _restartPoint.SetButtonColor(false);
        _restartPoint = GameObject.FindWithTag("Start").GetComponent<Checkpoint>();
        var restartTransform = _restartPoint.transform;
        ResetMarble(restartTransform.position  + restartTransform.up * respawnHeightOffset);
    }
    
    public void ResetMarble(Vector3 pos, bool initial = false)
    {
        if (!initial) GetComponent<AudioSource>().Play();
        StartCoroutine(ResetMarbleCoroutine(pos));
    }

    IEnumerator ResetMarbleCoroutine(Vector3 pos)
    {
        yield return new WaitForFixedUpdate();
        if (!_marble.gameObject.activeSelf) _marble.gameObject.SetActive(true);
        _marble.transform.position = pos;
        _marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void OnGameWin()
    {
        _marble.gameObject.SetActive(false);
    }
    public Vector3 GetPlaneUp()
    {
        return _plane.transform.up;
    }

    public void DebugLog(string s)
    {
        _debugText.text += $"{s}\n";
    }

    public void ClearDebugLog()
    {
        _debugText.text = "Debug text\n\n";
    }

    public void ToggleDebug()
    {
        debug.SetActive(!debug.activeSelf);
    }

    public void ToggleSound()
    {
        var volume = AudioListener.volume;
        AudioListener.volume = volume == 0 ? 1 : 0;
    }

    private void LoadLevelAsync(string levelPath, ARTrackedImage newImage)
    {
        _currentTrackedImage = newImage;
        if (_arEnabled)
        {
            _arEnabled = false;
            Destroy(_plane.gameObject);
        }

        var levelAsync = Resources.LoadAsync<PlaneBase>(levelPath);
        levelAsync.completed += op =>
        {
            _plane = Instantiate((PlaneBase)levelAsync.asset);
            _plane.TrackedImage = newImage;
            if (!_marble)
            {
                DebugLog("Add marble");
                _marble = Instantiate(marblePrefab);
            }
            _restartPoint = GameObject.FindWithTag("Start").GetComponent<Checkpoint>();
            var restartTransform = _restartPoint.transform;
            ResetMarble(restartTransform.position  + restartTransform.up * respawnHeightOffset, true);
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
            DebugLog($"Detected image {name} for the first time");
            if (name == trackedImageName)
            {
                LoadLevelAsync(levels[currentLevel], newImage);
                return;
            }
            if (!hiddenLevelMap.Dict.ContainsKey(name)) continue;
            var levelPath = hiddenLevelMap.Dict[name];
            DebugLog($"Load level {levelPath}");
            LoadLevelAsync(levelPath, newImage);
            return;
        }
    
        foreach (var updatedImage in eventArgs.updated)
        {
        }
    
        // I've never seen this being called in practice
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            DebugLog($"Image removed: {removedImage.referenceImage.name}");
        }
    }
}
