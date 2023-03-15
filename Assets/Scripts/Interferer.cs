using UnityEngine;

/// <summary>
/// The base class for objects that can interfere the marble.
/// </summary>
public abstract class Interferer : MonoBehaviour
{
    [Tooltip("The maximum distance that the interferer will affect the marble.")]
    public float interferenceDistance;
    
    [Tooltip("The magnitude of the force to exert on the marble.")]
    public float forceMagnitude;
    
    /// <summary>
    /// The force direction to exert.
    /// </summary>
    /// <remarks>
    /// 1 for attraction, -1 for repulsion
    /// </remarks>
    protected int ForceDirection;

    /// <summary>
    /// If the player stays in the trigger, apply a force to it.
    /// </summary>
    /// <param name="other">The other collider that stayed in the trigger.</param>
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
