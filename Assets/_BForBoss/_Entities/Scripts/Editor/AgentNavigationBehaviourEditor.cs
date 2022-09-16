using UnityEditor;
using UnityEngine;

namespace Perigon.Entities
{
    [CustomEditor(typeof(AgentNavigationBehaviour))]
    public class AgentNavigationBehaviourEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Pickable)]
        private static void DrawGizmos(AgentNavigationBehaviour agent, GizmoType gizmoType)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(agent.transform.position, agent.StopDistanceBeforeReachingDestination);
        }
    }
}
