using UnityEngine;

public abstract class Interferer : MonoBehaviour
{
    public float interferenceDistance;
    public float forceMagnitude;
    
    // 1 for attraction, -1 for repulsion
    protected int ForceDirection;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var distance = transform.position - other.transform.position;
        var magnitude = distance.magnitude;
        if (magnitude < interferenceDistance)
        {
            var direction = distance.normalized;
            var normal = transform.forward;
            var angle = Vector3.Dot(direction, normal);
            other.GetComponent<Rigidbody>().AddForce(ForceDirection * forceMagnitude * angle / magnitude * direction);
        }
    }
}
