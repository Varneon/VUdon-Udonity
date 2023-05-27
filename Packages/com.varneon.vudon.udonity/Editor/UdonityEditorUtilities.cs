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

            if (setAsActiveSelection)
            {
                Selection.activeGameObject = descriptorInstance;
            }
        }
    }
}
