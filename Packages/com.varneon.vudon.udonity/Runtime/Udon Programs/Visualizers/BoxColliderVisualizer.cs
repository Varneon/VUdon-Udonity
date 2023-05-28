using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Visualizers
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(2147483647)] // Ensure that visualization executes as late as possible
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BoxColliderVisualizer : UdonSharpBehaviour
    {
        [SerializeField]
        private Material material;

        private BoxCollider target;

        private Transform targetTransform;

        //private Mesh mesh;
        //private Matrix4x4[] matrixArray = new Matrix4x4[1];

        internal void Initialize(BoxCollider boxCollider)
        {
            //mesh = GetComponent<MeshFilter>().mesh;

            target = boxCollider;

            targetTransform = target.transform;
        }

        public override void PostLateUpdate()
        {
            if(target == null || targetTransform == null) { Destroy(); return; }

            // This would be the "better" way of doing this, but Udon's performance seems to take an increased hit
            //matrixArray[0] = Matrix4x4.TRS(targetTransform.TransformPoint(target.center), targetTransform.rotation, Vector3.Scale(targetTransform.lossyScale, target.size));
            //VRCGraphics.DrawMeshInstanced(mesh, 0, material, matrixArray);

            // So we will default to using transform
            transform.SetPositionAndRotation(targetTransform.TransformPoint(target.center), targetTransform.rotation);

            transform.localScale = Vector3.Scale(targetTransform.lossyScale, target.size);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
