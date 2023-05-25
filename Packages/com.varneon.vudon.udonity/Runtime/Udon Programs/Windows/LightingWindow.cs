using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LightingWindow : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(300f, 300f);

        [SerializeField]
        private ObjectField skyboxMaterialField;

        [SerializeField]
        private ObjectField sunSourceField;

        [SerializeField]
        private EnumField ambientModeField;

        [SerializeField]
        private ColorField
            ambientSkyColorField,
            ambientEquatorColorField,
            ambientGroundColorField,
            ambientColorField;

        [SerializeField]
        private FloatField ambientIntensityField;

        [SerializeField]
        private Toggle fogToggle;

        [SerializeField]
        private ColorField fogColorField;

        [SerializeField]
        private EnumField fogModeField;

        [SerializeField]
        private FloatField fogDensityField;

        [SerializeField]
        private FloatField fogStartField;

        [SerializeField]
        private FloatField fogEndField;

        [SerializeField]
        private FloatField haloStrengthField;

        [SerializeField]
        private FloatField flareFadeSpeedField;

        [SerializeField]
        private FloatField flareStrengthField;

        private void Start()
        {
            skyboxMaterialField.SetFieldType(typeof(Material));

            skyboxMaterialField.SetValueWithoutNotify(RenderSettings.skybox);

            sunSourceField.SetFieldType(typeof(Light));

            sunSourceField.SetValueWithoutNotify(RenderSettings.sun);

            ambientModeField.SetValueWithoutNotify((int)RenderSettings.ambientMode);

            ambientModeField.RegisterValueChangedCallback(this, nameof(OnAmbientModeChanged));

            //SKYBOX:

            ambientIntensityField.Value = RenderSettings.ambientIntensity;

            //GRADIENT:

            ambientSkyColorField.Value = RenderSettings.ambientSkyColor;
            ambientEquatorColorField.Value = RenderSettings.ambientEquatorColor;
            ambientGroundColorField.Value = RenderSettings.ambientGroundColor;

            //COLOR:

            ambientColorField.Value = RenderSettings.ambientSkyColor;

            ambientIntensityField.RegisterValueChangedCallback(this, nameof(OnAmbientIntensityChanged));

            ambientSkyColorField.RegisterValueChangedCallback(this, nameof(OnAmbientSkyColorChanged));

            ambientEquatorColorField.RegisterValueChangedCallback(this, nameof(OnAmbientEquatorColorChanged));

            ambientGroundColorField.RegisterValueChangedCallback(this, nameof(OnAmbientGroundColorChanged));

            ambientColorField.RegisterValueChangedCallback(this, nameof(OnAmbientColorChanged));

            fogToggle.SetValueWithoutNotify(RenderSettings.fog);

            fogToggle.RegisterValueChangedCallback(this, nameof(ToggleFog));

            fogColorField.SetValueWithoutNotify(RenderSettings.fogColor);

            fogColorField.RegisterValueChangedCallback(this, nameof(OnFogColorChanged));

            fogModeField.SetValueWithoutNotify((int)RenderSettings.fogMode - 1); // FogMode enum starts at 1

            fogModeField.RegisterValueChangedCallback(this, nameof(OnFogModeChanged));

            fogDensityField.SetValueWithoutNotify(RenderSettings.fogDensity);

            fogDensityField.RegisterValueChangedCallback(this, nameof(OnFogDensityChanged));

            fogStartField.SetValueWithoutNotify(RenderSettings.fogStartDistance);

            fogStartField.RegisterValueChangedCallback(this, nameof(OnFogStartChanged));

            fogEndField.SetValueWithoutNotify(RenderSettings.fogEndDistance);

            fogEndField.RegisterValueChangedCallback(this, nameof(OnFogEndChanged));

            haloStrengthField.SetValueWithoutNotify(RenderSettings.haloStrength);

            flareFadeSpeedField.SetValueWithoutNotify(RenderSettings.flareFadeSpeed);

            flareStrengthField.SetValueWithoutNotify(RenderSettings.flareStrength);

            SetEnvironmentLightingFieldsEnabled();

            SetFogFieldsEnabled();
        }

        public void Refresh()
        {
            skyboxMaterialField.SetValueWithoutNotify(RenderSettings.skybox);

            sunSourceField.SetValueWithoutNotify(RenderSettings.sun);

            ambientModeField.SetValueWithoutNotify((int)RenderSettings.ambientMode);

            ambientIntensityField.SetValueWithoutNotify(RenderSettings.ambientIntensity);

            ambientSkyColorField.SetValueWithoutNotify(RenderSettings.ambientSkyColor);

            ambientEquatorColorField.SetValueWithoutNotify(RenderSettings.ambientEquatorColor);

            ambientGroundColorField.SetValueWithoutNotify(RenderSettings.ambientGroundColor);

            ambientColorField.SetValueWithoutNotify(RenderSettings.ambientSkyColor);

            fogToggle.SetValueWithoutNotify(RenderSettings.fog);

            fogColorField.SetValueWithoutNotify(RenderSettings.fogColor);

            fogModeField.SetValueWithoutNotify((int)RenderSettings.fogMode - 1); // FogMode enum starts at 1

            fogDensityField.SetValueWithoutNotify(RenderSettings.fogDensity);

            fogStartField.SetValueWithoutNotify(RenderSettings.fogStartDistance);

            fogEndField.SetValueWithoutNotify(RenderSettings.fogEndDistance);

            haloStrengthField.SetValueWithoutNotify(RenderSettings.haloStrength);

            flareFadeSpeedField.SetValueWithoutNotify(RenderSettings.flareFadeSpeed);

            flareStrengthField.SetValueWithoutNotify(RenderSettings.flareStrength);
        }

        public void OnAmbientModeChanged()
        {
            RenderSettings.ambientMode = EnumResolver.UnityEngine_Rendering_AmbientMode.FromInt(ambientModeField.Value);

            SetEnvironmentLightingFieldsEnabled();
        }

        public void OnAmbientIntensityChanged()
        {
            RenderSettings.ambientIntensity = ambientIntensityField.Value;
        }

        public void OnAmbientSkyColorChanged()
        {
            RenderSettings.ambientSkyColor = ambientSkyColorField.Value;
        }

        public void OnAmbientEquatorColorChanged()
        {
            RenderSettings.ambientEquatorColor = ambientEquatorColorField.Value;
        }

        public void OnAmbientGroundColorChanged()
        {
            RenderSettings.ambientGroundColor = ambientGroundColorField.Value;
        }

        public void OnAmbientColorChanged()
        {
            RenderSettings.ambientSkyColor = ambientColorField.Value;
        }

        public void ToggleFog()
        {
            RenderSettings.fog = fogToggle.Value;

            SetFogFieldsEnabled();
        }

        public void OnFogModeChanged()
        {
            switch (fogModeField.Value)
            {
                case 0:
                    RenderSettings.fogMode = FogMode.Linear;
                    break;
                case 1:
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;
                case 2:
                    RenderSettings.fogMode = FogMode.ExponentialSquared;
                    break;
            }

            SetFogFieldsEnabled();
        }

        public void OnFogColorChanged()
        {
            RenderSettings.fogColor = fogColorField.Value;
        }

        public void OnFogDensityChanged()
        {
            RenderSettings.fogDensity = fogDensityField.Value;
        }

        public void OnFogStartChanged()
        {
            RenderSettings.fogStartDistance = fogStartField.Value;
        }

        public void OnFogEndChanged()
        {
            RenderSettings.fogEndDistance = fogEndField.Value;
        }

        private void SetFogFieldsEnabled()
        {
            bool fog = RenderSettings.fog;

            FogMode fogMode = RenderSettings.fogMode;

            bool linear = fogMode == FogMode.Linear;

            fogColorField.gameObject.SetActive(fog);
            fogModeField.gameObject.SetActive(fog);
            fogDensityField.gameObject.SetActive(fog && !linear);
            fogStartField.gameObject.SetActive(fog && linear);
            fogEndField.gameObject.SetActive(fog && linear);

            RebuildLayout();
        }

        private void SetEnvironmentLightingFieldsEnabled()
        {
            AmbientMode mode = RenderSettings.ambientMode;

            bool isGradient = mode == AmbientMode.Trilight;

            ambientIntensityField.gameObject.SetActive(mode == AmbientMode.Skybox);
            ambientSkyColorField.gameObject.SetActive(isGradient);
            ambientEquatorColorField.gameObject.SetActive(isGradient);
            ambientGroundColorField.gameObject.SetActive(isGradient);
            ambientColorField.gameObject.SetActive(mode == AmbientMode.Flat);

            RebuildLayout();
        }

        private void RebuildLayout()
        {
            foreach(UnityEngine.UI.LayoutGroup layoutGroup in GetComponentsInChildren<UnityEngine.UI.LayoutGroup>(true))
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }
    }
}
