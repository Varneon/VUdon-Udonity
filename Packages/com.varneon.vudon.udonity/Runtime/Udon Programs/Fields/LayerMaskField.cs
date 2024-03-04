using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LayerMaskField : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(LayerMask);

        [SerializeField]
        private Dropdown backingDropdown;

        [SerializeField]
        private LayerMaskDropdown.LayerMaskDropdown dropdown;

        [SerializeField]
        private TextMeshProUGUI label;

        public LayerMask Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private LayerMask _value;

        public void SetValueWithoutNotify(LayerMask value)
        {
            _value = value;

            dropdown.SetValueWithoutNotify(value);

            label.text = backingDropdown.captionText.text;
        }

        public void OnSubmit()
        {
            Value = dropdown.Value;
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value == null || (!value.GetType().Equals(FieldType) || !value.GetType().Equals(typeof(int)))) { return false; }

            SetValueWithoutNotify((int)value);

            return true;
        }
    }
}
