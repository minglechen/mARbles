using UnityEngine;

public class Bouncer : MonoBehaviour
{
    private MarbleControl _marbleControl;
    private Animator _bouncerAnimator;

    public float bounceMagnitude;
    void Start()
    {
        _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
        _bouncerAnimator = GetComponent<Animator>();
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        var normal = Vector3.ProjectOnPlane(other.transform.position - transform.position, _marbleControl.GetPlaneUp()).normalized;
        rb.AddForce(normal * bounceMagnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        GetComponent<AudioSource>().Play();
        _bouncerAnimator.SetTrigger("Bounced");
    }
}
