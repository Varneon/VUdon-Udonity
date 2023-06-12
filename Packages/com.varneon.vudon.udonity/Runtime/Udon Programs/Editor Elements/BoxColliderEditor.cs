using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.Visualizers;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class BoxColliderEditor : Abstract.ComponentEditor
    {
        [Header("BoxCollider EditorElement References")]
        [SerializeField]
        private Toggle isTriggerField;

        [SerializeField]
        private ObjectField materialField;

        [SerializeField]
        private Vector3Field centerField;

        [SerializeField]
        private Vector3Field sizeField;

        [SerializeField, HideInInspector]
        internal BoxColliderVisualizer visualizerPrefab;

        private BoxColliderVisualizer activeVisualizer;

        public override Type InspectedType => typeof(BoxCollider);

        private BoxCollider target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (BoxCollider)component;

            materialField.SetFieldType(typeof(PhysicMaterial));

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            SetVisualizerActive(Expanded);

            UpdateFields();
        }

        private void OnDestroy()
        {
            if (activeVisualizer) { activeVisualizer.Destroy(); }
        }

        public void UpdateFields()
        {
            isTriggerField.SetValueWithoutNotify(target.isTrigger);

            materialField.SetValueWithoutNotify(target.sharedMaterial);

            centerField.SetValueWithoutNotify(target.center);

            sizeField.SetValueWithoutNotify(target.size);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnIsTriggerChanged()
        {
            target.isTrigger = isTriggerField.Value;
        }

        public void OnCenterChanged()
        {
            target.center = centerField.Value;
        }

        public void OnSizeChanged()
        {
            target.size = sizeField.Value;
        }

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            SetVisualizerActive(expanded);

            if (expanded) { UpdateFields(); }
        }

        private void SetVisualizerActive(bool active)
        {
            if (active)
            {
                if (activeVisualizer) { return; }

                activeVisualizer = Instantiate(visualizerPrefab.gameObject).GetComponent<BoxColliderVisualizer>();

                activeVisualizer.Initialize(target);
            }
            else if (activeVisualizer)
            {
                activeVisualizer.Destroy();
            }
        }

        protected override void OnInitializedOnBuild()
        {
            if (isTriggerField) { isTriggerField.RegisterValueChangedCallback(this, nameof(OnIsTriggerChanged)); }

            if (centerField) { centerField.RegisterValueChangedCallback(this, nameof(OnCenterChanged)); }

            if(sizeField) { sizeField.RegisterValueChangedCallback(this, nameof(OnSizeChanged)); }

            visualizerPrefab = GetComponentInParent<Udonity>().BoxColliderVisualizerPrefab;
        }
    }
}
