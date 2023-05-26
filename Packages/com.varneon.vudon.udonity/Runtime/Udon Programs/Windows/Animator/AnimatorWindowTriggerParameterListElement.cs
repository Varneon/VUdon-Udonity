namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using UnityEngine;
    using Animator = UnityEngine.Animator;

    [AddComponentMenu("")]
    public class AnimatorWindowTriggerParameterListElement : Abstract.AnimatorWindowParameterListElement
    {
        internal void Intialize(Animator animator, string parameter)
        {
            targetAnimator = animator;

            parameterHash = Animator.StringToHash(parameter);
        }

        public void InvokeTrigger()
        {
            targetAnimator.SetTrigger(parameterHash);
        }
    }
}
