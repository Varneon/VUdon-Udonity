using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Enums;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
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

        private Vector3 originalScale;

        private float scaleMultiplier;

        private void Start()
        {
            vrEnabled = (localPlayer = Networking.LocalPlayer).IsUserInVR();
        }

        public override void PostLateUpdate()
        {
            if (grabbing) 
            {
                switch (handleType)
                {
                    case HandleType.Position:
                        if (Target) { transform.SetPositionAndRotation(Target.position, SpaceMode == Space.World ? Quaternion.identity : grabbing ? lockedRotation : Target.rotation); }
                        HandlePositionGrab();
                        break;
                    case HandleType.Rotation:
                        if (Target) { transform.position = Target.position; }
                        HandleRotationGrab();
                        break;
                    case HandleType.Scale:
                        if (Target) { transform.SetPositionAndRotation(Target.position, lockedRotation); }
                        HandleScaleGrab();
                        break;
                }
            }
            else
            {
                if (Target) { transform.SetPositionAndRotation(Target.position, (SpaceMode == Space.World && handleType != HandleType.Scale) ? Quaternion.identity : Target.rotation); }
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

                    if (Target)
                    {
                        switch (handleType)
                        {
                            case HandleType.Position:
                                udonUndo.EndRecordObject(Target, TransformUndoType.LocalPosition, "Move", undoData);
                                break;
                            case HandleType.Rotation:
                                udonUndo.EndRecordObject(Target, TransformUndoType.LocalRotation, "Rotate", undoData);
                                break;
                            case HandleType.Scale:
                                udonUndo.EndRecordObject(Target, TransformUndoType.LocalScale, "Scale", undoData);
                                break;
                        }
                    }
                }

                lockedRotation = Quaternion.identity;

                return;
            }

            switch (handleType)
            {
                case HandleType.Position:
                    InitializePositionGrab();
                    break;
                case HandleType.Rotation:
                    InitializeRotationGrab();
                    break;
                case HandleType.Scale:
                    InitializeScaleGrab();
                    break;
            }
        }

        private void InitializePositionGrab()
        {
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

            if (minDistance == distXY)
            {
                if (Vector3.Distance(posXY, transformPos) > maxRange) { return; }

                LockedAxis = Axis.Z;

                originalPos = transformPos; Offset = posXY - transformPos;

                grabbing = Vector3.Distance(posXY, transformPos) < Size;
            }
            else if (minDistance == distYZ)
            {
                if (Vector3.Distance(posYZ, transformPos) > maxRange) { return; }

                LockedAxis = Axis.X;

                originalPos = transformPos; Offset = posYZ - transformPos;

                grabbing = Vector3.Distance(posYZ, transformPos) < Size;
            }
            else if (minDistance == distZX)
            {
                if (Vector3.Distance(posZX, transformPos) > maxRange) { return; }

                LockedAxis = Axis.Y;

                originalPos = transformPos; Offset = posZX - transformPos;

                grabbing = Vector3.Distance(posZX, transformPos) < Size;
            }

            if (grabbing && Target) { undoData = udonUndo.BeginRecordObject(Target, TransformUndoType.LocalPosition); }

            lockedRotation = SpaceMode == Space.World ? Quaternion.identity : Target.rotation;
        }

        private void HandlePositionGrab()
        {
            switch (LockedAxis)
            {
                case Axis.Z:
                    if (!pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 posXY)) { return; }
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

        private void InitializeRotationGrab()
        {
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

            float maxRange = Size + 0.05f;

            float minRange = Size - 0.05f;

            float deltaXY = Vector3.Distance(posXY, transformPos);
            float deltaYZ = Vector3.Distance(posYZ, transformPos);
            float deltaZX = Vector3.Distance(posZX, transformPos);

            distXY = deltaXY < maxRange ? ((deltaXY > minRange) ? distXY : float.MaxValue) : float.MaxValue;
            distYZ = deltaYZ < maxRange ? ((deltaYZ > minRange) ? distYZ : float.MaxValue) : float.MaxValue;
            distZX = deltaZX < maxRange ? ((deltaZX > minRange) ? distZX : float.MaxValue) : float.MaxValue;

            LockedAxis = Axis.Z;

            float minDistance = Mathf.Min(distXY, distYZ, distZX);

            if (minDistance == distXY)
            {
                if (Vector3.Distance(posXY, transformPos) > maxRange) { return; }

                LockedAxis = Axis.Z;

                Vector3 localPos = transform.InverseTransformPoint(posXY).normalized;

                Offset = new Vector3(0f, 0f, Mathf.Atan2(-localPos.y, localPos.x) * Mathf.Rad2Deg);

                grabbing = true;
            }
            else if (minDistance == distYZ)
            {
                if (Vector3.Distance(posYZ, transformPos) > maxRange) { return; }

                LockedAxis = Axis.X;

                Vector3 localPos = transform.InverseTransformPoint(posYZ).normalized;

                Offset = new Vector3(Mathf.Atan2(-localPos.z, localPos.y) * Mathf.Rad2Deg, 0f, 0f);

                grabbing = true;
            }
            else if (minDistance == distZX)
            {
                if (Vector3.Distance(posZX, transformPos) > maxRange) { return; }

                LockedAxis = Axis.Y;

                Vector3 localPos = transform.InverseTransformPoint(posZX).normalized;

                Offset = new Vector3(0f, Mathf.Atan2(-localPos.x, localPos.z) * Mathf.Rad2Deg, 0f);

                grabbing = true;
            }

            if (grabbing && Target) { undoData = udonUndo.BeginRecordObject(Target, TransformUndoType.LocalRotation); }

            lockedRotation = SpaceMode == Space.World ? Quaternion.identity : Target.rotation;
        }

        private void HandleRotationGrab()
        {
            Vector3 localPos;

            Quaternion offset;

            switch (LockedAxis)
            {
                case Axis.Z:
                    if (!pointer.TryGetPointOnPlane(transform, transform.forward, out Vector3 posXY)) { return; }
                    localPos = transform.InverseTransformPoint(posXY).normalized;
                    offset = Quaternion.Euler(0f, 0f, Offset.z - Mathf.Atan2(-localPos.y, localPos.x) * Mathf.Rad2Deg);
                    break;
                case Axis.X:
                    if (!pointer.TryGetPointOnPlane(transform, transform.right, out Vector3 posYZ)) { return; }
                    localPos = transform.InverseTransformPoint(posYZ).normalized;
                    offset = Quaternion.Euler(Offset.x - Mathf.Atan2(-localPos.z, localPos.y) * Mathf.Rad2Deg, 0f, 0f);
                    break;
                case Axis.Y:
                    if (!pointer.TryGetPointOnPlane(transform, transform.up, out Vector3 posZX)) { return; }
                    localPos = transform.InverseTransformPoint(posZX).normalized;
                    offset = Quaternion.Euler(0f, Offset.y - Mathf.Atan2(-localPos.x, localPos.z) * Mathf.Rad2Deg, 0f);
                    break;
                default: return;
            }

            transform.rotation *= offset;

            if (Target)
            {
                Quaternion targetRotation = Quaternion.identity;

                switch (SpaceMode)
                {
                    case Space.Self:
                        targetRotation = Target.rotation * offset;
                        break;
                    case Space.World:
                        targetRotation = offset * Target.rotation;
                        break;
                }

                if (hasRigidbody)
                {
                    targetRigidbody.MoveRotation(targetRotation);

                    targetRigidbody.angularVelocity = Vector3.zero;
                }
                else
                {
                    Target.rotation = targetRotation;
                }
            }
        }

        private void InitializeScaleGrab()
        {
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

            float maxRange = Size; // TODO: Separate range detection to each axis separately

            distXY = Vector3.Distance(posXY, transformPos) < maxRange ? distXY : float.MaxValue;
            distYZ = Vector3.Distance(posYZ, transformPos) < maxRange ? distYZ : float.MaxValue;
            distZX = Vector3.Distance(posZX, transformPos) < maxRange ? distZX : float.MaxValue;

            LockedAxis = Axis.Z;

            float minDistance = Mathf.Min(distXY, distYZ, distZX);

            if (minDistance == distXY)
            {
                if (Vector3.Distance(posXY, transformPos) > maxRange) { return; }

                Vector3 localPos = transform.InverseTransformPoint(posXY);

                LockedAxis = Vector3.Dot(Vector3.up, localPos.normalized) >= 0.5f ? Axis.Y : Axis.X;

                Offset = localPos;
            }
            else if (minDistance == distYZ)
            {
                if (Vector3.Distance(posYZ, transformPos) > maxRange) { return; }

                Vector3 localPos = transform.InverseTransformPoint(posYZ);

                LockedAxis = Vector3.Dot(Vector3.up, localPos.normalized) >= 0.5f ? Axis.Y : Axis.Z;

                Offset = localPos;
            }
            else if (minDistance == distZX)
            {
                if (Vector3.Distance(posZX, transformPos) > maxRange) { return; }

                Vector3 localPos = transform.InverseTransformPoint(posZX);

                LockedAxis = Vector3.Dot(Vector3.right, localPos.normalized) >= 0.5f ? Axis.X : Axis.Z;

                Offset = localPos;
            }

            grabbing = true;

            if (grabbing && Target)
            {
                scaleMultiplier = 1f / Offset[(int)LockedAxis - 1];

                originalScale = Target.localScale;

                undoData = udonUndo.BeginRecordObject(Target, TransformUndoType.LocalScale);
            }

            lockedRotation = Target.rotation;
        }

        private void HandleScaleGrab()
        {
            Vector3 scale = Vector3.one;

            switch (LockedAxis)
            {
                case Axis.Z:
                    Vector3 dir = Quaternion.LookRotation(transform.forward, pointer.RightHandOrigin - transform.position) * Vector3.up;
                    if (!pointer.TryGetPointOnPlane(transform, dir, out Vector3 posZ)) { return; }
                    scale = new Vector3(1f, 1f, 1f + (transform.InverseTransformPoint(posZ).z - Offset.z) * scaleMultiplier);
                    break;
                case Axis.X:
                    dir = Quaternion.LookRotation(transform.right, pointer.RightHandOrigin - transform.position) * Vector3.up;
                    if (!pointer.TryGetPointOnPlane(transform, dir, out Vector3 posX)) { return; }
                    scale = new Vector3(1f + (transform.InverseTransformPoint(posX).x - Offset.x) * scaleMultiplier, 1f, 1f);
                    break;
                case Axis.Y:
                    dir = Quaternion.LookRotation(transform.up, pointer.RightHandOrigin - transform.position) * Vector3.up;
                    if (!pointer.TryGetPointOnPlane(transform, dir, out Vector3 posY)) { return; }
                    scale = new Vector3(1f, 1f + (transform.InverseTransformPoint(posY).y - Offset.y) * scaleMultiplier, 1f);
                    break;
            }

            if (Target)
            {
                Target.localScale = Vector3.Scale(originalScale, scale);
            }
        }

        internal void SetGridSnappingEnabled(bool enabled)
        {
            gridSnappingEnabled = enabled;
        }
    }
}
