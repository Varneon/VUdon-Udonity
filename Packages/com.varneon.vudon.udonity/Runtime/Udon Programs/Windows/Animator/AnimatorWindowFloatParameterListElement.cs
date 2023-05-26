using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using Animator = UnityEngine.Animator;

    [AddComponentMenu("")]
    public class AnimatorWindowFloatParameterListElement : Abstract.AnimatorWindowParameterListElement
    {
        [SerializeField]
        private FloatField floatField;

        private float Value
        {
            get => _value;
            set
            {
                if(_value != value)
                {
                    _value = value;

                    floatField.SetValueWithoutNotify(value);
                }
            }
        }

        private float _value;

        internal void Intialize(Animator animator, string parameter)
        {
            targetAnimator = animator;

            parameterHash = Animator.StringToHash(parameter);

            Value = targetAnimator.GetFloat(parameterHash);

            floatField.RegisterValueChangedCallback(this, nameof(OnValueChanged));
        }

        public void OnValueChanged()
        {
            targetAnimator.SetFloat(parameterHash, floatField.Value);
        }
    }
}
