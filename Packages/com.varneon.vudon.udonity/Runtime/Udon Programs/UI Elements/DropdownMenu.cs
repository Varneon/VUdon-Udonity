using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.UIElements
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class DropdownMenu : Abstract.DropdownMenu
    {
        [SerializeField]
        private Button[] eventProxies;

        protected override void OnMenuItemInvoked(int index)
        {
            eventProxies[index].OnSubmit(null);
        }
    }
}
