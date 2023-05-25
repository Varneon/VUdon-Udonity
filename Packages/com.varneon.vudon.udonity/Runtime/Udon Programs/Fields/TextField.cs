using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TextField : Abstract.Field
    {
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
    }
}
