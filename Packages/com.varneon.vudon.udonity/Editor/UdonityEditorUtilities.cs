using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
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

        public static string GetUdonityVersion()
        {
            const string MANIFEST_PATH = "Packages/com.varneon.vudon.udonity/package.json";

            PackageManifest manifest = AssetDatabase.LoadAssetAtPath<PackageManifest>(MANIFEST_PATH);

            if(manifest == null) { return "0.0.0"; }

            JObject manifestObject = JsonConvert.DeserializeObject<JObject>(manifest.text);

            return manifestObject.GetValue("version").ToString();
        }

        public static GameObject[] GetSceneRoots()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects();
        }

        public static T[] FindSceneComponentsOfTypeAll<T>() where T : Component
        {
            return GetSceneRoots().SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
        }

        public static T FindSceneComponentOfType<T>() where T : Component
        {
            return GetSceneRoots().Select(r => r.GetComponentInChildren<T>(true)).FirstOrDefault(c => c != null);
        }

        public static bool TryFindSceneComponentOfType<T>(out T component) where T : Component
        {
            component = FindSceneComponentOfType<T>();

            return component;
        }
    }
}
