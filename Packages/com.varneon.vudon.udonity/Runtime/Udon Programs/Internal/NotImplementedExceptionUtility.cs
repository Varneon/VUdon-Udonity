using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Utilities
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NotImplementedExceptionUtility : UdonSharpBehaviour
    {
        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        public void ThrowStaticFlagsNotImplementedException()
        {
            logger.LogError("Static Editor Flags are not implemented!");
        }
    }
}
