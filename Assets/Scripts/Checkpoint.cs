using UnityEngine;

/// <summary>
///  Script for the checkpoint to respawn the marble at.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [Tooltip("The material for when checkpoint is active.")]
    public Material activeMaterial;
    
    [Tooltip("The material for when checkpoint is inactive.")]
    public Material inactiveMaterial;
    
    [Tooltip("The button of the checkpoint.")]
    public GameObject button;
    private MarbleControl _marbleControl;
    private bool _isStart;
    
    /// <summary>
    /// Sets the button color to either active or inactive.
    /// </summary>
    /// <param name="active">The active or inactive color to set.</param>
    public void SetButtonColor(bool active)
    {
        if (_isStart) return;
        var material = active ? activeMaterial : inactiveMaterial;
        button.GetComponent<MeshRenderer>().material = material;
    }
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
        _isStart = CompareTag("Start");
    }
    
    /// <summary>
    /// If the player enters the trigger, set the checkpoint to active and play a sound.
    /// </summary>
    /// <param name="other">The other collider that enters the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_marbleControl.SetCheckpoint(this))
        {
            SetButtonColor(true);
            GetComponent<AudioSource>().Play();
        }
    }
}
