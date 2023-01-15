using BlazeAISpace;
using UnityEngine;
using UnityEngine.AI;

public class CrowdAvoidance : MonoBehaviour
{
    // Agent component
    public NavMeshAgent agent;
    // Avoidance Radius
    public float avoidanceRadius = 1.0f;
    // Avoidance force
    public float avoidanceForce = 2.0f;
    // Avoidance Layermask
    public LayerMask avoidanceLayer;
    // Closest point on Navmesh
    private Vector3 closestPoint;
    private BlazeAI _blaze;
    public BlazeAISpace.CoverShooterBehaviour _cover;

    private void Awake()
    {
        _blaze = GetComponent<BlazeAI>();
        _cover = GetComponent<CoverShooterBehaviour>();
    }

    private void Update()
    {
        // Find the closest point on the NavMesh
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, avoidanceRadius, NavMesh.AllAreas);
        closestPoint = hit.position;

        // Find the colliders in the avoidance radius
        Collider[] agents = Physics.OverlapSphere(closestPoint, avoidanceRadius, avoidanceLayer);

        Vector3 avoidanceVector = Vector3.zero;
        // For each nearby agent
        foreach (Collider nearbyAgent in agents)
        {
            if (nearbyAgent != GetComponent<Collider>())
            {
                // Calculate the direction away from the nearby agent
                Vector3 avoidDirection = transform.position - nearbyAgent.transform.position;

                // Add the avoidDirection to the agent's velocity to create a force pushing it away
                avoidanceVector += avoidDirection.normalized;
            }
        }

        // Add avoidance force to the agent's velocity
        agent.velocity += avoidanceVector.normalized * avoidanceForce * Time.deltaTime;
    }

}


