using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Vector3Field : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(Vector3);

        [SerializeField]
        private InputField inputFieldX, inputFieldY, inputFieldZ;

        [SerializeField]
        private UnityEngine.UI.Toggle constrainProportionsToggle;

        private bool editingX, editingY, editingZ;

        public Vector3 Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Vector3 _value;

        [HideInInspector]
        public bool ConstrainProportions;

        private Vector3 constrainedProportions;

        public void SetValueWithoutNotify(Vector3 value)
        {
            _value = value;

            if (!editingX) { inputFieldX.text = Mathf.DeltaAngle(0f, value.x).ToString(); }

            if (!editingY) { inputFieldY.text = Mathf.DeltaAngle(0f, value.y).ToString(); }

            if (!editingZ) { inputFieldZ.text = Mathf.DeltaAngle(0f, value.z).ToString(); }
        }

        public void BeginEditingX() { ApplyConstrainedProportions(0); editingX = true; }

        public void BeginEditingY() { ApplyConstrainedProportions(1); editingY = true; }

        public void BeginEditingZ() { ApplyConstrainedProportions(2); editingZ = true; }

        public void OnSubmitX()
        {
            float value;

            if (float.TryParse(inputFieldX.text, out value))
            {
                if (ConstrainProportions)
                {
                    Value = new Vector3(value, value * constrainedProportions.y, value * constrainedProportions.z);
                }
                else
                {
                    Vector3 currentValue = Value;

                    currentValue.x = value;

                    Value = currentValue;
                }
            }

            editingX = false;
        }

        public void OnSubmitY()
        {
            float value;

            if (float.TryParse(inputFieldY.text, out value))
            {
                if (ConstrainProportions)
                {
                    Value = new Vector3(value * constrainedProportions.x, value, value * constrainedProportions.z);
                }
                else
                {
                    Vector3 currentValue = Value;

                    currentValue.y = value;

                    Value = currentValue;
                }
            }

            editingY = false;
        }

        public void OnSubmitZ()
        {
            float value;

            if (float.TryParse(inputFieldZ.text, out value))
            {
                if (ConstrainProportions)
                {
                    Value = new Vector3(value * constrainedProportions.x, value * constrainedProportions.y, value);
                }
                else
                {
                    Vector3 currentValue = Value;

                    currentValue.z = value;

                    Value = currentValue;
                }
            }

            editingZ = false;
        }

        private void ApplyConstrainedProportions(int axis)
        {
            float scale = _value[axis];

            constrainedProportions = (scale == 0f) ? Vector3.one : (_value / scale);
        }

        public void OnConstrainProportionsChanged()
        {
            ConstrainProportions = constrainProportionsToggle.isOn;
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value == null || !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((Vector3)value);

            return true;
        }
    }
}
