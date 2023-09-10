using ECM2.Examples.Gameplay.BouncerExample;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using GUID = FMOD.GUID;

namespace BForBoss
{
    public static class JumpPadReplacerTool
    {
        private const string AUDIO_PATH = "event:/AudioEvents/Environment/Environment_JumpPad";
        
        private const string IMPULSE = "_launchImpulse";
        private const string VERTICAL_VELOCITY = "_overrideVerticalVelocity";
        private const string HORIZONTAL_VELOCITY = "_overrideLateralVelocity";
        private const string AUDIO = "_jumpAudio";
        
        [MenuItem("Tools/Replace ECM2 Jump pad with BForBoss jump pad")]
        public static void ReplaceAllInScene()
        {

            var bouncers = Object.FindObjectsOfType<Bouncer>();

            var eventRef = EventReference.Find(AUDIO_PATH);

            foreach(var bouncer in bouncers)
            {
                var impulse = bouncer.launchImpulse;
                var vert = bouncer.overrideVerticalVelocity;
                var horiz = bouncer.overrideLateralVelocity;
                
                var jumpPad = Undo.AddComponent<JumpPad>(bouncer.gameObject);

                SerializedObject obj = new SerializedObject(jumpPad);

                var impulseProp = obj.FindProperty(IMPULSE);
                var vertProp = obj.FindProperty(VERTICAL_VELOCITY);
                var horizProp = obj.FindProperty(HORIZONTAL_VELOCITY);
                var audioProp = obj.FindProperty(AUDIO);

                impulseProp.floatValue = impulse;
                vertProp.boolValue = vert;
                horizProp.boolValue = horiz;
                SetEventRef(audioProp, eventRef.Guid, eventRef.Path);
                
                obj.ApplyModifiedProperties();
            }
            
            Undo.RecordObjects(bouncers, "Replace bouncers");
            for (int i = bouncers.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(bouncers[i]);
            }
        }

        private static void SetEventRef(SerializedProperty prop, GUID guid, string path)
        {
            const string GUID = "Guid";
            const string PATH = "Path";
            const string DATA1 = "Data1";
            const string DATA2 = "Data2";
            const string DATA3 = "Data3";
            const string DATA4 = "Data4";
            
            var guidProp = prop.FindPropertyRelative(GUID);
            guidProp.FindPropertyRelative(DATA1).intValue = guid.Data1;
            guidProp.FindPropertyRelative(DATA2).intValue = guid.Data2;
            guidProp.FindPropertyRelative(DATA3).intValue = guid.Data3;
            guidProp.FindPropertyRelative(DATA4).intValue = guid.Data4;

            prop.FindPropertyRelative(PATH).stringValue = path;
        }
    }
}
