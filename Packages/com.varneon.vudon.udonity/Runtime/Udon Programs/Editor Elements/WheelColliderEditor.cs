using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class WheelColliderEditor : Abstract.ComponentEditor
    {
        public override System.Type InspectedType => typeof(WheelCollider);

        [SerializeField]
        private FloatField massField;

        [SerializeField]
        private FloatField radiusField;

        [SerializeField]
        private FloatField wheelDampingRateField;

        [SerializeField]
        private FloatField suspensionDistanceField;

        [SerializeField]
        private FloatField forceAppPointDistanceField;

        [SerializeField]
        private Vector3Field centerField;

        [SerializeField]
        private FloatField springField;

        [SerializeField]
        private FloatField damperField;

        [SerializeField]
        private FloatField targetPositionField;

        [SerializeField]
        private FloatField forwardExtremumSlipField;

        [SerializeField]
        private FloatField forwardExtremumValueField;

        [SerializeField]
        private FloatField forwardAsymptoteSlipField;

        [SerializeField]
        private FloatField forwardAsymptoteValueField;

        [SerializeField]
        private FloatField forwardStiffnessField;

        [SerializeField]
        private FloatField sidewaysExtremumSlipField;

        [SerializeField]
        private FloatField sidewaysExtremumValueField;

        [SerializeField]
        private FloatField sidewaysAsymptoteSlipField;

        [SerializeField]
        private FloatField sidewaysAsymptoteValueField;

        [SerializeField]
        private FloatField sidewaysStiffnessField;

        private WheelCollider target;

        private JointSpring suspensionSpring;

        private WheelFrictionCurve forwardFriction;

        private WheelFrictionCurve sidewaysFriction;

        protected override void OnEditorInitialized(Component component)
        {
            target = (WheelCollider)component;

            massField.RegisterValueChangedCallback(this, nameof(OnMassChanged));

            radiusField.RegisterValueChangedCallback(this, nameof(OnRadiusChanged));

            wheelDampingRateField.RegisterValueChangedCallback(this, nameof(OnWheelDampingRateChanged));

            suspensionDistanceField.RegisterValueChangedCallback(this, nameof(OnSuspensionDistanceChanged));

            forceAppPointDistanceField.RegisterValueChangedCallback(this, nameof(OnForceAppPointDistanceChanged));

            centerField.RegisterValueChangedCallback(this, nameof(OnCenterChanged));

            springField.RegisterValueChangedCallback(this, nameof(OnSpringChanged));

            damperField.RegisterValueChangedCallback(this, nameof(OnDamperChanged));

            targetPositionField.RegisterValueChangedCallback(this, nameof(OnTargetPositionChanged));

            forwardExtremumSlipField.RegisterValueChangedCallback(this, nameof(OnForwardExtremumSlipChanged));

            forwardExtremumValueField.RegisterValueChangedCallback(this, nameof(OnForwardExtremumValueChanged));

            forwardAsymptoteSlipField.RegisterValueChangedCallback(this, nameof(OnForwardAsymptoteSlipChanged));

            forwardAsymptoteValueField.RegisterValueChangedCallback(this, nameof(OnForwardAsymptoteValueChanged));

            forwardStiffnessField.RegisterValueChangedCallback(this, nameof(OnForwardStiffnessChanged));

            sidewaysExtremumSlipField.RegisterValueChangedCallback(this, nameof(OnSidewaysExtremumSlipChanged));

            sidewaysExtremumValueField.RegisterValueChangedCallback(this, nameof(OnSidewaysExtremumValueChanged));

            sidewaysAsymptoteSlipField.RegisterValueChangedCallback(this, nameof(OnSidewaysAsymptoteSlipChanged));

            sidewaysAsymptoteValueField.RegisterValueChangedCallback(this, nameof(OnSidewaysAsymptoteValueChanged));

            sidewaysStiffnessField.RegisterValueChangedCallback(this, nameof(OnSidewaysStiffnessChanged));

            UpdateSuspensionSpring();

            UpdateForwardFriction();

            UpdateSidewaysFriction();

            UpdateFields();
        }

        public void UpdateFields()
        {
            UpdateSuspensionSpring();

            UpdateForwardFriction();

            UpdateSidewaysFriction();

            massField.SetValueWithoutNotify(target.mass);

            radiusField.SetValueWithoutNotify(target.radius);

            wheelDampingRateField.SetValueWithoutNotify(target.wheelDampingRate);

            suspensionDistanceField.SetValueWithoutNotify(target.suspensionDistance);

            forceAppPointDistanceField.SetValueWithoutNotify(target.forceAppPointDistance);

            centerField.SetValueWithoutNotify(target.center);

            springField.SetValueWithoutNotify(suspensionSpring.spring);

            damperField.SetValueWithoutNotify(suspensionSpring.damper);

            targetPositionField.SetValueWithoutNotify(suspensionSpring.targetPosition);

            forwardExtremumSlipField.SetValueWithoutNotify(forwardFriction.extremumSlip);

            forwardExtremumValueField.SetValueWithoutNotify(forwardFriction.extremumValue);

            forwardAsymptoteSlipField.SetValueWithoutNotify(forwardFriction.asymptoteSlip);

            forwardAsymptoteValueField.SetValueWithoutNotify(forwardFriction.asymptoteValue);

            forwardStiffnessField.SetValueWithoutNotify(forwardFriction.stiffness);

            sidewaysExtremumSlipField.SetValueWithoutNotify(sidewaysFriction.extremumSlip);

            sidewaysExtremumValueField.SetValueWithoutNotify(sidewaysFriction.extremumValue);

            sidewaysAsymptoteSlipField.SetValueWithoutNotify(sidewaysFriction.asymptoteSlip);

            sidewaysAsymptoteValueField.SetValueWithoutNotify(sidewaysFriction.asymptoteValue);

            sidewaysStiffnessField.SetValueWithoutNotify(sidewaysFriction.stiffness);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnMassChanged()
        {
            target.mass = massField.Value;
        }

        public void OnRadiusChanged()
        {
            target.radius = radiusField.Value;
        }

        public void OnWheelDampingRateChanged()
        {
            target.wheelDampingRate = wheelDampingRateField.Value;
        }

        public void OnSuspensionDistanceChanged()
        {
            target.suspensionDistance = suspensionDistanceField.Value;
        }

        public void OnForceAppPointDistanceChanged()
        {
            target.forceAppPointDistance = forceAppPointDistanceField.Value;
        }

        public void OnCenterChanged()
        {
            target.center = centerField.Value;
        }

        public void OnSpringChanged()
        {
            suspensionSpring.spring = springField.Value;

            ApplySuspensionSpring();
        }

        public void OnDamperChanged()
        {
            suspensionSpring.damper = damperField.Value;

            ApplySuspensionSpring();
        }

        public void OnTargetPositionChanged()
        {
            suspensionSpring.targetPosition = targetPositionField.Value;

            ApplySuspensionSpring();
        }

        public void OnForwardExtremumSlipChanged()
        {
            forwardFriction.extremumSlip = forwardExtremumSlipField.Value;

            ApplyForwardFriction();
        }

        public void OnForwardExtremumValueChanged()
        {
            forwardFriction.extremumValue = forwardExtremumValueField.Value;

            ApplyForwardFriction();
        }

        public void OnForwardAsymptoteSlipChanged()
        {
            forwardFriction.asymptoteSlip = forwardAsymptoteSlipField.Value;

            ApplyForwardFriction();
        }

        public void OnForwardAsymptoteValueChanged()
        {
            forwardFriction.asymptoteValue = forwardAsymptoteValueField.Value;

            ApplyForwardFriction();
        }

        public void OnForwardStiffnessChanged()
        {
            forwardFriction.stiffness = forwardStiffnessField.Value;

            ApplyForwardFriction();
        }

        public void OnSidewaysExtremumSlipChanged()
        {
            sidewaysFriction.extremumSlip = sidewaysExtremumSlipField.Value;

            ApplySidewaysFriction();
        }

        public void OnSidewaysExtremumValueChanged()
        {
            sidewaysFriction.extremumValue = sidewaysExtremumValueField.Value;

            ApplySidewaysFriction();
        }

        public void OnSidewaysAsymptoteSlipChanged()
        {
            sidewaysFriction.asymptoteSlip = sidewaysAsymptoteSlipField.Value;

            ApplySidewaysFriction();
        }

        public void OnSidewaysAsymptoteValueChanged()
        {
            sidewaysFriction.asymptoteValue = sidewaysAsymptoteValueField.Value;

            ApplySidewaysFriction();
        }

        public void OnSidewaysStiffnessChanged()
        {
            sidewaysFriction.stiffness = sidewaysStiffnessField.Value;

            ApplySidewaysFriction();
        }

        private void UpdateSuspensionSpring() => suspensionSpring = target.suspensionSpring;

        private void UpdateForwardFriction() => forwardFriction = target.forwardFriction;

        private void UpdateSidewaysFriction() => sidewaysFriction = target.sidewaysFriction;

        private void ApplySuspensionSpring() => target.suspensionSpring = suspensionSpring;

        private void ApplyForwardFriction() => target.forwardFriction = forwardFriction;

        private void ApplySidewaysFriction() => target.sidewaysFriction = sidewaysFriction;
    }
}
