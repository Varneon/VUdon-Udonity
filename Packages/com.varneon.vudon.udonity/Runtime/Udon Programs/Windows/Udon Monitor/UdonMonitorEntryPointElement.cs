using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.UdonMonitor
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonMonitorEntryPointElement : UdonSharpBehaviour
    {
        internal string methodName;

        public void InvokeMethod()
        {
            GetComponentInParent<UdonMonitorWindow>().InvokeMethod(methodName);
        }
    }
}
