using UnityEditor;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Editor
{
    [CustomEditor(typeof(UdonityEditorDescriptor))]
    public class UdonityEditorDescriptorEditor : UnityEditor.Editor
    {
        private UdonityEditorDescriptor editorDescriptor;

        private RectTransform canvasRectTransform;

        private void OnEnable()
        {
            editorDescriptor = (UdonityEditorDescriptor)target;

            canvasRectTransform = editorDescriptor.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

            for(int i = 0; i < editorDescriptor.transform.childCount; i++)
            {
                editorDescriptor.transform.GetChild(i).gameObject.hideFlags = HideFlags.None;// HideFlags.HideInHierarchy | HideFlags.NotEditable;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

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
    }
}
