using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using VRC.SDK3.Components;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class VRCMirrorReflectionEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private EnumField cameraClearFlagsField;

        [SerializeField]
        private ColorField customClearColorField;

        [SerializeField]
        private ObjectField customSkyboxField;

        [SerializeField]
        private Toggle turnOffMirrorOcclusionField;

        [SerializeField]
        private Toggle disablePixelLightsField;

        [SerializeField]
        private LayerMaskField reflectLayersField;

        public override Type InspectedType => typeof(VRCMirrorReflection);

        private VRCMirrorReflection target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (VRCMirrorReflection)component;

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            customSkyboxField.SetFieldType(typeof(Material));

            reflectLayersField.SetValueWithoutNotify(target.m_ReflectLayers);

            UpdateFields();
        }

        public void UpdateFields()
        {
            cameraClearFlagsField.SetValueWithoutNotify((int)target.cameraClearFlags);

            customClearColorField.SetValueWithoutNotify(target.customClearColor);

            customSkyboxField.SetValueWithoutNotify(target.customSkybox);

            turnOffMirrorOcclusionField.SetValueWithoutNotify(target.TurnOffMirrorOcclusion);

            disablePixelLightsField.SetValueWithoutNotify(target.m_DisablePixelLights);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnCameraClearFlagsChanged()
        {
            target.cameraClearFlags = (MirrorClearFlags)cameraClearFlagsField.Value;
        }

        public void OnCustomClearColorChanged()
        {
            target.customClearColor = customClearColorField.Value;
        }

        public void OnCustomSkyboxChanged()
        {
            target.customSkybox = (Material)customSkyboxField.Value;
        }

        public void OnTurnOffMirrorOcclusionChanged()
        {
            target.TurnOffMirrorOcclusion = turnOffMirrorOcclusionField.Value;
        }

        public void OnDisablePixelLightsChanged()
        {
            target.m_DisablePixelLights = disablePixelLightsField.Value;
        }

        public void OnReflectLayersChanged()
        {
            target.m_ReflectLayers = reflectLayersField.Value;
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
            if(cameraClearFlagsField) cameraClearFlagsField.RegisterValueChangedCallback(this, nameof(OnCameraClearFlagsChanged));

            if(customClearColorField) customClearColorField.RegisterValueChangedCallback(this, nameof(OnCustomClearColorChanged));

            if(customSkyboxField) customSkyboxField.RegisterValueChangedCallback(this, nameof(OnCustomSkyboxChanged));

            if(turnOffMirrorOcclusionField) turnOffMirrorOcclusionField.RegisterValueChangedCallback(this, nameof(OnTurnOffMirrorOcclusionChanged));

            if(disablePixelLightsField) disablePixelLightsField.RegisterValueChangedCallback(this, nameof(OnDisablePixelLightsChanged));

            if(reflectLayersField) reflectLayersField.RegisterValueChangedCallback(this, nameof(OnReflectLayersChanged));
        }
    }
}
