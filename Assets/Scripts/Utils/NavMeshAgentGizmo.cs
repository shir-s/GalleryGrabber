using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class NavMeshAgentGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.red;

    void OnDrawGizmos()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;

        Gizmos.color = gizmoColor;

        Vector3 position = transform.position + Vector3.up * agent.baseOffset;

        // Draw cylinder base (top-down circle)
        Gizmos.DrawWireSphere(position, agent.radius);

        // Draw vertical lines to show height
        Vector3 top = position + Vector3.up * agent.height;
        Gizmos.DrawLine(position + Vector3.right * agent.radius, top + Vector3.right * agent.radius);
        Gizmos.DrawLine(position + Vector3.left * agent.radius, top + Vector3.left * agent.radius);
        Gizmos.DrawLine(position + Vector3.forward * agent.radius, top + Vector3.forward * agent.radius);
        Gizmos.DrawLine(position + Vector3.back * agent.radius, top + Vector3.back * agent.radius);

        // Draw top circle
        Gizmos.DrawWireSphere(top, agent.radius);
    }
}