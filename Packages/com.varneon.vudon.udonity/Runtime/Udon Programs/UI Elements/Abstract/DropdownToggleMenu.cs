using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements.Abstract
{
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class DropdownToggleMenu : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector]
        private Dropdown dropdown;

        public void OnMenuOptionToggled()
        {
            int dropdownValue = dropdown.value;

            int mask = 1 << (dropdownValue + 1);

            bool value = (_mask & (mask)) != 0;

            if (value)
            {
                _mask &= ~mask;
            }
            else
            {
                _mask |= mask;
            }

            OnMenuOptionToggled(dropdownValue, !value);
        }

        internal void InitializeOnBuild()
        {
            dropdown = GetComponentInChildren<Dropdown>(true);
        }

        protected abstract void OnMenuOptionToggled(int index, bool value);

        private int _mask;

        public void OnDropdownOpened()
        {
            Toggle[] toggles = transform.Find("Dropdown List").GetComponentsInChildren<Toggle>();

            for (int i = 0; i < toggles.Length; i++)
            {
                bool set = (_mask & (1 << (i + 1))) != 0;

                Toggle toggle = toggles[i];

                toggle.SetIsOnWithoutNotify(set);
                toggle.graphic.enabled = set;
            }
        }

        public bool IsFlagSet(int flag)
        {
            return (_mask & flag) != 0;
        }

        public bool IsFlagSetAtIndex(int index)
        {
            return (_mask & (1 << (index + 1))) != 0;
        }
    }
}
