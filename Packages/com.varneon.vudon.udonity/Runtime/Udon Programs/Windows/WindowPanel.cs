using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Windows.Abstract;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WindowPanel : WindowLayoutElement
    {
        public EditorWindow ActiveWindow => activeWindow;

        [SerializeField]
        private RectTransform tabRoot;

        private WindowTab[] tabs;

        private WindowTab activeTab;

        private EditorWindow activeWindow;

        private void Start()
        {
            tabs = tabRoot.GetComponentsInChildren<WindowTab>();

            foreach(WindowTab tab in tabs)
            {
                tab.LinkPanel(this);
            }

            SetActiveTab(tabs[0]);
        }

        internal void SetActiveTab(WindowTab newActinveTab)
        {
            if(activeTab == newActinveTab) { return; }

            activeTab = newActinveTab;

            SetActiveWindow(activeTab.Window);

            foreach(WindowTab windowTab in tabs)
            {
                if(windowTab == newActinveTab) { windowTab.SetActive(); continue; }

                windowTab.SetInactive();
            }
        }

        private void SetActiveWindow(EditorWindow window)
        {
            activeWindow = window;

            minSize = window == null ? Vector2.zero : window.MinSize;
        }
    }
}
