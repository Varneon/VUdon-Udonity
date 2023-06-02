using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Windows.Scene
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SceneViewCamera : UdonSharpBehaviour
    {
        [SerializeField]
        private Dropdown renderingModeDropdown;

        [SerializeField]
        private Toggle skyboxToggle;

        [SerializeField]
        private Toggle remotePlayersToggle;

        [SerializeField]
        private GameObject
            spatialMappingWireframeProjector,
            spatialMappingTransparentWireframeProjector;

        private new Camera camera;

        private Transform followTarget;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        public void OnToggleSkyboxRendering()
        {
            camera.clearFlags = skyboxToggle.isOn ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        }

        public void OnToggleRemotePlayerRendering()
        {
            if (remotePlayersToggle.isOn)
            {
                camera.cullingMask |= 1 << 9;
            }
            else
            {
                camera.cullingMask &= ~(1 << 9);
            }
        }

        public void OnSceneRenderingModeChanged()
        {
            switch (renderingModeDropdown.value)
            {
                case 0:
                    SetSpatialMappingWireframeProjectorActive(false, false);
                    break;
                case 1:
                    SetSpatialMappingWireframeProjectorActive(true, false);
                    break;
                case 2:
                    SetSpatialMappingWireframeProjectorActive(true, true);
                    break;
            }
        }

        private void SetSpatialMappingWireframeProjectorActive(bool active, bool transparent)
        {
            spatialMappingWireframeProjector.SetActive(active && !transparent);

            spatialMappingTransparentWireframeProjector.SetActive(active && transparent);
        }

        public void OnFollowTarget()
        {
            if(followTarget == null) { return; }

            FrameLockedTransform();

            SendCustomEventDelayedFrames(nameof(OnFollowTarget), 0);
        }

        internal void FrameTransform(Transform target)
        {
            ReleaseFollow();

            Vector3 rotationalOffset = transform.rotation * Vector3.forward;

            transform.position = target.position + rotationalOffset * -5f; // TODO: Detect target bounds
        }

        private void FrameLockedTransform()
        {
            Vector3 rotationalOffset = transform.rotation * Vector3.forward;

            transform.position = followTarget.position + rotationalOffset * -5f; // TODO: Detect target bounds
        }

        internal void LockViewToSelected(Transform target)
        {
            followTarget = target;

            OnFollowTarget();
        }

        internal void ReleaseFollow()
        {
            followTarget = null;
        }

        internal void MoveToView(Transform target)
        {
            target.position = transform.position + transform.rotation * (Vector3.forward * 2f);
        }

        internal void AlignWithView(Transform target)
        {
            target.SetPositionAndRotation(transform.position, transform.rotation);
        }

        internal void Rotate(Vector3 delta)
        {
            transform.localEulerAngles += delta;
        }

        internal void RotateAroundTarget(Vector3 axis, float angle)
        {
            Vector3 pivot = followTarget == null ? transform.position + transform.forward * 3f : followTarget.position;

            transform.RotateAround(pivot, Quaternion.Euler(0f, transform.localEulerAngles.y, 0f) * axis, angle);
        }
    }
}
