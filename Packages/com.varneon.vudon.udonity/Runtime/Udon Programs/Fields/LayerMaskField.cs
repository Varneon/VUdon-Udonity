using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LayerMaskField : Abstract.Field
    {
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
    }
}
