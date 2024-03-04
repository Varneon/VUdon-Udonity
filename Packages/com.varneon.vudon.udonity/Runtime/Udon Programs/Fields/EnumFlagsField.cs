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
    public class EnumFlagsField : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(int);

        [SerializeField]
        private Dropdown dropdown;

        [SerializeField]
        private TextMeshProUGUI label;

        [SerializeField]
        private int itemCount;

        public int Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private int _value;

        private int flagCount;

        public void SetValueWithoutNotify(int value)
        {
            _value = value;

            flagCount = 0;

            int lastSetBit = -1;

            for(int i = 1; i < itemCount + 1; i++)
            {
                if ((_value & (1 << i)) != 0)
                {
                    lastSetBit = i;

                    flagCount++;
                }
            }

            switch (flagCount)
            {
                case 0: label.text = "Nothing"; break;
                case 1:
                    dropdown.SetValueWithoutNotify(lastSetBit + 1);
                    label.text = dropdown.captionText.text;
                    break;
                default: label.text = flagCount == itemCount ? "Everything" : "Mixed..."; break;
            }
        }

        public void OnDropdownOpened()
        {
            UnityEngine.UI.Toggle[] toggles = dropdown.transform.Find("Dropdown List").GetComponentsInChildren<UnityEngine.UI.Toggle>();

            if (flagCount == 0) { toggles[0].graphic.enabled = true; }
            else if (flagCount == itemCount) { toggles[1].graphic.enabled = true; }

            for (int i = 2; i < toggles.Length; i++)
            {
                bool set = (_value & (1 << (i - 1))) != 0;

                UnityEngine.UI.Toggle toggle = toggles[i];

                toggle.SetIsOnWithoutNotify(set);
                toggle.graphic.enabled = set;
            }
        }

        public bool IsFlagSet(int flag)
        {
            return (_value & flag) != 0;
        }

        public bool IsFlagSetAtIndex(int index)
        {
            return (_value & (1 << (index + 1))) != 0;
        }

        public void OnSubmit()
        {
            int dropdownValue = dropdown.value;

            if(dropdownValue == 0)
            {
                Value = 0;
            }
            else if (dropdownValue == 1)
            {
                Value = ~0;
            }
            else
            {
                int mask = 1 << (dropdownValue - 1);

                if ((_value & (mask)) != 0)
                {
                    Value &= ~mask;
                }
                else
                {
                    Value |= mask;
                }
            }
        }

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value == null || !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((int)value);

            return true;
        }
    }
}
