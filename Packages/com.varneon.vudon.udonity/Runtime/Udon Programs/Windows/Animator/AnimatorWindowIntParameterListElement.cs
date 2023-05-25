using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using Animator = UnityEngine.Animator;

    public class AnimatorWindowIntParameterListElement : Abstract.AnimatorWindowParameterListElement
    {
        [SerializeField]
        private FloatField floatField;

        private int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    floatField.SetValueWithoutNotify(value);
                }
            }
        }

        private int _value;

        internal void Intialize(Animator animator, string parameter)
        {
            targetAnimator = animator;

            parameterHash = Animator.StringToHash(parameter);

            Value = targetAnimator.GetInteger(parameterHash);

            floatField.RegisterValueChangedCallback(this, nameof(OnValueChanged));
        }

        public void OnValueChanged()
        {
            targetAnimator.SetInteger(parameterHash, (int)floatField.Value);
        }
    }
}
