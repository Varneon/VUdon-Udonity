using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Windows.Scene;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class SceneView : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 400f);

        [SerializeField]
        private Pointer pointer;

        [SerializeField]
        private Toggle altKeyToggle;

        [SerializeField]
        private SceneViewCamera sceneViewCamera;

        private bool sceneViewDragging;

        private Vector3 originalPointerPos;

        private bool altKeyPressed;

        private bool isLocalPlayerInVR;

        private float vrScrollInput;

        private void Start()
        {
            isLocalPlayerInVR = Networking.LocalPlayer.IsUserInVR();
        }

        protected override void OnWindowActiveStateChanged(bool active)
        {
            sceneViewCamera.gameObject.SetActive(active);
        }

        public void OnBeginSceneViewDrag()
        {
            if(pointer.TryGetPointOnPlane(transform, transform.forward, out originalPointerPos))
            {
                sceneViewDragging = true;

                OnSceneViewDrag();
            }
        }

        public void OnEndSceneViewDrag()
        {
            sceneViewDragging = false;
        }

        public void OnSceneViewDrag()
        {
            if (sceneViewDragging && pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 pointerPos))
            {
                Vector3 delta = transform.InverseTransformVector(pointerPos - originalPointerPos);

                Vector3 cameraDelta = new Vector3(-delta.y, delta.x, 0f) * 0.125f; // TODO: Parametrize

                if (altKeyPressed)
                {
                    sceneViewCamera.Rotate(cameraDelta);
                }
                else
                {
                    sceneViewCamera.RotateAroundTarget(cameraDelta);
                }

                originalPointerPos = pointerPos;

                SendCustomEventDelayedFrames(nameof(OnSceneViewDrag), 0);

                if(TryGetScrollInput(out float scrollInput))
                {
                    sceneViewCamera.ApplyScrollInput(scrollInput);
                }
            }
        }

        private bool TryGetScrollInput(out float scrollInput)
        {
            if (isLocalPlayerInVR)
            {
                scrollInput = vrScrollInput;

                return Mathf.Abs(scrollInput) > 0.1f;
            }
            else
            {
                scrollInput = Input.GetAxisRaw("Mouse ScrollWheel") * 5f;

                return Mathf.Abs(scrollInput) > 0f;
            }
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if(isLocalPlayerInVR && args.handType == HandType.RIGHT)
            {
                vrScrollInput = value;
            }
        }

        public void OnAltKeyToggled()
        {
            altKeyPressed = altKeyToggle.isOn;
        }
    }
}
