using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Windows.Animator
{
    using Animator = UnityEngine.Animator;

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class AnimatorWindow : Windows.Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 300f);

        [SerializeField]
        internal AnimatorControllerDataStorage dataStorage;

        [SerializeField]
        private RectTransform layerListContainer;

        [SerializeField]
        private RectTransform layerPreviewContainer;

        [SerializeField]
        private RectTransform parameterListContainer;

        [SerializeField]
        private GameObject layerListItem;

        [SerializeField]
        private GameObject[] parameterListItems;

        private Animator selectedAnimator;

        private RuntimeAnimatorController selectedRuntimeAnimatorController;

        public void TryOpenAnimator(Animator animator)
        {
            Clear();

            selectedAnimator = animator;

            selectedRuntimeAnimatorController = selectedAnimator.runtimeAnimatorController;

            if(selectedRuntimeAnimatorController.GetType() == typeof(AnimatorOverrideController))
            {
                selectedRuntimeAnimatorController = ((AnimatorOverrideController)selectedRuntimeAnimatorController).runtimeAnimatorController;
            }

            Debug.Log(selectedRuntimeAnimatorController);

            if(!dataStorage.TryGetControllerIndex(selectedRuntimeAnimatorController, out int index)) { return; }

            string[] parameterNames = dataStorage.GetControllerParameterNames(index);

            int[] parameterTypes = dataStorage.GetControllerParameterTypes(index);

            for(int i = 0; i < parameterNames.Length; i++)
            {
                string name = parameterNames[i];

                int type = parameterTypes[i];

                GameObject newParameterListItem = Instantiate(parameterListItems[type], parameterListContainer, false);

                newParameterListItem.GetComponentInChildren<TextMeshProUGUI>().text = name;

                switch (type)
                {
                    case 0:
                        newParameterListItem.GetComponent<AnimatorWindowFloatParameterListElement>().Intialize(selectedAnimator, name);
                        break;
                    case 1:
                        newParameterListItem.GetComponent<AnimatorWindowIntParameterListElement>().Intialize(selectedAnimator, name);
                        break;
                    case 2:
                        newParameterListItem.GetComponent<AnimatorWindowBoolParameterListElement>().Intialize(selectedAnimator, name);
                        break;
                    case 3:
                        newParameterListItem.GetComponent<AnimatorWindowTriggerParameterListElement>().Intialize(selectedAnimator, name);
                        break;
                }
            }

            string[] layerNames = dataStorage.GetControllerLayerNames(index);

            for(int i = 0; i < layerNames.Length; i++)
            {
                GameObject newLayerListItem = Instantiate(layerListItem, layerListContainer, false);

                newLayerListItem.GetComponentInChildren<TextMeshProUGUI>().text = layerNames[i];

                newLayerListItem.GetComponentInChildren<Slider>().value = animator.GetLayerWeight(i);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < layerListContainer.childCount; i++)
            {
                Destroy(layerListContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < parameterListContainer.childCount; i++)
            {
                Destroy(parameterListContainer.GetChild(i).gameObject);
            }
        }
    }
}
