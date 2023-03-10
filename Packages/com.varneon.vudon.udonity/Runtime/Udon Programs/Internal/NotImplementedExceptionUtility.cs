using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Utilities
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NotImplementedExceptionUtility : UdonSharpBehaviour
    {
        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        public void ThrowStaticFlagsNotImplementedException()
        {
            logger.LogError("Static Editor Flags are not implemented!");
        }

        public void ThrowRotationToolNotImplementedException()
        {
            logger.LogError("Rotation tool is not implemented yet, only visual preview is available");
        }

        public void ThrowScaleToolNotImplementedException()
        {
            logger.LogError("Scale tool is not implemented yet, only visual preview is available");
        }
    }
}
