using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class RectTransformEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private Vector3Field positionVector3Field;

        [SerializeField]
        private Vector3Field localEulerAnglesVector3Field;

        [SerializeField]
        private Vector3Field localScaleVector3Field;

        [SerializeField]
        private Vector2Field anchorMinField;

        [SerializeField]
        private Vector2Field anchorMaxField;

        [SerializeField]
        private Vector2Field anchoredPositionField;

        [SerializeField]
        private Vector2Field sizeDeltaField;

        [SerializeField]
        private Vector2Field pivotField;

        public override Type InspectedType => typeof(RectTransform);

        private RectTransform target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (RectTransform)component;

            positionVector3Field.RegisterValueChangedCallback(this, nameof(OnPositionChanged));

            localEulerAnglesVector3Field.RegisterValueChangedCallback(this, nameof(OnRotationChanged));

            localScaleVector3Field.RegisterValueChangedCallback(this, nameof(OnScaleChanged));

            anchorMinField.RegisterValueChangedCallback(this, nameof(OnAnchorMinChanged));

            anchorMaxField.RegisterValueChangedCallback(this, nameof(OnAnchorMaxChanged));

            anchoredPositionField.RegisterValueChangedCallback(this, nameof(OnAnchoredPositionChanged));

            sizeDeltaField.RegisterValueChangedCallback(this, nameof(OnSizeDeltaChanged));

            pivotField.RegisterValueChangedCallback(this, nameof(OnPivotChanged));

            UpdateFields();
        }

        public void UpdateFields()
        {
            positionVector3Field.SetValueWithoutNotify(target.localPosition);

            localEulerAnglesVector3Field.SetValueWithoutNotify(target.localEulerAngles);

            localScaleVector3Field.SetValueWithoutNotify(target.localScale);

            anchorMinField.SetValueWithoutNotify(target.anchorMin);

            anchorMaxField.SetValueWithoutNotify(target.anchorMax);

            anchoredPositionField.SetValueWithoutNotify(target.anchoredPosition);

            sizeDeltaField.SetValueWithoutNotify(target.sizeDelta);

            pivotField.SetValueWithoutNotify(target.pivot);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
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

        public void OnAnchorMinChanged()
        {
            target.anchorMin = anchorMinField.Value;
        }

        public void OnAnchorMaxChanged()
        {
            target.anchorMax = anchorMaxField.Value;
        }

        public void OnAnchoredPositionChanged()
        {
            target.anchoredPosition = anchoredPositionField.Value;
        }

        public void OnSizeDeltaChanged()
        {
            target.sizeDelta = sizeDeltaField.Value;
        }

        public void OnPivotChanged()
        {
            target.pivot = pivotField.Value;
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
    }
}
