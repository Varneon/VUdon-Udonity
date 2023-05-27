using System.Linq;
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

            for(int i = 0; i < editorDescriptor.transform.childCount; i++)
            {
                editorDescriptor.transform.GetChild(i).gameObject.hideFlags = HideFlags.None;// HideFlags.HideInHierarchy | HideFlags.NotEditable;
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
                GUI.Label(rect, string.Concat(rootReorderableList.serializedProperty.arraySize, " Inspected Hierarchy Roots"));
            };
        }

        protected override void OnPreDrawFields()
        {
            GUILayout.Space(18);

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
                    float scale = EditorGUILayout.FloatField("Canvas Scale", canvasRectTransform.localScale.x);

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

            serializedObject.Update();

            rootReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(18);
        }
    }
}
