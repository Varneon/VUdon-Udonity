namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using Animator = UnityEngine.Animator;

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
