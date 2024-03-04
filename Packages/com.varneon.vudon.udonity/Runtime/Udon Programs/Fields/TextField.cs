using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TextField : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(string);

        [SerializeField]
        private InputField inputField;

        public string Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private string _value;

        private bool ignoreSubmit;

        private bool fieldLockedForEdit;

        public void SetValueWithoutNotify(string value)
        {
            _value = value;

            if (fieldLockedForEdit) { return; }

            ignoreSubmit = true;

            inputField.text = value;
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

            Value = inputField.text;

            fieldLockedForEdit = false;

            inputField.OnDeselect(null);
        }

        protected override void OnInteractiveChanged(bool interactive)
        {
            inputField.interactable = interactive;
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value != null && !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((string)value);

            return true;
        }
    }
}
