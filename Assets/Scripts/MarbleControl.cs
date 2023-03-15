using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Serialization;

/// <summary>
/// Script for the main game controller.
/// </summary>
public class MarbleControl : MonoBehaviour
{
    [Tooltip("The AR Tracked Image Manager by AR Foundation in the scene.")]
    public ARTrackedImageManager trackedImageManager;
    
    [Tooltip("The Marble Prefab to be loaded at runtime.")]
    public GameObject marblePrefab;
    
    [Tooltip("The mapping from image name to level paths for hidden levels.")]
    [FormerlySerializedAs("levelMap")] 
    public SerializableDictionary<string, string> hiddenLevelMap;

    [Tooltip("The default tracked image name for level select.")]
    public string trackedImageName;
    
    [Tooltip("the list of level paths. These will show up in the level select menu.")]
    public List<string> levels;
    
    [Tooltip("The current level that is being played.")]
    public int currentLevel;

    [Tooltip("The debug game object in the scene.")]
    [SerializeField] private GameObject debug;

    [Tooltip("The offset in the level up direction to the respawn location in the level.")]
    public float respawnHeightOffset;
    
    [Tooltip("The distance between the marble and the level to trigger a respawn.")]
    public float respawnTriggerDistance;
    
    private Checkpoint _restartPoint;
    private GameObject _marble;
    private PlaneBase _plane;
    private TextMeshProUGUI _debugText;
    private ARTrackedImage _currentTrackedImage;
    private bool _arEnabled;
    
    /// <summary>
    /// Selects the level to load given the level index.
    /// </summary>
    /// <param name="l">The level index from <see cref="levels"/>.</param>
    public void SelectLevel(int l)
    {
        if (currentLevel == l || l < 0 || levels.Count <= l) return;
        currentLevel = l;
        LoadLevelAsync(levels[currentLevel], _currentTrackedImage);
    }
    
    /// <summary>
    /// Sets the given checkpoint to active, deactivate the previous one.
    /// </summary>
    /// <param name="checkpoint">The checkpoint to set active.</param>
    /// <returns>Whether a new checkpoint is being set.</returns>
    public bool SetCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == _restartPoint) return false;
        
        _restartPoint.SetButtonColor(false);
        _restartPoint = checkpoint;
        return true;
    }

    /// <summary>
    /// Resets the marble to start position and deactivates all checkpoints.
    /// </summary>
    public void RestartLevel()
    {
        _restartPoint.SetButtonColor(false);
        _restartPoint = GameObject.FindWithTag("Start").GetComponent<Checkpoint>();
        var restartTransform = _restartPoint.transform;
        ResetMarble(restartTransform.position  + GetPlaneUp() * respawnHeightOffset);
    }
    
    /// <summary>
    /// Sets the game to winning state, disables marble.
    /// </summary>
    public void OnGameWin()
    {
        _marble.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Gets the plane up direction.
    /// </summary>
    /// <returns>A vector of the plane up direction.</returns>
    public Vector3 GetPlaneUp()
    {
        return _plane.transform.up;
    }

    /// <summary>
    /// Logs string to the debug output.
    /// </summary>
    /// <param name="s">The string to log.</param>
    public void DebugLog(string s)
    {
        _debugText.text += $"{s}\n";
    }

    /// <summary>
    /// Clears the log of debug output.
    /// </summary>
    public void ClearDebugLog()
    {
        _debugText.text = "Debug text\n\n";
    }

    /// <summary>
    /// Toggles the visibility of the debug output.
    /// </summary>
    public void ToggleDebug()
    {
        debug.SetActive(!debug.activeSelf);
    }

    /// <summary>
    /// Toggles the sound in game.
    /// </summary>
    public void ToggleSound()
    {
        var volume = AudioListener.volume;
        AudioListener.volume = volume == 0 ? 1 : 0;
    }

    /// <summary>
    /// Resets the marble position to the give value, play a sound if this is not an initial set.
    /// </summary>
    /// <param name="pos">The position to set the marble.</param>
    /// <param name="initial">Whether this is the first time setting the marble position.</param>
    private void ResetMarble(Vector3 pos, bool initial = false)
    {
        if (!initial) GetComponent<AudioSource>().Play();
        StartCoroutine(ResetMarbleCoroutine(pos));
    }

    /// <summary>
    /// The coroutine to reset marble. We wait till the next fixed update before resetting.
    /// </summary>
    /// <param name="pos">The position to set the marble.</param>
    /// <returns>The <see cref="IEnumerator"/> for the coroutine.</returns>
    private IEnumerator ResetMarbleCoroutine(Vector3 pos)
    {
        yield return new WaitForFixedUpdate();
        if (!_marble.gameObject.activeSelf) _marble.gameObject.SetActive(true);
        _marble.transform.position = pos;
        _marble.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// Load a level asynchronously, given the level path and the associated <see cref="ARTrackedImage"/>.
    /// Disposes the previous level if there is one.
    /// </summary>
    /// <param name="levelPath">The level path to load the level.</param>
    /// <param name="newImage">The <see cref="ARTrackedImage"/> for the level.</param>
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

    private void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;

    private void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;

    /// <summary>
    /// The main function for handling <see cref="ARTrackedImage"/> changes, loading a level on new images tracked.
    /// </summary>
    /// <param name="eventArgs">Event arguments for the <see cref="ARTrackedImageManager.trackedImagesChanged"/> event.</param>
    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            var name = newImage.referenceImage.name;
            DebugLog($"Detected image {name}");
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

        // I've never seen this being called in practice
        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            DebugLog($"Image removed: {removedImage.referenceImage.name}");
        }
    }
    
    /// <summary>
    /// If the distance between marble and plane is greater than the <see cref="respawnTriggerDistance"/>, reset the marble.
    /// </summary>
    private void FixedUpdate()
    {
        if (_arEnabled)
        {
            if ((_marble.transform.position - _plane.transform.position).magnitude > respawnTriggerDistance)
            {
                var restartTransform = _restartPoint.transform;
                ResetMarble(restartTransform.position + GetPlaneUp() * respawnHeightOffset);
            }
        }
    }
    
    private void Start()
    {
        _debugText = debug.GetComponent<TextMeshProUGUI>();
    }
}
