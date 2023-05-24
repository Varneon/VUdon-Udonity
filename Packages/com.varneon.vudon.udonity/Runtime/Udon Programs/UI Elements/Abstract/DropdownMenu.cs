using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class DropdownMenu : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector]
        internal Dropdown dropdown;

        public void OnMenuItemInvoked()
        {
            OnMenuItemInvoked(dropdown.value);
        }

        protected abstract void OnMenuItemInvoked(int index);
    }
}
