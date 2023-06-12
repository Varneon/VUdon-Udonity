using System.Linq;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Varneon.VUdon.Editors.Editor;

namespace Varneon.VUdon.Udonity.Editor
{
    [CustomEditor(typeof(UdonityEditorDescriptor))]
    public class UdonityEditorDescriptorEditor : InspectorBase
    {
        [SerializeField]
        private Texture2D headerIcon;

        private UdonityEditorDescriptor editorDescriptor;

        private RectTransform canvasRectTransform;

        private ReorderableList rootReorderableList;

        private UdonityRootInclusionDescriptor[] rootInclusionDescriptors;

        private bool isDragAndDropValid;

        private UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase;

        protected override string FoldoutPersistenceKey => null;

        protected override InspectorHeader Header => new InspectorHeaderBuilder()
            .WithTitle("Udonity Editor")
            .WithDescription("Runtime Unity Editor for VRChat worlds")
            .WithIcon(headerIcon)
            .WithURL("GitHub", "https://github.com/Varneon/VUdon-Udonity")
            .Build();

        protected override void OnEnable()
        {
            rootInclusionDescriptors = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<UdonityRootInclusionDescriptor>(true)).ToArray();

            base.OnEnable();

            editorDescriptor = (UdonityEditorDescriptor)target;

            canvasRectTransform = editorDescriptor.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

            // If we are currently not in prefab editing mode and the descriptor is not part of a prefab asset, hide the placeholder canvas
            if(UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null && !PrefabUtility.IsPartOfPrefabAsset(editorDescriptor))
            {
                foreach(Transform t in canvasRectTransform.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
                }

                // Ensure that the expand arrow gets hidden
                EditorApplication.RepaintHierarchyWindow();
            }

            rootReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(UdonityEditorDescriptor.hierarchyRoots)), true, true, true, true);

            rootReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = rootReorderableList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 1;
                rect.height -= 2;

                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };

            rootReorderableList.drawHeaderCallback = (Rect rect) =>
            {
                if(Event.current.type == EventType.DragExited)
                {
                    isDragAndDropValid = false;
                }

                if (rect.Contains(Event.current.mousePosition))
                {
                    switch (Event.current.type)
                    {
                        case EventType.DragUpdated:
                            isDragAndDropValid = DragAndDrop.objectReferences.All(o => o.GetType().Equals(typeof(GameObject)));
                            DragAndDrop.visualMode = isDragAndDropValid ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                            Event.current.Use();
                            break;
                        case EventType.DragPerform:
                            isDragAndDropValid = false;
                            Undo.RecordObject(editorDescriptor, "Add Inspected Roots");
                            editorDescriptor.hierarchyRoots = editorDescriptor.hierarchyRoots.Union(DragAndDrop.objectReferences.Select(o => ((GameObject)o).transform)).ToList();
                            Event.current.Use();
                            break;
                    }
                }

                if (isDragAndDropValid)
                {
                    int objectCount = DragAndDrop.objectReferences.Length;

                    GUI.Label(rect, string.Concat("Add ", objectCount, " root", objectCount > 1 ? "s" : string.Empty, " to Udonity's hierarchy"));
                }
                else
                {
                    GUI.Label(rect, new GUIContent(string.Concat(rootReorderableList.serializedProperty.arraySize, " Inspected Hierarchy Roots"), "Drag and drop objects here to add them"));
                }
            };

            udonAssetDatabase = UdonityEditorUtilities.FindSceneComponentOfType<UdonAssetDatabase.UdonAssetDatabase>();
        }

        protected override void OnPreDrawFields()
        {
            GUILayout.Space(18);

            if (editorDescriptor.transform.localScale != Vector3.one)
            {
                EditorGUILayout.HelpBox("Do not change the editor descriptor transform's scale!\n\nIf you want to scale the entire editor window, use the 'Canvas Scale' option in 'Window Settings' panel below.", MessageType.Error);

                if (GUILayout.Button("Apply Descriptor Scale To Canvas"))
                {
                    Undo.RecordObjects(new Object[] { editorDescriptor.transform, canvasRectTransform }, "Fix Editor Scale");

                    Vector3 localScale = editorDescriptor.transform.localScale;

                    float transformScale = (localScale.x + localScale.y + localScale.z) / 3f;

                    float finalCanvasScale = canvasRectTransform.localScale.x * transformScale;

                    editorDescriptor.transform.localScale = Vector3.one;

                    canvasRectTransform.localScale = Vector3.one * finalCanvasScale;
                }

                GUILayout.Space(18);
            }

            GUILayout.Label("Window Settings", EditorStyles.boldLabel);

            using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    float width = EditorGUILayout.Slider("Width", canvasRectTransform.sizeDelta.x, 1920f, 2560f);

                    if (scope.changed)
                    {
                        Undo.RecordObject(canvasRectTransform, "Adjust Udonity Editor resolution");

                        canvasRectTransform.sizeDelta = new Vector2(width, canvasRectTransform.sizeDelta.y);
                    }
                }

                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    float height = EditorGUILayout.Slider("Height", canvasRectTransform.sizeDelta.y, 1000f, 1440f);

                    if (scope.changed)
                    {
                        Undo.RecordObject(canvasRectTransform, "Adjust Udonity Editor resolution");

                        canvasRectTransform.sizeDelta = new Vector2(canvasRectTransform.sizeDelta.x, height);
                    }
                }

                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    float scale = Mathf.Clamp(EditorGUILayout.FloatField("Canvas Scale", canvasRectTransform.localScale.x), 0.0001f, 10f);

                    if (scope.changed)
                    {
                        Undo.RecordObject(canvasRectTransform, "Adjust Udonity Editor scale");

                        canvasRectTransform.localScale = Vector3.one * scale;
                    }
                }
            }

            GUILayout.Space(18);

            UdonityEditorDescriptor descriptor = target as UdonityEditorDescriptor;

            if (!descriptor.hideRootInclusionTip)
            {
                EditorGUILayout.HelpBox("Tip: You can add inspected roots to Udonity's hierarchy by adding a component 'VUdon' > 'Udonity' > 'Root Inclusion Descriptor'", MessageType.Info);

                if (GUILayout.Button("OK") || rootInclusionDescriptors.Length > 0)
                {
                    Undo.RecordObject(descriptor, "Hide Inspector Tip");

                    descriptor.hideRootInclusionTip = true;
                }

                GUILayout.Space(18);
            }

            if (udonAssetDatabase == null)
            {
                EditorGUILayout.HelpBox("Tip: You can add 'UdonAssetDatabase' into your scene to clone your project's AssetDatabase, making it available during runtime.\n\nProject window and material inspection are only available if UdonAssetDatabase is present.", MessageType.Info);

                if (GUILayout.Button("Add UdonAssetDatabase To Scene"))
                {
                    UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase = new GameObject(nameof(UdonAssetDatabase.UdonAssetDatabase)).AddUdonSharpComponent<UdonAssetDatabase.UdonAssetDatabase>();

                    UdonSharpEditorUtility.GetBackingUdonBehaviour(udonAssetDatabase).SyncMethod = VRC.SDKBase.Networking.SyncType.None;

                    Selection.activeGameObject = udonAssetDatabase.gameObject;

                    Undo.RegisterCreatedObjectUndo(udonAssetDatabase.gameObject, "Add UdonAssetDatabase To Scene");
                }

                GUILayout.Space(18);
            }

            serializedObject.Update();

            rootReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(18);
        }
    }
}
