using System;
using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Toggle : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(bool);

        [SerializeField]
        private UnityEngine.UI.Toggle toggle;

        public bool Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private bool _value;

        public void SetValueWithoutNotify(bool value)
        {
            _value = value;

            toggle.SetIsOnWithoutNotify(value);
        }

        public void OnSubmit()
        {
            Value = toggle.isOn;
        }

        protected override void OnInteractiveChanged(bool interactive)
        {
            toggle.interactable = interactive;
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value == null || !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((bool)value);

            return true;
        }
    }
}
