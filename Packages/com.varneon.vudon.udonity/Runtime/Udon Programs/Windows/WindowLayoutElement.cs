using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class WindowLayoutElement : UdonSharpBehaviour
    {
        public Vector2 SizeDelta
        {
            get => rectTransform.sizeDelta;
            set => rectTransform.sizeDelta = value;
        }

        public Vector2 AnchoredPosition
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }

        public RectTransform RectTransform => rectTransform;

        public Vector2 MinSize => minSize;

        [SerializeField, HideInInspector]
        internal protected RectTransform rectTransform;

        protected Vector2 minSize = Vector2.zero;
    }
}
