using System;
using UnityEngine;
using UnityEngine.Serialization;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class TransformEditor : Abstract.ComponentEditor
    {
        [Header("Transform EditorElement References")]
        [SerializeField]
        [FormerlySerializedAs("positionVector3Field")]
        private Vector3Field localPositionVector3Field;

        [SerializeField]
        private Vector3Field localEulerAnglesVector3Field;

        [SerializeField]
        private Vector3Field localScaleVector3Field;

        public override Type InspectedType => typeof(Transform);

        private Transform target;

        private Vector3 lastLocalPosition;

        private Vector3 lastLocalEulerAngles;

        private Vector3 lastLocalScale;

        protected override void OnEditorInitialized(Component component)
        {
            target = (Transform)component;

            UpdateFields();
        }

        public void UpdateFields()
        {
            Vector3 localPosition = target.localPosition;

            if (lastLocalPosition != localPosition)
            {
                localPositionVector3Field.SetValueWithoutNotify(localPosition);

                lastLocalPosition = localPosition;
            }

            Vector3 localEulerAngles = target.localEulerAngles;

            if(lastLocalEulerAngles != localEulerAngles)
            {
                localEulerAnglesVector3Field.SetValueWithoutNotify(target.localEulerAngles);

                lastLocalEulerAngles = localEulerAngles;
            }

            Vector3 localScale = target.localScale;

            if (lastLocalScale != localScale)
            {
                localScaleVector3Field.SetValueWithoutNotify(localScale);

                lastLocalScale = localScale;
            }

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnPositionChanged()
        {
            target.localPosition = localPositionVector3Field.Value;
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

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnInitializedOnBuild()
        {
            if (localPositionVector3Field) { localPositionVector3Field.RegisterValueChangedCallback(this, nameof(OnPositionChanged)); }

            if (localEulerAnglesVector3Field) { localEulerAnglesVector3Field.RegisterValueChangedCallback(this, nameof(OnRotationChanged)); }

            if (localScaleVector3Field) { localScaleVector3Field.RegisterValueChangedCallback(this, nameof(OnScaleChanged)); }
        }
    }
}
