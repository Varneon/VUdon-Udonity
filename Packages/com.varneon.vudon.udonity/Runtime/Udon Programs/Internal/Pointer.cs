using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [DefaultExecutionOrder(10000)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Pointer : UdonSharpBehaviour
    {
        public Vector3 RightHandOrigin => rightHandOrigin;

        public Vector3 RightHandDirection => rightHandDirection;

        [SerializeField]
        private LineRenderer leftHandLine, rightHandLine;

        private VRCPlayerApi localPlayer;

        private bool vrEnabled;

        private bool desktopEnabled;

        private Vector3 rightHandOrigin, leftHandOrigin;

        private Vector3 rightHandDirection = Vector3.forward, leftHandDirection = Vector3.forward;

        private Quaternion rightHandRotation, leftHandRotation;

#if UNITY_ANDROID
        private readonly Vector3 vrRayPositionOffset = new Vector3(0.06f, 0f, 0.08f);

        private readonly Quaternion vrRayRotationOffset = Quaternion.Euler(0f, 45f, 0f);
#else
        private readonly Vector3 vrRayPositionOffset = new Vector3(0.032f, 0f, 0.088f);

        private readonly Quaternion vrRayRotationOffset = Quaternion.Euler(0f, 40f, 0f);
#endif

        private readonly Vector3 vrRayForward = Quaternion.Euler(0f, 40f, 0f) * Vector3.forward;

        private Vector3 playerScale;

        private Vector3 scaledVRRayPositionOffset;

        private float intersectedRayDistance;

        private const float BASE_ROTATION_DELTA = 5f;

        private const float BASE_POSITION_DELTA = 0.0025f;

        private void Start()
        {
            desktopEnabled = !(vrEnabled = (localPlayer = Networking.LocalPlayer).IsUserInVR());

            if (vrEnabled)
            {
                SetPlayerScale(1f);
            }
            else
            {
                leftHandLine.gameObject.SetActive(false);
                rightHandLine.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            UpdateTrackingData();
        }

        private void UpdateTrackingData()
        {
            if (vrEnabled)
            {
                VRCPlayerApi.TrackingData rightHandTD = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
                VRCPlayerApi.TrackingData leftHandTD = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);

                Quaternion rightHandRotationRaw = rightHandTD.rotation;
                Quaternion leftHandRotationRaw = leftHandTD.rotation;

                float rotationDeltaTime = Time.deltaTime * 2.5f;

                rightHandRotation = Quaternion.Lerp(rightHandRotation, rightHandRotationRaw, rotationDeltaTime * (Quaternion.Angle(rightHandRotation, rightHandRotationRaw) + BASE_ROTATION_DELTA));
                leftHandRotation = Quaternion.Lerp(leftHandRotation, leftHandRotationRaw, rotationDeltaTime * (Quaternion.Angle(leftHandRotation, leftHandRotationRaw) + BASE_ROTATION_DELTA));
                //rightHandRotation = Quaternion.RotateTowards(rightHandRotation, rightHandRotationRaw, Time.deltaTime * 25f * Quaternion.Angle(rightHandRotation, rightHandRotationRaw));
                //leftHandRotation = leftHandTD.rotation;

                Vector3 rightHandOriginTarget = rightHandTD.position + rightHandRotation * scaledVRRayPositionOffset;
                Vector3 leftHandOriginTarget = leftHandTD.position + leftHandRotation * scaledVRRayPositionOffset;

                float positionDeltaTime = rotationDeltaTime * 1000f;

                rightHandOrigin = Vector3.Lerp(rightHandOrigin, rightHandOriginTarget, positionDeltaTime * (Vector3.Distance(rightHandOrigin, rightHandOriginTarget) + BASE_POSITION_DELTA));
                leftHandOrigin = Vector3.Lerp(leftHandOrigin, leftHandOriginTarget, positionDeltaTime * (Vector3.Distance(leftHandOrigin, leftHandOriginTarget) + BASE_POSITION_DELTA));
                //leftHandOrigin = leftHandTD.position + leftHandRotation * scaledVRRayPositionOffset;

                //Vector3 rightHandDirectionTarget = (rightHandRotation *= vrRayRotationOffset) * Vector3.forward;

                //rightHandDirection = Vector3.Slerp(rightHandDirection, rightHandDirectionTarget, Time.deltaTime * 100f * Vector3.Angle(rightHandDirection, rightHandDirectionTarget));
                rightHandDirection = rightHandRotation * vrRayForward;
                leftHandDirection = leftHandRotation * vrRayForward;

#region Ray Visualization

                leftHandLine.SetPosition(0, leftHandOrigin);
                leftHandLine.SetPosition(1, leftHandOrigin + (leftHandDirection * 2f));
                rightHandLine.SetPosition(0, rightHandOrigin);
                rightHandLine.SetPosition(1, rightHandOrigin + (rightHandDirection * intersectedRayDistance));

#endregion

                intersectedRayDistance = 5f;
            }
            else
            {
                VRCPlayerApi.TrackingData headTD = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

                rightHandOrigin = headTD.position;
                rightHandDirection = headTD.rotation * Vector3.forward;
            }
        }

        public void SetPlayerScale(float scale)
        {
            playerScale = Vector3.one * scale;

            scaledVRRayPositionOffset = Vector3.Scale(vrRayPositionOffset, playerScale);
        }

        public Vector3 GetInverseTransformRayPoint(Transform targetTransform, Vector3 rayOrigin)
        {
            return targetTransform.InverseTransformPoint(rayOrigin);
        }

        public Vector3 GetInverseTransformVector(Transform targetTransform)
        {
            return targetTransform.InverseTransformVector(targetTransform.position);
        }

        [PublicAPI]
        /// <summary>
        /// Attempts to get a point on a plane
        /// </summary>
        /// <param name="planeRoot"></param>
        /// <param name="forwardDirection"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TryGetPointOnPlane(Transform planeRoot, Vector3 forwardDirection, out Vector3 position)
        {
            // Get the position of the plane root
            Vector3 planeRootPosition = planeRoot.position;

            // If the pointer direction is pointing away from the plane, return early
            if (Vector3.Angle(RightHandDirection, planeRootPosition - RightHandOrigin) > 90f)
            {
                position = Vector3.zero;

                return false;
            }

            // Create a plane
            Plane plane = new Plane(forwardDirection, 0f);

            // Create a ray
            Ray ray = new Ray(RightHandOrigin - planeRootPosition, RightHandDirection);

            // Check if the ray hits the plane and get the intersection distance
            bool hit = plane.Raycast(ray, out float enter);

            // If ray doesn't hit the plane, return early
            if (!hit) { position = Vector3.zero; return false; }

            // Get the intersection point from ray
            Vector3 point = ray.GetPoint(enter);

            // Get the world space point and assign it to the out parameter
            position = point + planeRootPosition;

            // Return the hit state (true)
            return hit;
        }

        public void IntersectRayAtDistance(float distance)
        {
            intersectedRayDistance = distance;
        }
    }
}
