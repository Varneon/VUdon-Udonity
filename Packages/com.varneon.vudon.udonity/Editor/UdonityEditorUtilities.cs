using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Varneon.VUdon.Udonity.Editor
{
    public static class UdonityEditorUtilities
    {
        public static string UDONITY_EDITOR_DESCRIPTOR_PREFAB_PATH = "Packages/com.varneon.vudon.udonity/Runtime/Prefabs/UdonityEditor.prefab";

        [MenuItem("Varneon/VUdon/Udonity/Add Udonity To Scene")]
        private static void AddUdonityEditorDescriptorToScene()
        {
            AddUdonityEditorDescriptorToScene(true);
        }

        public static void AddUdonityEditorDescriptorToScene(bool setAsActiveSelection)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            GameObject descriptorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(UDONITY_EDITOR_DESCRIPTOR_PREFAB_PATH);

            GameObject descriptorInstance = PrefabUtility.InstantiatePrefab(descriptorPrefab, activeScene) as GameObject;

            Undo.RegisterCreatedObjectUndo(descriptorInstance, "Add Udonity To Scene");

            if (setAsActiveSelection)
            {
                Selection.activeGameObject = descriptorInstance;
            }
        }

        internal static void HideUdonitySceneIcons()
        {
            Type annotationUtilityType = Type.GetType("UnityEditor.AnnotationUtility,UnityEditor");

            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;

            MethodInfo setIconEnabledMethod = annotationUtilityType.GetMethod("SetIconEnabled", bindingFlags);

            setIconEnabledMethod.Invoke(null, new object[] { 114, "UdonityEditorDescriptor", 0 });
            setIconEnabledMethod.Invoke(null, new object[] { 114, "UdonityRootInclusionDescriptor", 0 });
        }
    }
}
