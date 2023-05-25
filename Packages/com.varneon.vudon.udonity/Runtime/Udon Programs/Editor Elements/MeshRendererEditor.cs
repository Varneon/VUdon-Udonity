using System;
using UnityEngine;
using UnityEngine.Rendering;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    public class MeshRendererEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private EnumField castShadowsField;

        [SerializeField]
        private Toggle receiveShadowsToggle;

        [SerializeField]
        private EnumField lightProbesField;

        [SerializeField]
        private EnumField reflectionProbesField;

        [SerializeField]
        private ObjectField anchorOverrideField;

        [SerializeField]
        private EnumField motionVectorsField;

        [SerializeField]
        private Toggle dynamicOcclusionField;

        public override Type InspectedType => typeof(MeshRenderer);

        private MeshRenderer target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (MeshRenderer)component;

            anchorOverrideField.SetFieldType(typeof(Transform));

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            UpdateFields();
        }

        public void UpdateFields()
        {
            castShadowsField.SetValueWithoutNotify((int)target.shadowCastingMode);

            receiveShadowsToggle.SetValueWithoutNotify(target.receiveShadows);

            lightProbesField.SetValueWithoutNotify((int)target.lightProbeUsage);

            reflectionProbesField.SetValueWithoutNotify((int)target.reflectionProbeUsage);

            anchorOverrideField.SetValueWithoutNotify(target.probeAnchor);

            motionVectorsField.SetValueWithoutNotify((int)target.motionVectorGenerationMode);

            dynamicOcclusionField.SetValueWithoutNotify(target.allowOcclusionWhenDynamic);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnCastShadowsChanged()
        {
            target.shadowCastingMode = (ShadowCastingMode)castShadowsField.Value;
        }

        public void OnReceiveShadowsChanged()
        {
            target.receiveShadows = receiveShadowsToggle.Value;
        }

        public void OnLightProbesChanged()
        {
            target.lightProbeUsage = (LightProbeUsage)lightProbesField.Value;
        }

        public void OnReflectionProbesChanged()
        {
            target.reflectionProbeUsage = (ReflectionProbeUsage)reflectionProbesField.Value;
        }

        public void OnMotionVectorsChanged()
        {
            target.motionVectorGenerationMode = (MotionVectorGenerationMode)motionVectorsField.Value;
        }

        public void OnDynamicOcclusionChanged()
        {
            target.allowOcclusionWhenDynamic = dynamicOcclusionField.Value;
        }

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnInitializedOnBuild()
        {
            if (castShadowsField) { castShadowsField.RegisterValueChangedCallback(this, nameof(OnCastShadowsChanged)); }

            if (receiveShadowsToggle) { receiveShadowsToggle.RegisterValueChangedCallback(this, nameof(OnReceiveShadowsChanged)); }

            if (lightProbesField) { lightProbesField.RegisterValueChangedCallback(this, nameof(OnLightProbesChanged)); }

            if (reflectionProbesField) { reflectionProbesField.RegisterValueChangedCallback(this, nameof(OnReflectionProbesChanged)); }

            if (motionVectorsField) { motionVectorsField.RegisterValueChangedCallback(this, nameof(OnMotionVectorsChanged)); }

            if (dynamicOcclusionField) { dynamicOcclusionField.RegisterValueChangedCallback(this, nameof(OnDynamicOcclusionChanged)); }
        }
    }
}
