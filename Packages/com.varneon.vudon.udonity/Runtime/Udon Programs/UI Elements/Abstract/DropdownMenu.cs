using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements.Abstract
{
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class DropdownMenu : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector]
        private Dropdown dropdown;

        public void OnMenuItemInvoked()
        {
            OnMenuItemInvoked(dropdown.value);
        }

        internal void InitializeOnBuild()
        {
            dropdown = GetComponentInChildren<Dropdown>(true);
        }

        protected abstract void OnMenuItemInvoked(int index);
    }
}
