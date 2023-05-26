using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Editors;
using Varneon.VUdon.Udonity.Editors.Abstract;
using Varneon.VUdon.Udonity.UIElements.Abstract;
using Varneon.VUdon.Udonity.Windows.Abstract;
using Varneon.VUdon.Udonity.Windows.Project;
using Varneon.VUdon.Udonity.Windows.UdonMonitor;
using Varneon.VUdon.UdonProgramDataStorage;
using VRC.Udon;
using Object = UnityEngine.Object;

namespace Varneon.VUdon.Udonity.Editor
{
    public static class UdonityBuildPostProcessor
    {
        #region Scene
        private static GameObject[] SceneRoots
        {
            get
            {
                if(sceneRoots == null || sceneRoots.Length != ActiveScene.rootCount)
                {
                    //Debug.Log("Scene roots were null or didn't match the previous ones, attempting to reassign them...");

                    sceneRoots = ActiveScene.GetRootGameObjects();
                }

                return sceneRoots;
            }
        }

        private static GameObject[] sceneRoots;

        private static Scene ActiveScene
        {
            get
            {
                if(activeScene == null || !activeScene.IsValid())
                {
                    //Debug.Log("Active scene was null or not valid, attempting to reassign it...");

                    activeScene = SceneManager.GetActiveScene();
                }

                return activeScene;
            }
        }

        private static Scene activeScene;
        #endregion

        private static Udonity udonity;

        private static Transform editorRoot;

        private static Windows.ConsoleWindow consoleWindow;

        private static Windows.Inspector.Inspector inspectorWindow;

        private static Windows.Animator.AnimatorWindow animatorWindow;

        private static ProjectWindow projectWindow;

        private static Windows.Hierarchy.Hierarchy hierarchyWindow;

        private static UdonMonitorWindow udonMonitorWindow;

        private static Windows.Profiler profilerWindow;

        private static Windows.LightingWindow lightingWindow;

        private static Windows.SceneView sceneViewWindow;

        private static UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase;

        private static UdonityLink.UdonityLinkClient udonityLinkClient;

        /// <summary>
        /// Finds a UdonityEditorDescriptor and replaces it with the full Udonity Editor prefab
        /// </summary>
        [UsedImplicitly]
        [PostProcessScene(-1000)] // Leave some room for other build postprocessors between -1000 and -1
        private static void InitializeUdonity()
        {
            // Find the descriptor
            UdonityEditorDescriptor editorDescriptor = Object.FindObjectOfType<UdonityEditorDescriptor>();

            // If there is no descriptor in the scene, return
            if(editorDescriptor == null)
            {
                //Debug.Log("Couldn't find Udonity Editor Descriptor in the scene!");
                
                return;
            }

            //Debug.Log("Initializing Udonity Editor...");

            // Get the descriptor's Canvas RectTransform
            RectTransform editorDescriptorCanvasRectTransform = editorDescriptor.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

            // Load the full prefab
            GameObject editorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.varneon.vudon.udonity/Runtime/Prefabs/Editor/UdonityEditor.prefab");

            // Instantiate the full prefab
            GameObject editorInstance = Object.Instantiate(editorPrefab, editorDescriptor.transform.position, editorDescriptor.transform.rotation);

            // Get the editor's Canvas RectTransform
            RectTransform editorCanvasRectTransform = editorInstance.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

            // Apply the size of the Canvas
            editorCanvasRectTransform.sizeDelta = editorDescriptorCanvasRectTransform.sizeDelta;

            // Apply the scale of the Canvas
            editorCanvasRectTransform.localScale = Vector3.one * editorDescriptorCanvasRectTransform.localScale.x;

            // Assign the editor
            udonity = editorInstance.GetComponentInChildren<Udonity>();

            // Assign the inspected scope roots to the editor
            udonity.roots = editorDescriptor.hierarchyRoots;

            editorRoot = editorInstance.transform;

            // Destroy the descriptor
            Object.DestroyImmediate(editorDescriptor.gameObject);
        }

        [UsedImplicitly]
        [PostProcessScene(-100)]
        private static void PostProcessUdonity()
        {
            if (udonity == null)
            {
                //Debug.Log("No Udonity Editor assigned by the descriptor, attempting to find one from the scene...");

                udonity = FindSceneComponentOfType<Udonity>();

                if (udonity == null)
                {
                    //Debug.Log("Couldn't find Udonity in the scene!");

                    return;
                }

                editorRoot = udonity.transform.parent;
            }

            //Debug.Log(string.Concat("Postprocessing Udonity: ", udonity), udonity);

            consoleWindow = udonity.ConsoleWindow;

            hierarchyWindow = udonity.Hierarchy;

            inspectorWindow = udonity.Inspector;

            animatorWindow = udonity.AnimatorWindow;

            projectWindow = udonity.ProjectWindow;

            udonityLinkClient = udonity.LinkClient;

            udonAssetDatabase = FindSceneComponentOfType<UdonAssetDatabase.UdonAssetDatabase>();

            if (udonityLinkClient) { udonityLinkClient.udonAssetDatabase = udonAssetDatabase; }

            PostProcessApplicationWindows();

            PostProcessWindowLayoutElements();

            PostProcessConsoleWindow();

            PostProcessHierarchy();

            PostProcessInspector();

            PostProcessProjectWindow();

            PostProcessUdonMonitorWindow();

            PostProcessAnimatorWindow();

            PostProcessDropdownMenus();

            PostProcessEditors();

            UdonProgramDataStorageGenerator.GenerateProgramDataStorage();

            foreach (UdonBehaviour ub in editorRoot.GetComponentsInChildren<UdonBehaviour>(true))
            {
                ub.SyncMethod = VRC.SDKBase.Networking.SyncType.None;
            }
        }

