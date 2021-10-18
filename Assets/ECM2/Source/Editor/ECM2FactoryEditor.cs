using ECM2.Characters;
using ECM2.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ECM2.Editor
{
    public static class ECM2FactoryEditor
    {
        private static void InitCharacter(GameObject go)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();

            rb.drag = 0.0f;
            rb.angularDrag = 0.0f;
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            CapsuleCollider capsuleCollider = go.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material =
                AssetDatabase.LoadAssetAtPath<PhysicMaterial>(
                    "Assets/ECM2/Physic Materials/Frictionless.physicMaterial");

            PhysicMaterial physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = new PhysicMaterial("Frictionless")
                {
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    bounciness = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Multiply,
                    bounceCombine = PhysicMaterialCombine.Multiply
                };

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    $"CharacterMovement: No 'PhysicMaterial' found for '{go.name}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{go.name}'.");
            }
        }

        [MenuItem("GameObject/ECM2/Character", false, 0)]
        public static void CreateCharacter()
        {
            GameObject go = new GameObject("ECM2_Character", typeof(Rigidbody), typeof(CapsuleCollider), typeof(CharacterMovement), typeof(Character));

            InitCharacter(go);

            // Assign default input actions

            Character character = go.GetComponent<Character>();
            if (character)
                character.actions =
                    AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                        "Assets/ECM2/Input Actions/ECM2_Character_InputActions.inputactions");

            // Focus the newly created character

            Selection.activeGameObject = go;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/ECM2/AgentCharacter", false, 0)]
        public static void CreateAgentCharacter()
        {
            GameObject go = new GameObject("ECM2_AgentCharacter", typeof(NavMeshAgent), typeof(Rigidbody),
                typeof(CapsuleCollider), typeof(CharacterMovement), typeof(AgentCharacter));

            InitCharacter(go);

            // Assign default input actions

            Character character = go.GetComponent<Character>();
            if (character)
                character.actions =
                    AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                        "Assets/ECM2/Input Actions/ECM2_Agent_InputActions.inputactions");

            // Focus the newly created character

            Selection.activeGameObject = go;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/ECM2/FirstPersonCharacter", false, 0)]
        public static void CreateFirstPersonCharacter()
        {
            GameObject go = new GameObject("ECM2_FirstPersonCharacter", typeof(Rigidbody), typeof(CapsuleCollider),
                typeof(CharacterMovement), typeof(CharacterLook), typeof(FirstPersonCharacter));
            
            InitCharacter(go);

            // Assign default input actions

            Character character = go.GetComponent<Character>();
            if (character)
                character.actions =
                    AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                        "Assets/ECM2/Input Actions/ECM2_FirstPerson_InputActions.inputactions");

            // Init first person RIG FirstPersonCharacter -> rootPivot -> eyePivot -> camera

            GameObject rootPivot = new GameObject("Root");
            rootPivot.transform.SetParent(go.transform);
            
            GameObject eyePivot = new GameObject("Eye");
            eyePivot.transform.SetParent(rootPivot.transform);
            eyePivot.transform.localPosition = new Vector3(0.0f, 1.6f, 0.0f);

            GameObject cameraGameObject = new GameObject("Camera", typeof(Camera), typeof(AudioListener));
            cameraGameObject.transform.SetParent(eyePivot.transform, false);

            FirstPersonCharacter fpc = go.GetComponent<FirstPersonCharacter>();

            fpc.camera = cameraGameObject.GetComponent<Camera>();
            fpc.rootPivot = rootPivot.transform;
            fpc.eyePivot = eyePivot.transform;

            // Focus the newly created character

            Selection.activeGameObject = go;
            SceneView.FrameLastActiveSceneView();
        }
    }
}
