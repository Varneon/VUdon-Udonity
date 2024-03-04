using System;
using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.UdonMonitor
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonMonitorSymbolElement : UdonSharpBehaviour
    {
        private Type symbolType;

        private string symbolName;

        internal void Initialize(string symbolName, Type symbolType)
        {
            this.symbolName = symbolName;
            this.symbolType = symbolType;
        }

        public void SelectSymbol()
        {
            GetComponentInParent<UdonMonitorWindow>().SelectSymbol(symbolName, symbolType);
        }
    }
}
