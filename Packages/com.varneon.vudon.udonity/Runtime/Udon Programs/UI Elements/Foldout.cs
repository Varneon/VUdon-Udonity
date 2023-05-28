using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Foldout : UdonSharpBehaviour
    {
        public bool Expanded
        {
            get => expanded;
            set
            {
                expanded = value;

                // Shouldn't require a null check, just try to keep UdonSharp happy
                if (toggle) { toggle.SetExpandedStateWithoutNotify(expanded); }

                SetContentExpandedState(expanded);

                OnExpandedStateChanged();
            }
        }

        [Header("Options")]
        [SerializeField]
        private bool expanded;

        [Header("References")]
        [SerializeField]
        private FoldoutToggle toggle;

        [SerializeField, HideInInspector]
        private UdonSharpBehaviour expandedStateChangedCallbackReceiver;

        [SerializeField, HideInInspector]
        private string expandedStateChangedCallbackMethod;

        public void RegisterValueChangedCallback(UdonSharpBehaviour callbackReceiver, string methodName)
        {
            expandedStateChangedCallbackReceiver = callbackReceiver;

            expandedStateChangedCallbackMethod = methodName;
        }

        public void OnExpandedStateChanged()
        {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
            return;
#endif

#pragma warning disable CS0162 // Callback shouldn't be invoked in editor
            if (expandedStateChangedCallbackReceiver)
            {
                expandedStateChangedCallbackReceiver.SendCustomEvent(expandedStateChangedCallbackMethod);
            }
#pragma warning restore CS0162
        }

        private void SetContentExpandedState(bool expanded)
        {
            gameObject.SetActive(expanded);

            LayoutGroup[] layoutGroups = GetComponentsInParent<LayoutGroup>(true);

            foreach (LayoutGroup layoutGroup in layoutGroups)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }

        public void SetExpandedStateWithoutNotify(bool value)
        {
            expanded = value;

            toggle.SetExpandedStateWithoutNotify(expanded);

            SetContentExpandedState(expanded);
        }

//#if UNITY_EDITOR && !COMPILER_UDONSHARP
//        private void OnValidate()
//        {
//            UnityEditor.EditorApplication.delayCall += OnValidateDelayed;
//        }

//        private void OnValidateDelayed()
//        {
//            if (this == null) { return; }

//            if (toggle)
//            {
//                toggle.Foldout = this;

//                toggle.SetExpandedStateWithoutNotify(expanded);
//            }

//            SetContentExpandedState(expanded);
//        }
//#endif
    }
}

#if UNITY_EDITOR && !COMPILER_UDONSHARP
namespace Varneon.VUdon.Udonity.UIElements.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(Foldout))]
    public class FoldoutEditor : Editor
    {
        private Foldout foldout;

        private SerializedProperty
            expandedProperty,
            toggleProperty;

        private void OnEnable()
        {
            foldout = (Foldout)target;

            expandedProperty = serializedObject.FindProperty("expanded");

            toggleProperty = serializedObject.FindProperty("toggle");
        }

        public override void OnInspectorGUI()
        {
            using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(toggleProperty);

                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties();

                    FoldoutToggle foldoutToggle = toggleProperty.objectReferenceValue as FoldoutToggle;

                    if (foldoutToggle)
                    {
                        Undo.RecordObject(foldoutToggle, "Assign Foldout");

                        foldoutToggle.Foldout = foldout;
                    }
                }
            }

            using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(expandedProperty);

                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties();

                    if (toggleProperty.objectReferenceValue)
                    {
                        foldout.Expanded = expandedProperty.boolValue;
                    }
                }
            }
        }
    }
}
#endif
