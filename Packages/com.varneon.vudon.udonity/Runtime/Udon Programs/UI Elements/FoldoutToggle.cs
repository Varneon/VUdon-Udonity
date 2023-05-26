using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Toggle))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class FoldoutToggle : UdonSharpBehaviour
    {
        [HideInInspector]
        public Foldout Foldout;

        [SerializeField]
        private bool expanded;

        [SerializeField, HideInInspector]
        private Toggle toggle;

        [SerializeField, HideInInspector]
        private RectTransform imageTransform;

        private Vector3 imageExpandedAngle = new Vector3(0f, 0f, -90f);

        public void ToggleExpanded()
        {
            expanded = toggle.isOn;

            SetImageExpandedState(expanded);

            if (Foldout)
            {
                Foldout.Expanded = expanded;
            }
        }

        public void SetExpandedStateWithoutNotify(bool value)
        {
            if(expanded == value) { return; }

            expanded = value;

            toggle.SetIsOnWithoutNotify(expanded);

            SetImageExpandedState(expanded);
        }

        private void SetImageExpandedState(bool expanded)
        {
            imageTransform.localEulerAngles = expanded ? imageExpandedAngle : Vector3.zero;
        }

//#if UNITY_EDITOR && !COMPILER_UDONSHARP
//        private void OnValidate()
//        {
//            EditorApplication.delayCall += OnValidateDelayed;
//        }

//        private void OnValidateDelayed()
//        {
//            if (this == null) { return; }

//            if (toggle == null) { toggle = GetComponent<Toggle>(); }

//            if (toggle)
//            {
//                toggle.SetIsOnWithoutNotify(expanded);
//            }

//            if (imageTransform == null) { imageTransform = toggle.targetGraphic.rectTransform; }

//            if (Foldout)
//            {
//                Foldout.Expanded = expanded;
//            }

//            if (imageTransform)
//            {
//                SetImageExpandedState(expanded);
//            }
//        }
//#endif
    }
}

#if UNITY_EDITOR && !COMPILER_UDONSHARP
namespace Varneon.VUdon.Udonity.UIElements.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(FoldoutToggle))]
    public class FoldoutToggleEditor : Editor
    {
        private FoldoutToggle foldoutToggle;

        private SerializedProperty expandedProperty;

        private void OnEnable()
        {
            foldoutToggle = (FoldoutToggle)target;

            expandedProperty = serializedObject.FindProperty("expanded");

            SerializedProperty toggleProperty = serializedObject.FindProperty("toggle");

            if(toggleProperty.objectReferenceValue == null)
            {
                toggleProperty.objectReferenceValue = foldoutToggle.GetComponent<Toggle>();

                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(expandedProperty);

                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties();

                    bool expanded = expandedProperty.boolValue;

                    foldoutToggle.SetExpandedStateWithoutNotify(expanded);

                    if (foldoutToggle.Foldout)
                    {
                        foldoutToggle.Foldout.SetExpandedStateWithoutNotify(expanded);
                    }
                }
            }
        }
    }
}
#endif
