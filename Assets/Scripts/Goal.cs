using UnityEngine;

public class Goal : MonoBehaviour
{
    private MarbleControl _marbleControl;
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _marbleControl.OnGameWin();
        _marbleControl.ClearDebugLog();
        _marbleControl.DebugLog("you win!");
    }
}
