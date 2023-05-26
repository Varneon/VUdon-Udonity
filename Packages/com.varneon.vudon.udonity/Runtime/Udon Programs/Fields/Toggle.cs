using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Toggle : Abstract.Field
    {
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
    }
}
