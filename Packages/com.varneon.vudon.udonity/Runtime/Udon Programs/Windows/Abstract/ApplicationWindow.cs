using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ApplicationWindow : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector]
        internal protected Pointer pointer;

        private Vector3 grabOffset;

        private bool dragging;

        private Vector2 windowSize;

        private RectTransform parentRectTransform;

        float parentExtentX;

        float parentExtentY;

        [SerializeField, HideInInspector]
        internal RectTransform rectTransform;

        public void BeginWindowDrag()
        {
            // If window is already being dragged, return
            if (dragging) { return; }

            // Check if drag is valid and get the pointer position
            if (dragging = pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 pointerPosition))
            {
                // Set the window's transform as last sibling so it will render on top of other windows
                transform.SetAsLastSibling();

                // Calculate the relative grab offset
                grabOffset = Vector3.Scale(transform.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

                // Get the size of the window
                Vector2 selfSize = rectTransform.sizeDelta;

                // Get the size of the containing element
                Vector2 parentSize = transform.parent.GetComponent<RectTransform>().rect.size;

                // Get the extents where the window can move
                parentExtentX = (parentSize.x - selfSize.x) / 2f;
                parentExtentY = (parentSize.y - selfSize.y) / 2f;

                _LateUpdate();
            }
        }

        public void _LateUpdate()
        {
            // If window is not currently being dragged, return
            if (!dragging) { return; }

            // If the pointer isn't currently on the window's plane, set dragging state to false and return
            if(!pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 pointerPosition)) { dragging = false; return; }

            // Get this frame's pointer position delta
            Vector3 delta = Vector3.Scale(transform.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

            // Apply the position delta
            transform.localPosition += delta - grabOffset;

            // Get the local position of the transform
            Vector3 localPos = transform.localPosition;

            // Clamp local position to the allowed extents
            transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -parentExtentX, parentExtentX), Mathf.Clamp(localPos.y, -parentExtentY, parentExtentY), 0f);

            SendCustomEventDelayedFrames(nameof(_LateUpdate), 0, VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        }

        public void EndWindowDrag()
        {
            if (!dragging) { return; }

            dragging = false;
        }
    }
}
