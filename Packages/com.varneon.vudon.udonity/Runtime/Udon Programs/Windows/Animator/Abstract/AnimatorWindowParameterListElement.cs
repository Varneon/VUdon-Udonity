using UdonSharp;

namespace Varneon.VUdon.Udonity.Windows.Animator.Abstract
{
    using Animator = UnityEngine.Animator;

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class AnimatorWindowParameterListElement : UdonSharpBehaviour
    {
        internal Animator targetAnimator;

        protected int parameterHash;
    }
}
