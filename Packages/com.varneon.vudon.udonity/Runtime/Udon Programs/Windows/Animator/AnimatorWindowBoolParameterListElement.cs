using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using Animator = UnityEngine.Animator;

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class AnimatorWindowBoolParameterListElement : Abstract.AnimatorWindowParameterListElement
    {
        [SerializeField]
        private Fields.Toggle toggle;

        private bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    toggle.SetValueWithoutNotify(value);
                }
            }
        }

        private bool _value;

        internal void Intialize(Animator animator, string parameter)
        {
            targetAnimator = animator;

            parameterHash = Animator.StringToHash(parameter);

            Value = targetAnimator.GetBool(parameterHash);

            toggle.RegisterValueChangedCallback(this, nameof(OnValueChanged));
        }

        public void OnValueChanged()
        {
            targetAnimator.SetBool(parameterHash, toggle.Value);
        }
    }
}
