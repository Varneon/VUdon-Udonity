using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Vector2Field : Abstract.Field
    {
        [SerializeField]
        private InputField inputFieldX, inputFieldY;

        private bool editingX, editingY;

        public Vector2 Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Vector2 _value;

        public void SetValueWithoutNotify(Vector2 value)
        {
            _value = value;

            if (!editingX) { inputFieldX.text = Mathf.DeltaAngle(0f, value.x).ToString(); }

            if (!editingY) { inputFieldY.text = Mathf.DeltaAngle(0f, value.y).ToString(); }
        }

        public void BeginEditingX() { editingX = true; }

        public void BeginEditingY() { editingY = true; }

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
    }
}
