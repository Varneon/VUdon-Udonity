using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Enums;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Handle : UdonSharpBehaviour
    {
        public float Size = 1f;

        public Transform Target;

        public Space SpaceMode;

        [SerializeField]
        private Pointer pointer;

        [SerializeField]
        private GameObject[] handles;

        [SerializeField]
        private Transform pointerRoot;

        [SerializeField]
        private UdonUndo udonUndo;

        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        private HandleType handleType = HandleType.Position;

        private VRCPlayerApi localPlayer;

        private bool vrEnabled;

        private bool grabbing;

        private HandType currentHand;

        private Axis currentAxis;

        private float axisGrabOffset;

        private VRCPlayerApi.TrackingDataType handTrackingDataType;

        public bool Hitting;

        public Vector3 RayPosition;

        public float Enter;

        public bool HitXY, HitYZ, HitZX;

        public Vector3 XY, YZ, ZX;

        public float EnterXY, EnterYZ, EnterZX;

        public Vector3 Offset;

        public bool Grabbing;

        public Vector3 originalPos;

        public float Depth;

        public Axis LockedAxis;

        private bool gridSnappingEnabled;

        private Rigidbody targetRigidbody;

        private bool hasRigidbody;

        private Quaternion lockedRotation;

        private object undoData;

        private void Start()
        {
            vrEnabled = (localPlayer = Networking.LocalPlayer).IsUserInVR();
        }

        public override void PostLateUpdate()
        {
            if (Target)
            {
                transform.SetPositionAndRotation(Target.position, SpaceMode == Space.World ? Quaternion.identity : grabbing ? lockedRotation : Target.rotation);
            }

            if (grabbing) 
            {
                switch (LockedAxis)
                {
                    case Axis.Z:
                        if(!pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 posXY)) { return; }
                        transform.position = transform.TransformPoint(Vector3.Scale(transform.InverseTransformPoint(posXY), new Vector3(1f, 1f, 0f))) - Offset;
                        break;
                    case Axis.X:
                        if (!pointer.TryGetPointOnPlane(transform, transform.right, out Vector3 posYZ)) { return; }
                        transform.position = transform.TransformPoint(Vector3.Scale(transform.InverseTransformPoint(posYZ), new Vector3(0f, 1f, 1f))) - Offset;
                        break;
                    case Axis.Y:
                        if (!pointer.TryGetPointOnPlane(transform, transform.up, out Vector3 posZX)) { return; }
                        transform.position = transform.TransformPoint(Vector3.Scale(transform.InverseTransformPoint(posZX), new Vector3(1f, 0f, 1f))) - Offset;
                        break;
                }

                if (Target)
                {
                    Vector3 newPosition = gridSnappingEnabled ? new Vector3(Mathf.Round(transform.position.x * 4f) / 4f, Mathf.Round(transform.position.y * 4f) / 4f, Mathf.Round(transform.position.z * 4f) / 4f) : transform.position;

                    if (hasRigidbody)
                    {
                        targetRigidbody.MovePosition(newPosition);

                        targetRigidbody.velocity = Vector3.zero;
                    }
                    else
                    {
                        Target.position = newPosition;
                    }

                    transform.SetPositionAndRotation(Target.position, lockedRotation);
                }
            }
        }

        public void SetHandleType(HandleType type)
        {
            // If attempting to set the same type, return
            if(handleType == type) { return; }

            if (handleType != HandleType.None)
            {
                handles[(int)handleType - 1].SetActive(false);
            }

            handleType = type;

            if (handleType != HandleType.None)
            {
                handles[(int)handleType - 1].SetActive(true);
            }
        }

        internal void SetTarget(Transform target)
        {
            Target = target;

            hasRigidbody = targetRigidbody = Target.GetComponent<Rigidbody>();
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if(args.handType != HandType.RIGHT) { return; }

            if(!value)
            {
                if (grabbing)
                {
                    grabbing = false;

                    if (Target) { udonUndo.EndRecordObject(Target, TransformUndoType.LocalPosition, "Move", undoData); }
                }

                lockedRotation = Quaternion.identity;

                return;
            }

            if(handleType != HandleType.Position) { return; }

            Vector3 headPos = pointer.RightHandOrigin;

            Vector3 headForward = pointer.RightHandDirection;

            // If the handle is behind the ray's forward direction, reject the grabbing action
            if (Vector3.Angle(headForward, transform.position - headPos) > 90f) { return; }

            Vector3 transformPos = transform.position;

            HitXY = pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 posXY);
            HitYZ = pointer.TryGetPointOnPlane(transform, transform.right, out Vector3 posYZ);
            HitZX = pointer.TryGetPointOnPlane(transform, transform.up, out Vector3 posZX);

            float distXY = Vector3.Distance(posXY, headPos);
            float distYZ = Vector3.Distance(posYZ, headPos);
            float distZX = Vector3.Distance(posZX, headPos);

            float maxRange = Size * 0.3f; // TODO: Separate range detection to each axis separately

            distXY = Vector3.Distance(posXY, transformPos) < maxRange ? distXY : float.MaxValue;
            distYZ = Vector3.Distance(posYZ, transformPos) < maxRange ? distYZ : float.MaxValue;
            distZX = Vector3.Distance(posZX, transformPos) < maxRange ? distZX : float.MaxValue;

            LockedAxis = Axis.Z;

            float minDistance = Mathf.Min(distXY, distYZ, distZX);

            //if(minDistance == EnterXY)
            if (minDistance == distXY)
            {
                LockedAxis = Axis.Z;

                originalPos = transformPos; Offset = posXY - transformPos;

                grabbing = Vector3.Distance(posXY, transformPos) < Size;
            }
            else if (minDistance == distYZ)
            {
                LockedAxis = Axis.X;

                originalPos = transformPos; Offset = posYZ - transformPos;

                grabbing = Vector3.Distance(posYZ, transformPos) < Size;
            }
            else if (minDistance == distZX)
            {
                LockedAxis = Axis.Y;

                originalPos = transformPos; Offset = posZX - transformPos;

                grabbing = Vector3.Distance(posZX, transformPos) < Size;
            }

            if (grabbing && Target) { undoData = udonUndo.BeginRecordObject(Target, TransformUndoType.LocalPosition); }

            lockedRotation = SpaceMode == Space.World ? Quaternion.identity : Target.rotation;
        }

        internal void SetGridSnappingEnabled(bool enabled)
        {
            gridSnappingEnabled = enabled;
        }
    }
}
