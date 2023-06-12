using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.Visualizers;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class MeshColliderEditor : Abstract.ComponentEditor
    {
        [Header("MeshCollider EditorElement References")]
        [SerializeField]
        private Toggle convexField;

        [SerializeField]
        private Toggle isTriggerField;

        [SerializeField]
        private ObjectField materialField;

        [SerializeField]
        private ObjectField meshField;

        [SerializeField, HideInInspector]
        internal MeshColliderVisualizer visualizerPrefab;

        private MeshColliderVisualizer activeVisualizer;

        public override Type InspectedType => typeof(MeshCollider);

        private MeshCollider target;

        private bool convex;

        protected override void OnEditorInitialized(Component component)
        {
            target = (MeshCollider)component;

            materialField.SetFieldType(typeof(PhysicMaterial));

            meshField.SetFieldType(typeof(Mesh));

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            convex = target.convex;

            SetIsTriggerFieldEnabled(convex);

            SetVisualizerActive(Expanded);

            UpdateFields();
        }

        private void OnDestroy()
        {
            if (activeVisualizer) { activeVisualizer.Destroy(); }
        }

        public void UpdateFields()
        {
            if(convex != target.convex)
            {
                convex ^= true;

                SetIsTriggerFieldEnabled(convex);
            }

            convexField.SetValueWithoutNotify(convex);

            isTriggerField.SetValueWithoutNotify(target.isTrigger);

            materialField.SetValueWithoutNotify(target.sharedMaterial);

            meshField.SetValueWithoutNotify(target.sharedMesh);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        private void SetIsTriggerFieldEnabled(bool enabled)
        {
            if (!enabled)
            {
                target.isTrigger = false;

                isTriggerField.SetValueWithoutNotify(false);
            }

            isTriggerField.Interactive = enabled;
        }

        public void OnConvexChanged()
        {
            convex = convexField.Value;

            SetIsTriggerFieldEnabled(convex);

            target.convex = convex;
        }

        public void OnIsTriggerChanged()
        {
            target.isTrigger = isTriggerField.Value;
        }

        public void OnMaterialChanged()
        {
            target.sharedMaterial = (PhysicMaterial)materialField.Value;
        }

        public void OnMeshChanged()
        {
            target.sharedMesh = (Mesh)meshField.Value;
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

                activeVisualizer = Instantiate(visualizerPrefab.gameObject).GetComponent<MeshColliderVisualizer>();

                activeVisualizer.Initialize(target);
            }
            else if (activeVisualizer)
            {
                activeVisualizer.Destroy();
            }
        }

        protected override void OnInitializedOnBuild()
        {
            if (convexField) { convexField.RegisterValueChangedCallback(this, nameof(OnConvexChanged)); }

            if (isTriggerField) { isTriggerField.RegisterValueChangedCallback(this, nameof(OnIsTriggerChanged)); }

            if (materialField) { materialField.RegisterValueChangedCallback(this, nameof(OnMaterialChanged)); }

            if (meshField) { meshField.RegisterValueChangedCallback(this, nameof(OnMeshChanged)); }

            visualizerPrefab = GetComponentInParent<Udonity>().MeshColliderVisualizerPrefab;
        }
    }
}
