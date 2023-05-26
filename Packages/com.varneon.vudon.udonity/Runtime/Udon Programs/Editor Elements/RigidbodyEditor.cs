using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class RigidbodyEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private FloatField massField;

        [SerializeField]
        private FloatField dragField;

        [SerializeField]
        private FloatField angularDragField;

        [SerializeField]
        private Toggle useGravityField;

        [SerializeField]
        private Toggle isKinematicField;

        [SerializeField]
        private EnumField interpolateField;

        [SerializeField]
        private EnumField collisionDetectionField;

        public override Type InspectedType => typeof(Rigidbody);

        private Rigidbody target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (Rigidbody)component;

            massField.RegisterValueChangedCallback(this, nameof(OnMassChanged));

            dragField.RegisterValueChangedCallback(this, nameof(OnDragChanged));

            angularDragField.RegisterValueChangedCallback(this, nameof(OnAngularDragChanged));

            useGravityField.RegisterValueChangedCallback(this, nameof(OnUseGravityChanged));

            isKinematicField.RegisterValueChangedCallback(this, nameof(OnIsKinematicChanged));

            interpolateField.RegisterValueChangedCallback(this, nameof(OnInterpolateChanged));

            collisionDetectionField.RegisterValueChangedCallback(this, nameof(OnCollisionDetectionChanged));

            UpdateFields();
        }

        public void UpdateFields()
        {
            massField.SetValueWithoutNotify(target.mass);

            dragField.SetValueWithoutNotify(target.drag);

            angularDragField.SetValueWithoutNotify(target.angularDrag);

            useGravityField.SetValueWithoutNotify(target.useGravity);

            isKinematicField.SetValueWithoutNotify(target.isKinematic);

            interpolateField.SetValueWithoutNotify((int)target.interpolation);

            collisionDetectionField.SetValueWithoutNotify((int)target.collisionDetectionMode);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnMassChanged()
        {
            target.mass = massField.Value;
        }

        public void OnDragChanged()
        {
            target.drag = dragField.Value;
        }

        public void OnAngularDragChanged()
        {
            target.angularDrag = angularDragField.Value;
        }

        public void OnUseGravityChanged()
        {
            target.useGravity = useGravityField.Value;
        }

        public void OnIsKinematicChanged()
        {
            target.isKinematic = isKinematicField.Value;
        }

        public void OnInterpolateChanged()
        {
            target.interpolation = (RigidbodyInterpolation)interpolateField.Value;
        }

        public void OnCollisionDetectionChanged()
        {
            // UdonBehaviour will crash when trying to cast CollisionDetectionMode

            switch (collisionDetectionField.Value)
            {
                case 0:
                    target.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    return;
                case 1:
                    target.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    return;
                case 2:
                    target.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    return;
                case 3:
                    target.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    return;
            }
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }
    }
}
