using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Fields.Abstract;
using Varneon.VUdon.Udonity.UIElements;

namespace Varneon.VUdon.Udonity.Editors.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ComponentEditor : UdonSharpBehaviour
    {
        /// <summary>
        /// Is the editor currently expanded
        /// </summary>
        public bool Expanded => expanded;

        [Header("Abstract EditorElement References")]
        [SerializeField]
        protected Toggle enabledToggle;

        [SerializeField]
        private Dropdown contextDropdown;

        [SerializeField]
        private Foldout inspectorFoldout;

        public abstract Type InspectedType { get; }

        protected Windows.Inspector.Inspector containingInspector;

        private bool expanded = true;

        private Component abstractTarget;

        internal void Initialize(Component component, Windows.Inspector.Inspector inspector = null, bool collapsed = false)
        {
            gameObject.SetActive(true);

            if (collapsed) { SetEditorExpandedStateWithoutNotify(false); }

            containingInspector = inspector;

            abstractTarget = component;

            OnEditorInitialized(component);
        }

        protected virtual void OnEditorInitialized(Component component) { }

        protected virtual void OnFieldValueChanged(Field field, object newValue) { }

        protected virtual void OnToggleEnabled(bool enabled) { }

        protected virtual void OnToggleExpanded(bool expanded) { }

        public virtual void OnContextDropdownActionInvoked()
        {
            OnContextDropdownActionInvoked(contextDropdown.value);
        }

        protected virtual void OnContextDropdownActionInvoked(int index)
        {
            if (contextDropdown.value == 0)
            {
                RemoveComponent();
            }
        }

        public void OnExpandedStateChanged()
        {
            expanded = inspectorFoldout.Expanded;

            containingInspector.SetTypeCollapsedState(InspectedType, !expanded);

            OnToggleExpanded(expanded);
        }

        public void ToggleEnabled()
        {
            OnToggleEnabled(enabledToggle.isOn);
        }

        internal void SetEditorExpandedStateWithoutNotify(bool value)
        {
            expanded = value;

            if (inspectorFoldout) { inspectorFoldout.SetExpandedStateWithoutNotify(value); }
        }

        public void RemoveComponent()
        {
            Destroy(abstractTarget);

            Destroy(gameObject);
        }

        protected virtual void OnInitializedOnBuild() { }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            if (inspectorFoldout) { inspectorFoldout.RegisterValueChangedCallback(this, nameof(OnExpandedStateChanged)); }

            OnInitializedOnBuild();
        }
#endif
    }
}
