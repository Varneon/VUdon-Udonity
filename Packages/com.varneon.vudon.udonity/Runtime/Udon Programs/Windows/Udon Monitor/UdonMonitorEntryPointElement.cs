using UdonSharp;

namespace Varneon.VUdon.Udonity.Windows.UdonMonitor
{
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
