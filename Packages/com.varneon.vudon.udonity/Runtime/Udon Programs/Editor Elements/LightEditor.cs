using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    public class LightEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private EnumField typeField;

        [SerializeField]
        private FloatField rangeField;

        [SerializeField]
        private FloatField spotAngleField;

        [SerializeField]
        private ColorField colorField;

        [SerializeField]
        private FloatField intensityField;

        [SerializeField]
        private FloatField indirectMultiplierField;

        [SerializeField]
        private EnumField shadowTypeField;

        [SerializeField]
        private FloatField strengthField;

        [SerializeField]
        private EnumField resolutionField;

        [SerializeField]
        private EnumField renderModeField;

        [SerializeField]
        private LayerMaskField cullingMaskField;

        public override Type InspectedType => typeof(Light);

        private Light target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (Light)component;

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            typeField.RegisterValueChangedCallback(this, nameof(OnTypeChanged));

            rangeField.RegisterValueChangedCallback(this, nameof(OnRangeChanged));

            spotAngleField.RegisterValueChangedCallback(this, nameof(OnSpotAngleChanged));

            colorField.RegisterValueChangedCallback(this, nameof(OnColorChanged));

            intensityField.RegisterValueChangedCallback(this, nameof(OnIntensityChanged));

            indirectMultiplierField.RegisterValueChangedCallback(this, nameof(OnIndirectMultiplierChanged));

            shadowTypeField.RegisterValueChangedCallback(this, nameof(OnShadowTypeChanged));

            strengthField.RegisterValueChangedCallback(this, nameof(OnStrengthChanged));

            resolutionField.RegisterValueChangedCallback(this, nameof(OnResolutionChanged));

            renderModeField.RegisterValueChangedCallback(this, nameof(OnRenderModeChanged));

            cullingMaskField.RegisterValueChangedCallback(this, nameof(OnCullingMaskChanged));

            cullingMaskField.SetValueWithoutNotify(target.cullingMask);

            UpdateFields();
        }

        public void UpdateFields()
        {
            typeField.SetValueWithoutNotify((int)target.type);

            rangeField.SetValueWithoutNotify(target.range);

            spotAngleField.SetValueWithoutNotify(target.spotAngle);

            colorField.SetValueWithoutNotify(target.color);

            intensityField.SetValueWithoutNotify(target.intensity);

            indirectMultiplierField.SetValueWithoutNotify(target.bounceIntensity);

            shadowTypeField.SetValueWithoutNotify((int)target.shadows);

            strengthField.SetValueWithoutNotify(target.shadowStrength);

            resolutionField.SetValueWithoutNotify(EnumResolver.UnityEngine_Rendering_LightShadowResolution.ToInt(target.shadowResolution));

            renderModeField.SetValueWithoutNotify((int)target.renderMode);

            // LayerMaskDropdown may not be compatible with per-frame updates
            //cullingMaskField.SetValueWithoutNotify(target.cullingMask);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnTypeChanged()
        {
            target.type = (LightType)typeField.Value;
        }

        public void OnRangeChanged()
        {
            target.range = rangeField.Value;
        }

        public void OnSpotAngleChanged()
        {
            target.spotAngle = spotAngleField.Value;
        }

        public void OnColorChanged()
        {
            target.color = colorField.Value;
        }

        public void OnIntensityChanged()
        {
            target.intensity = intensityField.Value;
        }

        public void OnIndirectMultiplierChanged()
        {
            target.bounceIntensity = indirectMultiplierField.Value;
        }

        public void OnShadowTypeChanged()
        {
            target.shadows = (LightShadows)shadowTypeField.Value;
        }

        public void OnStrengthChanged()
        {
            target.shadowStrength = strengthField.Value;
        }

        public void OnResolutionChanged()
        {
            target.shadowResolution = EnumResolver.UnityEngine_Rendering_LightShadowResolution.FromInt(resolutionField.Value);
        }

        public void OnRenderModeChanged()
        {
            target.renderMode = (LightRenderMode)renderModeField.Value;
        }

        public void OnCullingMaskChanged()
        {
            target.cullingMask = cullingMaskField.Value;
        }

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }
    }
}
