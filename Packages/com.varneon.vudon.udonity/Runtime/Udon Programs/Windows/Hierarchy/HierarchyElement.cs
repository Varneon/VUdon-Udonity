using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Editors;

namespace Varneon.VUdon.Udonity.Windows.Hierarchy
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HierarchyElement : UdonSharpBehaviour
    {
        public bool Expanded => expanded;

        public RectTransform ElementRectTransform => elementRectTransform;

        public GameObject Target => target;

        [SerializeField]
        private Hierarchy hierarchy;

        [SerializeField]
        private Button button;

        [SerializeField]
        private RectTransform elementRectTransform;

        [SerializeField]
        private Toggle activeToggle;

        [SerializeField]
        private Toggle expandToggle;

        [SerializeField]
        private RectTransform expandArrow;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField, HideInInspector]
        private Udonity udonity;

        private GameObject target;

        private int hierarchyDepth;

        private bool isInspected;

        private bool targetActiveSelf;

        private bool expanded;

        private Color
            inactiveTextColor = new Color(0.5f, 0.5f, 0.5f),
            activeInHierarchyColor = new Color(0.8f, 0.8f, 0.8f);

        private Color
            elementDefaultColor = new Color(1f, 1f, 1f, 0f),
            elementSelectedUnfocusedColor = new Color(0.3f, 0.3f, 0.3f);

        private bool waitingForSecondClick;

        public void OnToggleExpanded()
        {
            expanded = expandToggle.isOn;

            expandArrow.localEulerAngles = new Vector3(0f, 0f, expanded ? -90f : 0f);

            SetChildrenExpanded(expanded);
        }

        public void OnSelected()
        {
            if (waitingForSecondClick) { hierarchy.FrameObjectBySelection(); return; }

            if (isInspected) { return; }

            waitingForSecondClick = true;

            isInspected = true;

            button.Select();

            ColorBlock block = button.colors;

            block.normalColor = elementSelectedUnfocusedColor;

            button.colors = block;

            hierarchy.OnElementSelected(this);
        }

        internal void OnDeselected()
        {
            waitingForSecondClick = false;

            isInspected = false;

            button.OnDeselect(null);

            ColorBlock block = button.colors;

            block.normalColor = elementDefaultColor;

            button.colors = block;
        }

        public void OnBeginDrag()
        {
            udonity.Logger.Log($"OnBeginDrag: {target}");

            udonity.SetDraggedObject(target);
        }

        public void SetHierarchyDepth(int depth)
        {
            elementRectTransform.sizeDelta = new Vector2(depth * -20f, 0f);
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetChildrenExpanded(bool expanded)
        {
            if(!hierarchy.SetElementExpandedState(transform.GetSiblingIndex(), hierarchyDepth, expanded))
            {
                expandToggle.gameObject.SetActive(false);
            }
        }

        internal void Initialize(Transform targetTransform, int depth, bool hasChildren)
        {
            expandToggle.gameObject.SetActive(hasChildren);

            hierarchyDepth = depth;

            target = targetTransform.gameObject;

            targetActiveSelf = target.activeSelf;

            bool activeInHierarchy = target.activeInHierarchy;

            if (!activeInHierarchy)
            {
                nameText.color = inactiveTextColor;
            }

            activeToggle.SetIsOnWithoutNotify(targetActiveSelf);

            nameText.text = target.name;

            elementRectTransform.sizeDelta = new Vector2(depth * -16f, 0f);

            if(depth > 1)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Refreshes the hierarchy element
        /// </summary>
        /// <returns>Should the element be destroyed</returns>
        internal bool Refresh()
        {
            if(target == null)
            {
                return true;
            }

            bool active = target.activeSelf;

            if (targetActiveSelf != active)
            {
                nameText.color = active ? activeInHierarchyColor : inactiveTextColor;

                activeToggle.SetIsOnWithoutNotify(active);

                targetActiveSelf = active;

                hierarchy.SetElementActiveState(transform.GetSiblingIndex(), hierarchyDepth, active);
            }

            return false;
        }

        public void ToggleActive()
        {
            bool isActive = activeToggle.isOn;

            if (isInspected)
            {
                GameObjectEditor gameObjectEditor = hierarchy.Inspector.ActiveGameObjectEditor;

                if (gameObjectEditor)
                {
                    gameObjectEditor.SetActive(isActive);
                }
            }

            SetActive(isActive);
        }

        internal void SetActive(bool active)
        {
            nameText.color = active ? activeInHierarchyColor : inactiveTextColor;

            activeToggle.SetIsOnWithoutNotify(active);

            target.SetActive(active);

            targetActiveSelf = active;

            hierarchy.SetElementActiveState(transform.GetSiblingIndex(), hierarchyDepth, active);
        }

        internal void SetActiveInHierarchyState(bool active)
        {
            nameText.color = (targetActiveSelf && active) ? activeInHierarchyColor : inactiveTextColor;
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            udonity = GetComponentInParent<Udonity>();
        }
#endif
    }
}
