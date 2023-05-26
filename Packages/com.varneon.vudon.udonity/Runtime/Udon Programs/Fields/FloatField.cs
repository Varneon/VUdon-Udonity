using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class FloatField : Abstract.Field
    {
        [SerializeField]
        private InputField inputField;

        [SerializeField]
        private Slider slider;

        public float Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private float _value;

        private bool ignoreSubmit;

        private bool fieldLockedForEdit;

        public void SetValueWithoutNotify(float value)
        {
            _value = value;

            if (fieldLockedForEdit) { return; }

            ignoreSubmit = true;

            inputField.text = value.ToString();

            if (slider) { slider.SetValueWithoutNotify(value); }
        }

        public void OnBeginEdit()
        {
            fieldLockedForEdit = true;

            ignoreSubmit = false;
        }

        public void OnCancel()
        {
            fieldLockedForEdit = false;

            ignoreSubmit = true;
        }

        public void OnSubmit()
        {
            if (ignoreSubmit) { ignoreSubmit = false; return; }

            float value;

            if (float.TryParse(inputField.text, out value))
            {
                Value = value;
            }

            fieldLockedForEdit = false;

            inputField.OnDeselect(null);
        }

        public void OnSliderUpdated()
        {
            Value = slider.value;
        }
    }
}
