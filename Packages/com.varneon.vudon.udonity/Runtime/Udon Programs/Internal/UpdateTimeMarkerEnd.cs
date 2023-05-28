using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Utilities
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(2147483647)]
    public class UpdateTimeMarkerEnd : UdonSharpBehaviour
    {
        [SerializeField]
        private UpdateTimeMarkerStart startMarker;

        private void FixedUpdate()
        {
            startMarker.OnFixedUpdateEnd();
        }

        private void Update()
        {
            startMarker.OnUpdateEnd();
        }

        private void LateUpdate()
        {
            startMarker.OnLateUpdateEnd();
        }

        public override void PostLateUpdate()
        {
            startMarker.OnPostLateUpdateEnd();
        }
    }
}
