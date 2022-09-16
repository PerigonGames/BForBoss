using UnityEditor;
using UnityEngine;

namespace BForBoss
{
    [CustomEditor(typeof(EnemyShootingBehaviour))]
    public class EnemyShootingBehaviourEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Pickable)]
        private static void DrawGizmos(EnemyShootingBehaviour shootingBehaviour, GizmoType gizmoType)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawSphere(shootingBehaviour.transform.position, shootingBehaviour.DistanceToShootAt);
        }
    }
}
