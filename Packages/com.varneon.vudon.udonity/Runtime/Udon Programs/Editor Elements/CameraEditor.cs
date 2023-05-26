using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class CameraEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private EnumField clearFlagsField;

        [SerializeField]
        private ColorField backgroundField;

        [SerializeField]
        private LayerMaskField cullingMaskField;

        [SerializeField]
        private Toggle orthographicField;

        [SerializeField]
        private FloatField fovField;

        [SerializeField]
        private FloatField sizeField;

        [SerializeField]
        private Toggle physicalCameraField;

        [SerializeField]
        private FloatField nearClippingPlaneField;

        [SerializeField]
        private FloatField farClippingPlaneField;

        [SerializeField]
        private FloatField depthField;

        [SerializeField]
        private EnumField renderingPathField;

        [SerializeField]
        private ObjectField targetTextureField;

        [SerializeField]
        private Toggle occlusionCullingField;

        [SerializeField]
        private Toggle hdrField;

        [SerializeField]
        private Toggle msaaField;

        [SerializeField]
        private Toggle allowDynamicResolutionField;

        [SerializeField]
        private FloatField stereoSeparationField;

        [SerializeField]
        private FloatField stereoConvergenceField;

        [SerializeField]
        private EnumField targetDisplayField;

        [SerializeField]
        private EnumField targetEyeField;

        public override Type InspectedType => typeof(Camera);

        private Camera target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (Camera)component;

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            clearFlagsField.RegisterValueChangedCallback(this, nameof(OnClearFlagsChanged));

            backgroundField.RegisterValueChangedCallback(this, nameof(OnBackgroundColorChanged));

            cullingMaskField.RegisterValueChangedCallback(this, nameof(OnCullingMaskChanged));

            orthographicField.RegisterValueChangedCallback(this, nameof(OnOrthographicChanged));

            fovField.RegisterValueChangedCallback(this, nameof(OnFOVChanged));

            sizeField.RegisterValueChangedCallback(this, nameof(OnSizeChanged));

            physicalCameraField.RegisterValueChangedCallback(this, nameof(OnPhysicalCameraChanged));

            nearClippingPlaneField.RegisterValueChangedCallback(this, nameof(OnClippingPlaneNearChanged));

            farClippingPlaneField.RegisterValueChangedCallback(this, nameof(OnClippingPlaneFarChanged));

            depthField.RegisterValueChangedCallback(this, nameof(OnDepthChanged));

            renderingPathField.RegisterValueChangedCallback(this, nameof(OnRenderingPathChanged));

            occlusionCullingField.RegisterValueChangedCallback(this, nameof(OnOcclusionCullingChanged));

            hdrField.RegisterValueChangedCallback(this, nameof(OnHDRChanged));

            msaaField.RegisterValueChangedCallback(this, nameof(OnMSAAChanged));

            allowDynamicResolutionField.RegisterValueChangedCallback(this, nameof(OnAllowDynamicResolutionChanged));

            stereoSeparationField.RegisterValueChangedCallback(this, nameof(OnStereoSeparationChanged));

            stereoConvergenceField.RegisterValueChangedCallback(this, nameof(OnStereoConvergenceChanged));

            targetEyeField.RegisterValueChangedCallback(this, nameof(OnTargetEyeChanged));

            // LayerMaskDropdowns don't work with per-frame updates yet
            cullingMaskField.SetValueWithoutNotify(target.cullingMask);

            targetTextureField.SetFieldType(typeof(RenderTexture));

            SetProjectionFieldsActive();

            UpdateFields();
        }

        public void UpdateFields()
        {
            clearFlagsField.SetValueWithoutNotify(EnumResolver.UnityEngine_CameraClearFlags.ToInt(target.clearFlags));

            backgroundField.SetValueWithoutNotify(target.backgroundColor);

            orthographicField.SetValueWithoutNotify(target.orthographic);

            fovField.SetValueWithoutNotify(target.fieldOfView);

            sizeField.SetValueWithoutNotify(target.orthographicSize);

            physicalCameraField.SetValueWithoutNotify(target.usePhysicalProperties);

            nearClippingPlaneField.SetValueWithoutNotify(target.nearClipPlane);

            farClippingPlaneField.SetValueWithoutNotify(target.farClipPlane);

            depthField.SetValueWithoutNotify(target.depth);

            renderingPathField.SetValueWithoutNotify(EnumResolver.UnityEngine_RenderingPath.ToInt(target.renderingPath));

            targetTextureField.SetValueWithoutNotify(target.targetTexture);

            occlusionCullingField.SetValueWithoutNotify(target.useOcclusionCulling);

            hdrField.SetValueWithoutNotify(target.allowHDR);

            msaaField.SetValueWithoutNotify(target.allowMSAA);

            allowDynamicResolutionField.SetValueWithoutNotify(target.allowDynamicResolution);

            stereoSeparationField.SetValueWithoutNotify(target.stereoSeparation);

            stereoConvergenceField.SetValueWithoutNotify(target.stereoConvergence);

            targetDisplayField.SetValueWithoutNotify(target.targetDisplay);

            targetEyeField.SetValueWithoutNotify(EnumResolver.UnityEngine_StereoTargetEyeMask.ToInt(target.stereoTargetEye));

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnClearFlagsChanged()
        {
            target.clearFlags = EnumResolver.UnityEngine_CameraClearFlags.FromInt(clearFlagsField.Value);
        }

        public void OnBackgroundColorChanged()
        {
            target.backgroundColor = backgroundField.Value;
        }

        public void OnCullingMaskChanged()
        {
            target.cullingMask = cullingMaskField.Value;
        }

        public void OnOrthographicChanged()
        {
            target.orthographic = orthographicField.Value;

            SetProjectionFieldsActive();
        }

        public void OnFOVChanged()
        {
            target.fieldOfView = fovField.Value;
        }

        public void OnSizeChanged()
        {
            target.orthographicSize = sizeField.Value;
        }

        public void OnPhysicalCameraChanged()
        {
            target.usePhysicalProperties = physicalCameraField.Value;
        }

        public void OnClippingPlaneNearChanged()
        {
            target.nearClipPlane = nearClippingPlaneField.Value;
        }

        public void OnClippingPlaneFarChanged()
        {
            target.farClipPlane = farClippingPlaneField.Value;
        }

        public void OnDepthChanged()
        {
            target.depth = depthField.Value;
        }

        public void OnRenderingPathChanged()
        {
            target.renderingPath = EnumResolver.UnityEngine_RenderingPath.FromInt(renderingPathField.Value);
        }

        public void OnOcclusionCullingChanged()
        {
            target.useOcclusionCulling = occlusionCullingField.Value;
        }

        public void OnHDRChanged()
        {
            target.allowHDR = hdrField.Value;
        }

        public void OnMSAAChanged()
        {
            target.allowMSAA = msaaField.Value;
        }

        public void OnAllowDynamicResolutionChanged()
        {
            target.allowDynamicResolution = allowDynamicResolutionField.Value;
        }

        public void OnStereoSeparationChanged()
        {
            target.stereoSeparation = stereoSeparationField.Value;
        }

        public void OnStereoConvergenceChanged()
        {
            target.stereoConvergence = stereoConvergenceField.Value;
        }

        public void OnTargetDisplayChanged()
        {
            target.targetDisplay = targetDisplayField.Value;
        }

        public void OnTargetEyeChanged()
        {
            // The updated value doesn't automatically refresh on Unity Editor's inspector: https://github.com/Varneon/VUdon-Udonity/wiki/Developer-Notes#cameraeditor
            target.stereoTargetEye = EnumResolver.UnityEngine_StereoTargetEyeMask.FromInt(targetEyeField.Value);
        }

        private void SetProjectionFieldsActive()
        {
            bool orthographic = target.orthographic;

            fovField.gameObject.SetActive(!orthographic);
            physicalCameraField.gameObject.SetActive(!orthographic);
            sizeField.gameObject.SetActive(orthographic);

            containingInspector.RebuildLayout();
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
