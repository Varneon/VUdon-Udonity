using UdonSharp;
using UnityEngine;
using Varneon.VUdon.ArrayExtensions;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WindowPanelSeparator : WindowLayoutElement
    {
        [SerializeField]
        private Pointer pointer;

        [SerializeField]
        private WindowLayoutElement[] firstElements, secondElements;

        private Vector3 grabOffset;

        private bool grabbing;

        [SerializeField]
        private bool vertical;

        private bool horizontal;

        private Vector2 originalPosition;

        private float minSize1, minSize2;

        private void Start()
        {
            horizontal = !vertical;
        }

        public void OnDragBegin()
        {
            if (grabbing) { return; }

            grabbing = pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 pointerPosition);

            if (grabbing)
            {
                grabOffset = Vector3.Scale(transform.InverseTransformPoint(pointerPosition), new Vector3(horizontal ? 1f : 0f, vertical ? 1f : 0f, 0f));

                originalPosition = rectTransform.anchoredPosition;

                minSize1 = 0f;
                minSize2 = 0f;

                foreach(WindowLayoutElement element in firstElements)
                {
                    minSize1 = Mathf.Max(minSize1, vertical ? element.MinSize.y : element.MinSize.x);
                }

                foreach (WindowLayoutElement element in secondElements)
                {
                    minSize2 = Mathf.Max(minSize2, vertical ? element.MinSize.y : element.MinSize.x);
                }

                _LateUpdate();
            }
        }

        public void _LateUpdate()
        {
            if (!grabbing) { return; }

            if (!pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 pointerPosition)) { grabbing = false; return; }

            Vector3 scale = transform.lossyScale;

            Vector3 delta = Vector3.Scale(transform.InverseTransformPoint(pointerPosition), new Vector3(horizontal ? 1f : 0f, vertical ? 1f : 0f, 0f));

            Vector2 resizeDelta = (Vector2)delta - (Vector2)grabOffset;

            Vector2 firstSize = firstElements.FirstOrDefault().RectTransform.rect.size;
            Vector2 secondSize = secondElements.FirstOrDefault().RectTransform.rect.size;

            Vector2 fixedResizeDelta = new Vector2(
                horizontal ? Mathf.Clamp(resizeDelta.x, minSize1 - firstSize.x, secondSize.x - minSize2) : 0f,
                vertical ? Mathf.Clamp(resizeDelta.y, minSize1 - firstSize.y, secondSize.y - minSize2) : 0f);

            rectTransform.anchoredPosition += fixedResizeDelta;

            Vector2 halfResizeDelta = fixedResizeDelta / 2f;

            foreach (WindowLayoutElement element in firstElements)
            {
                element.AnchoredPosition += halfResizeDelta;
                element.SizeDelta += fixedResizeDelta;
            }

            foreach (WindowLayoutElement element in secondElements)
            {
                element.AnchoredPosition += halfResizeDelta;
                element.SizeDelta -= fixedResizeDelta;
            }

            SendCustomEventDelayedFrames(nameof(_LateUpdate), 0, VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        }

        public void OnDragEnd()
        {
            if (!grabbing) { return; }

            grabbing = false;
        }
    }
}
