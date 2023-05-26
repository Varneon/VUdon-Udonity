using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class TransformEditor : Abstract.ComponentEditor
    {
        [Header("Transform EditorElement References")]
        [SerializeField]
        private Vector3Field positionVector3Field;

        [SerializeField]
        private Vector3Field localEulerAnglesVector3Field;

        [SerializeField]
        private Vector3Field localScaleVector3Field;

        public override Type InspectedType => typeof(Transform);

        private Transform target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (Transform)component;

            UpdateFields();
        }

        public void UpdateFields()
        {
            positionVector3Field.SetValueWithoutNotify(target.localPosition);

            localEulerAnglesVector3Field.SetValueWithoutNotify(target.localEulerAngles);

            localScaleVector3Field.SetValueWithoutNotify(target.localScale);

            SendCustomEventDelayedFrames(nameof(UpdateFields), 0);
        }

        public void OnPositionChanged()
        {
            target.localPosition = positionVector3Field.Value;
        }

        public void OnRotationChanged()
        {
            target.localEulerAngles = localEulerAnglesVector3Field.Value;
        }

        public void OnScaleChanged()
        {
            target.localScale = localScaleVector3Field.Value;
        }

        protected override void OnContextDropdownActionInvoked(int index)
        {
            switch (index)
            {
                case 0:
                    ResetTransform();
                    break;
                case 1:
                    ResetTransformPosition();
                    break;
                case 2:
                    ResetTransformRotation();
                    break;
                case 3:
                    ResetTransformScale();
                    break;
            }
        }

        private void ResetTransform()
        {
            ResetTransformPosition();
            ResetTransformRotation();
            ResetTransformScale();
        }

        private void ResetTransformPosition()
        {
            target.localPosition = Vector3.zero;
        }

        private void ResetTransformRotation()
        {
            target.localEulerAngles = Vector3.zero;
        }

        private void ResetTransformScale()
        {
            target.localScale = Vector3.one;
        }

        protected override void OnInitializedOnBuild()
        {
            if (positionVector3Field) { positionVector3Field.RegisterValueChangedCallback(this, nameof(OnPositionChanged)); }

            if (localEulerAnglesVector3Field) { localEulerAnglesVector3Field.RegisterValueChangedCallback(this, nameof(OnRotationChanged)); }

            if (localScaleVector3Field) { localScaleVector3Field.RegisterValueChangedCallback(this, nameof(OnScaleChanged)); }
        }
    }
}
