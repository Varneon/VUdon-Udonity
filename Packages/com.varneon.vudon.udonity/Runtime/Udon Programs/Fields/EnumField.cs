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
    public class EnumField : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(int);

        [SerializeField]
        private Dropdown dropdown;

        [SerializeField]
        private TextMeshProUGUI label;

        public int Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private int _value;

        public void SetValueWithoutNotify(int value)
        {
            _value = value;

            dropdown.value = value;

            label.text = dropdown.captionText.text;
        }

        public void OnSubmit()
        {
            Value = dropdown.value;
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value == null || !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((int)value);

            return true;
        }
    }
}
