using UnityEngine;

/// <summary>
/// Script for the bouncer to reflect the marble on hit.
/// </summary>
public class Bouncer : MonoBehaviour
{
    [Tooltip("The bouncer return force magnitude on hit.")]
    public float bounceMagnitude;
    private MarbleControl _marbleControl;
    private Animator _bouncerAnimator;
    
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
        _bouncerAnimator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// If the player is in the trigger zone, apply a force to the marble, in the direction of the normal.
    /// </summary>
    /// <param name="other">The other collider that is in the trigger.</param>
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        // A hack to get the normal vector, assuming the bouncer is a cylinder.
        var normal = Vector3.ProjectOnPlane(other.transform.position - transform.position, _marbleControl.GetPlaneUp()).normalized;
        rb.AddForce(normal * bounceMagnitude);
    }

    /// <summary>
    /// If the player enters the trigger, play a sound and play the animation.
    /// </summary>
    /// <param name="other">The other collider that enters the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _bouncerAnimator.SetTrigger("Bounced");
    }
}