        [UsedImplicitly]
        [PostProcessScene(1000)]
        private static void PostProcessEditorIcons()
        {
            BuiltinEditorIconImage[] images = FindSceneComponentsOfTypeAll<BuiltinEditorIconImage>();

            foreach (BuiltinEditorIconImage image in images)
            {
                image.Initialize();
            }
        }

        [UsedImplicitly]
        [PostProcessScene(2)]
        private static void PostProcessUdonityRuntimePrefabs()
        {
            if(udonity == null) { return; }

            string[] runtimePrefabs = Directory.GetFiles("Assets/UdonSharp/PrefabBuild/Packages/com.varneon.vudon.udonity", "*.prefab", SearchOption.AllDirectories);

            AssetDatabase.StartAssetEditing();

            foreach (string prefab in runtimePrefabs)
            {
                GameObject prefabAsset = PrefabUtility.LoadPrefabContents(prefab);

                foreach (BuiltinEditorIconImage i in prefabAsset.GetComponentsInChildren<BuiltinEditorIconImage>(true))
                {
                    i.Initialize();
                }

                PrefabUtility.SaveAsPrefabAsset(prefabAsset, prefab);

                PrefabUtility.UnloadPrefabContents(prefabAsset);
            }

            AssetDatabase.StopAssetEditing();

            AssetDatabase.Refresh();
        }

        private static void PostProcessApplicationWindows()
        {
            Pointer pointer = Object.FindObjectOfType<Pointer>();

            foreach (ApplicationWindow window in FindSceneComponentsOfTypeAll<ApplicationWindow>())
            {
                window.pointer = pointer;

                window.rectTransform = window.GetComponent<RectTransform>();
            }
        }

        private static void PostProcessWindowLayoutElements()
        {
            foreach (WindowLayoutElement element in FindSceneComponentsOfTypeAll<WindowLayoutElement>())
            {
                element.rectTransform = element.GetComponent<RectTransform>();
            }
        }

        private static void PostProcessConsoleWindow()
        {
            if(consoleWindow == null) { return; }

            consoleWindow.InitializeOnBuild();
        }

        private static void PostProcessHierarchy()
        {
            if(hierarchyWindow == null) { return; }

            UdonityRootInclusionDescriptor[] rootInclusionDescriptors = FindSceneComponentsOfTypeAll<UdonityRootInclusionDescriptor>();

            hierarchyWindow.roots = hierarchyWindow.GetComponentInParent<Udonity>().roots.Union(rootInclusionDescriptors.Select(d => d.transform.root)).ToArray();

            foreach(UdonityRootInclusionDescriptor descriptor in rootInclusionDescriptors)
            {
                Object.DestroyImmediate(descriptor);
            }
        }

        private static void PostProcessInspector()
        {
            if(inspectorWindow == null) { return; }

            string[] editorGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { "Packages/com.varneon.vudon.udonity/Runtime/Prefabs/UI/Editor Elements/Components" });

            string[] editorPaths = editorGUIDs.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray();

            UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase = Object.FindObjectOfType<UdonAssetDatabase.UdonAssetDatabase>();

            inspectorWindow.udonAssetDatabase = udonAssetDatabase;

            inspectorWindow.udonMonitorWindow = editorRoot.GetComponentInChildren<UdonMonitorWindow>(true);

            Transform inspectorTransform = inspectorWindow.transform;

            List<ComponentEditor> editors = new List<ComponentEditor>();

            foreach (string editorPath in editorPaths)
            {
                ComponentEditor editorPrefab = AssetDatabase.LoadAssetAtPath<ComponentEditor>(editorPath);

                if (editorPrefab == null) { Debug.LogError($"ComponentEditor at path {editorPath} couldn't be loaded!"); continue; }

                ComponentEditor editor = Object.Instantiate(editorPrefab, inspectorTransform, false);

                editor.gameObject.SetActive(false);

                editors.Add(editor);
            }

            inspectorWindow.componentEditors = editors.ToArray();

            inspectorWindow.availableComponentEditorTypes = editors.Select(e => e.InspectedType).ToArray();

            inspectorWindow.gameObjectEditor = InstantiateEditorElement(inspectorWindow.gameObjectEditor, inspectorTransform);
            inspectorWindow.defaultComponentEditor = InstantiateEditorElement(inspectorWindow.defaultComponentEditor, inspectorTransform);
            inspectorWindow.defaultVRCInternalComponentEditor = InstantiateEditorElement(inspectorWindow.defaultVRCInternalComponentEditor, inspectorTransform);
            inspectorWindow.missingComponentEditor = InstantiateEditorElement(inspectorWindow.missingComponentEditor, inspectorTransform);
            inspectorWindow.materialEditor = InstantiateEditorElement(inspectorWindow.materialEditor, inspectorTransform);

