﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
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

        protected override string FoldoutPersistenceKey => null;

        protected override InspectorHeader Header => new InspectorHeaderBuilder()
            .WithTitle("Udonity Editor")
            .WithDescription("Runtime Unity Editor for VRChat worlds")
            .WithIcon(headerIcon)
            .WithURL("GitHub", "https://github.com/Varneon/VUdon-Udonity")
            .Build();

        protected override void OnEnable()
        {
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

            GUI.color = Color.black;

            using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.Box(verticalScope.rect, string.Empty);

                GUI.color = Color.white;

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

            serializedObject.Update();

            rootReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(18);
        }
    }
}
