using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class DropdownToggleMenu : Abstract.DropdownToggleMenu
    {
        [SerializeField]
        private Toggle[] eventProxies;

        protected override void OnMenuOptionToggled(int index, bool value)
        {
            eventProxies[index].isOn = value;
        }
    }
}
