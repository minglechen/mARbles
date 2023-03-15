using UnityEngine;

/// <summary>
/// Script for the goal of completing the level.
/// </summary>
public class Goal : MonoBehaviour
{
    private MarbleControl _marbleControl;
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
    }

    /// <summary>
    /// If the player enters the trigger, play a winning sound and tell <see cref="MarbleControl"/> that we won.
    /// </summary>
    /// <param name="other">The other collider that enters the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _marbleControl.OnGameWin();
        _marbleControl.ClearDebugLog();
        _marbleControl.DebugLog("you win!");
    }
}
