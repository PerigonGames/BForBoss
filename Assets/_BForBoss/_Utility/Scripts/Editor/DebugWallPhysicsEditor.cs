using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Perigon.Utility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugWallPhysics))]
public class DebugWallPhysicsEditor : Editor
{
    public void OnSceneGUI()
    {
        var debugPhysics = target as DebugWallPhysics;
        if (debugPhysics == null) return;

        var _contactPosition = Handles.PositionHandle(debugPhysics.ContactPoint, Quaternion.identity);
        debugPhysics.SetContactPoint(Handles.PositionHandle(debugPhysics.ContactPoint, Quaternion.identity));
        debugPhysics.SetVelocityPoint(Handles.PositionHandle(debugPhysics.VelocityPoint, Quaternion.LookRotation(debugPhysics.Velocity, Vector3.up)));
        Handles.color = Color.green;
        Handles.Label(debugPhysics.VelocityPoint, $"{debugPhysics.Velocity.magnitude:F2}");
        Handles.color = Color.cyan;
        Handles.Label(debugPhysics.ProjectionPoint, $"{debugPhysics.ProjectionMagnitude:F2}");
    }
}
