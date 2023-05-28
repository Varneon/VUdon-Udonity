using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Windows.Abstract;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WindowTab : UdonSharpBehaviour
    {
        public EditorWindow Window => window;

        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private Image highlight;

        [SerializeField]
        private Color defaultButtonColor;

        [SerializeField]
        private Color selectedButtonColor;

        [SerializeField]
        private EditorWindow window;

        private WindowPanel linkedPanel;

        private Button button;

        internal void SetInactive()
        {
            SetActive(false);
        }

        internal void SetActive()
        {
            SetActive(true);
        }

        private void SetActive(bool active)
        {
            ColorBlock buttonColorBlock = button.colors;

            buttonColorBlock.normalColor = active ? selectedButtonColor : defaultButtonColor;

            button.colors = buttonColorBlock;

            highlight.gameObject.SetActive(active);

            content.gameObject.SetActive(active);

            if (window) { window.SetWindowActive(active); }
        }

        internal void LinkPanel(WindowPanel panel)
        {
            button = GetComponent<Button>();

            linkedPanel = panel;
        }

        public void OnClicked()
        {
            linkedPanel.SetActiveTab(this);
        }
    }
}
