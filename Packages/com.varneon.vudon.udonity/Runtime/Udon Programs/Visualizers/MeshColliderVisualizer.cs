using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Visualizers
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(2147483647)] // Ensure that visualization executes as late as possible
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MeshColliderVisualizer : UdonSharpBehaviour
    {
        [SerializeField]
        private Material material;

        [SerializeField]
        private MeshFilter meshFilter;

        private MeshCollider target;

        private Transform targetTransform;

        internal void Initialize(MeshCollider meshCollider)
        {
            target = meshCollider;

            targetTransform = target.transform;

            Mesh mesh = target.sharedMesh;

            // If shared mesh of the collider is null, primitive is likely used, try to get one from MeshFilter
            if (mesh == null)
            {
                MeshFilter filter = target.GetComponent<MeshFilter>();

                if (filter)
                {
                    mesh = filter.sharedMesh;

                    if (mesh)
                    {
                        meshFilter.sharedMesh = mesh;
                    }
                }
            }
            else
            {
                meshFilter.sharedMesh = target.sharedMesh;
            }
        }

        public override void PostLateUpdate()
        {
            if (target == null || targetTransform == null) { Destroy(); return; }

            transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);

            transform.localScale = targetTransform.lossyScale;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
