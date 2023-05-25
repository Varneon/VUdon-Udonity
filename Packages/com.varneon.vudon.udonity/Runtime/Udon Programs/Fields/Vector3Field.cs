using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Vector3Field : Abstract.Field
    {
        [SerializeField]
        private InputField inputFieldX, inputFieldY, inputFieldZ;

        private bool editingX, editingY, editingZ;

        public Vector3 Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Vector3 _value;

        public void SetValueWithoutNotify(Vector3 value)
        {
            _value = value;

            if (!editingX) { inputFieldX.text = Mathf.DeltaAngle(0f, value.x).ToString(); }

            if (!editingY) { inputFieldY.text = Mathf.DeltaAngle(0f, value.y).ToString(); }

            if (!editingZ) { inputFieldZ.text = Mathf.DeltaAngle(0f, value.z).ToString(); }
        }

        public void BeginEditingX() { editingX = true; }

        public void BeginEditingY() { editingY = true; }

        public void BeginEditingZ() { editingZ = true; }

        public void OnSubmitX()
        {
            float value;

            if (float.TryParse(inputFieldX.text, out value))
            {
                Vector3 currentValue = Value;

                currentValue.x = value;

                Value = currentValue;
            }

            editingX = false;
        }

        public void OnSubmitY()
        {
            float value;

            if (float.TryParse(inputFieldY.text, out value))
            {
                Vector3 currentValue = Value;

                currentValue.y = value;

                Value = currentValue;
            }

            editingY = false;
        }

        public void OnSubmitZ()
        {
            float value;

            if (float.TryParse(inputFieldZ.text, out value))
            {
                Vector3 currentValue = Value;

                currentValue.z = value;

                Value = currentValue;
            }

            editingZ = false;
        }
    }
}