            List<Editors.MaterialEditor> materialEditors = new List<Editors.MaterialEditor>();

            Editors.MaterialEditor materialEditor = AssetDatabase.LoadAssetAtPath<Editors.MaterialEditor>("Packages/com.varneon.vudon.udonity/Runtime/Prefabs/UI/Editor Elements/EditorElement_UnityEditor.Material.prefab");

            string[] shaderNames = udonAssetDatabase == null ? new string[0] : udonAssetDatabase.ShaderNames;

            foreach (string shaderName in shaderNames)
            {
                Editors.MaterialEditor newMaterialEditor = Object.Instantiate(materialEditor, inspectorTransform, false);

                newMaterialEditor.GenerateFields(Shader.Find(shaderName), udonAssetDatabase);

                newMaterialEditor.gameObject.SetActive(false);

                materialEditors.Add(newMaterialEditor);
            }

            inspectorWindow.availableMaterialEditors = shaderNames;

            inspectorWindow.materialEditors = materialEditors.ToArray();
        }

        private static void PostProcessProjectWindow()
        {
            if(projectWindow == null) { return; }

            UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase = FindSceneComponentOfType<UdonAssetDatabase.UdonAssetDatabase>();

            projectWindow.assetDatabase = udonAssetDatabase;

            string[] folders = udonAssetDatabase == null ? new string[0] : udonAssetDatabase.FolderPaths;

            projectWindow.folderHierarchies = string.Join("\n", folders);

            for (int i = 0; i < folders.Length; i++)
            {
                string thisDirectory = folders.ElementAt(i);

                bool hasSubfolders = false;

                if (i < folders.Length - 1)
                {
                    hasSubfolders = folders.ElementAt(i + 1).Contains(thisDirectory);
                }

                GameObject newElement = Object.Instantiate(projectWindow.hierarchyElement, projectWindow.hierarchyContainer, false);

                newElement.SetActive(true);

                ProjectHierarchyElement projectHierarchyElement = newElement.GetComponent<ProjectHierarchyElement>();

                projectHierarchyElement.Directory = thisDirectory;

                projectHierarchyElement.ProjectWindow = projectWindow;

                newElement.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(thisDirectory);

                int depth = thisDirectory.Split('/').Length - 1;

                newElement.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(depth * -16f, 0f);

                if (!hasSubfolders) { newElement.GetComponentInChildren<Toggle>().gameObject.SetActive(false); }
            }
        }

        private static void PostProcessUdonMonitorWindow()
        {
            UdonMonitorWindow udonMonitorWindow = editorRoot.GetComponentInChildren<UdonMonitorWindow>(true);

            if(udonMonitorWindow == null) { return; }

            udonMonitorWindow.programDataStorage = editorRoot.GetComponentInChildren<UdonProgramDataStorage>(true);
        }

        private static void PostProcessAnimatorWindow()
        {
            if(animatorWindow == null) { return; }

            inspectorWindow.animatorWindow = animatorWindow;

            AnimatorControllerDataStorage animatorControllerDataStorage = editorRoot.GetComponentInChildren<AnimatorControllerDataStorage>(true);

            animatorWindow.dataStorage = animatorControllerDataStorage;

            AnimatorControllerDataStorageGenerator.GenerateAnimatorControllerDataStorage();
        }

        private static void PostProcessDropdownMenus()
        {
            DropdownMenu[] dropdownMenus = FindSceneComponentsOfTypeAll<DropdownMenu>();

            foreach(DropdownMenu dropdownMenu in dropdownMenus)
            {
                dropdownMenu.InitializeOnBuild();
            }
        }

        private static void PostProcessEditors()
        {
            foreach(ComponentEditor componentEditor in FindSceneComponentsOfTypeAll<ComponentEditor>())
            {
                componentEditor.InitializeOnBuild();
            }

            foreach (Editors.MaterialEditor materialEditor in FindSceneComponentsOfTypeAll<Editors.MaterialEditor>())
            {
                materialEditor.InitializeOnBuild();
            }

            foreach(GameObjectEditor gameObjectEditor in FindSceneComponentsOfTypeAll<GameObjectEditor>())
            {
                gameObjectEditor.InitializeOnBuild();
            }
        }

        private static T[] FindSceneComponentsOfTypeAll<T>() where T : Component
        {
            return SceneRoots.SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
        }

        private static T FindSceneComponentOfType<T>() where T : Component
        {
            return SceneRoots.Select(r => r.GetComponentInChildren<T>(true)).FirstOrDefault(c => c != null);
        }

        private static T InstantiateEditorElement<T>(T editor, Transform parent) where T : Component
        {
            T newEditorInstance = Object.Instantiate<T>(editor, parent, false);

            newEditorInstance.gameObject.SetActive(false);

            return newEditorInstance;
        }
    }
}
