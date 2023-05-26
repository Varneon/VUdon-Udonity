using System;
using System.Diagnostics;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.ArrayExtensions;
using Varneon.VUdon.Logger.Abstract;
using VRC.SDKBase;

namespace Varneon.VUdon.Udonity.Windows.Hierarchy
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Hierarchy : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(200f, 200f);

        public Inspector.Inspector Inspector => inspector;

        public HierarchyElement ActiveHierarchyElement => activeHierarchyElement;

        [SerializeField, HideInInspector]
        internal Transform[] roots;

        [SerializeField]
        private Inspector.Inspector inspector;

        [SerializeField]
        private RectTransform listContainer;

        [SerializeField]
        private GameObject hierarchyElementGameObject;

        [SerializeField]
        private GameObject emptyGameObject;

        [SerializeField]
        private InputField searchInputField;

        [SerializeField]
        private UdonLogger logger;

        private HierarchyElement activeHierarchyElement;

        private int[] depthLookup;

        private HierarchyElement[] elements;

        private bool[] expandedStates;

        private int objectCount;

        private Udonity udonity;

        private bool recursiveSelectionHiglighting;

        private GameObject selectedGameObject;

        private int instantiationIndex;

        private Transform[] children;

        private int childCount;

        private Stopwatch stopwatch = new Stopwatch();

        private int elementIteratorIndex;

        private const long INSTANTIATION_BATCH_TIME_LIMIT = 5000;

        private const string LOG_PREFIX = "[<color=#00FFFF>Udonity</color>]:";

        private void Start()
        {
            if(hierarchyElementGameObject == null) { LogError("Fatal error: Hierarchy element is null and cannot be instantiated!"); }

            udonity = GetComponentInParent<Udonity>();

            children = new Transform[0];

            foreach(Transform root in roots)
            {
                if(root == null) { continue; }

                children = children.AddRange(root.GetComponentsInChildren<Transform>(true));
            }

            int firstInvalid = -1;

            int lastInvalid = -1;

            bool isInvalid = false;

            int indexOffset = 0;

            childCount = children.Length;

            for (int i = 0; i < childCount; i++)
            {
                int index = i - indexOffset;

                Transform t = children[index];

                if (VRC.SDKBase.Utilities.IsValid(t))
                {
                    if (isInvalid)
                    {
                        isInvalid = false;

                        lastInvalid = index;

                        int length = lastInvalid - firstInvalid;

                        children = children.RemoveRange(firstInvalid, length);

                        indexOffset += length;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if(!isInvalid)
                {
                    firstInvalid = index;

                    isInvalid = true;
                }
            }

            childCount = children.Length;

            objectCount = childCount;

            depthLookup = new int[objectCount];

            expandedStates = new bool[objectCount];

            elements = new HierarchyElement[objectCount];

            SendCustomEventDelayedFrames(nameof(InstantiateBatch), 0);
        }

        public void InstantiateBatch()
        {
            stopwatch.Restart();

            while(stopwatch.ElapsedMilliseconds < INSTANTIATION_BATCH_TIME_LIMIT && instantiationIndex < childCount)
            {
                Transform child = children[instantiationIndex];

                //HierarchyElement newElement = Instantiate(hierarchyElementGameObject, listContainer, false).GetComponent<HierarchyElement>();

                //elements[i] = newElement;

                //int depth = child.GetComponentsInParent<Transform>(true).Length;

                //newElement.Initialize(child, depth, child.childCount > 0);

                if (child)
                {
                    HierarchyElement newElement = CreateNewHierarchyElement(child, out int depth);

                    elements[instantiationIndex] = newElement;

                    depthLookup[instantiationIndex] = depth;
                }

                instantiationIndex++;
            }

            stopwatch.Stop();

            if (instantiationIndex < childCount)
            {
                SendCustomEventDelayedFrames(nameof(InstantiateBatch), 0);
            }
            else
            {
                IterateHierarchyElements();
            }
        }

        private HierarchyElement CreateNewHierarchyElement(Transform target, out int depth)
        {
            HierarchyElement newElement = Instantiate(hierarchyElementGameObject, listContainer, false).GetComponent<HierarchyElement>();

            depth = target.GetComponentsInParent<Transform>(true).Length;

            newElement.Initialize(target, depth, target.childCount > 0);

            return newElement;
        }

        internal void OnElementSelected(HierarchyElement element)
        {
            if(activeHierarchyElement == element) { return; }

            if (activeHierarchyElement)
            {
                SetTargetHiglightedState(false);

                activeHierarchyElement.OnDeselected();
            }

            activeHierarchyElement = element;

            selectedGameObject = element.Target;

            if(selectedGameObject == null)
            {
                logger.LogError(string.Format("[<color=#F00>Udonity</color>]: Inspected GameObject is null! Has the object been destroyed?"));

                int index = elements.IndexOf(element);

                depthLookup = depthLookup.RemoveAt(index);

                elements = elements.RemoveAt(index);

                expandedStates = expandedStates.RemoveAt(index);

                Destroy(element.gameObject);

                objectCount--;

                return;
            }

            udonity.OnGameObjectSelected(selectedGameObject);

            inspector.Initialize(element);

            Log($"Selected object: <color=#9CD>{selectedGameObject.name}</color> <color=#888>({selectedGameObject.GetInstanceID()})</color>");

            EnableTargetHighlight();
        }

        internal bool SetElementExpandedState(int index, int depth, bool expanded)
        {
            expandedStates[index] = expanded;

            int collapsedDepth = 0;

            bool isCollapsed = false;

            int currentDepth;

            int firstChildIndex = index + 1;

            // If the next object is on the same depth or higher, return false
            if (depthLookup[firstChildIndex] <= depth) { return false; }

            for (int i = firstChildIndex; i < objectCount; i++)
            {
                currentDepth = depthLookup[i];

                if (currentDepth <= depth) { return true; }

                if (isCollapsed)
                {
                    if (collapsedDepth < currentDepth) { continue; }
                    else { isCollapsed = false; }
                }

                HierarchyElement element = elements[i];

                if (element == null)
                {
                    LogError("Child HierarchyElement is null!");
                }
                else
                {
                    element.gameObject.SetActive(expanded);
                }

                if (expanded && !expandedStates[i])
                {
                    collapsedDepth = currentDepth;

                    isCollapsed = true;
                }
            }

            return true;
        }

        internal void SetElementActiveState(int index, int depth, bool active)
        {
            int collapsedDepth = 0;

            bool isInactiveInHierarchy = false;

            int currentDepth;

            for (int i = index + 1; i < objectCount; i++)
            {
                currentDepth = depthLookup[i];

                if (currentDepth <= depth) { return; }

                if (isInactiveInHierarchy)
                {
                    if (collapsedDepth < currentDepth) { continue; }
                    else { isInactiveInHierarchy = false; }
                }

                HierarchyElement element = elements[i];

                if(element == null) { RemoveHierarchyElementAtIndex(i); break; }

                element.SetActiveInHierarchyState(active);

                if (active && !expandedStates[i])
                {
                    collapsedDepth = currentDepth;

                    isInactiveInHierarchy = true;
                }
            }
        }

        public void IterateHierarchyElements()
        {
            if(elementIteratorIndex >= objectCount) { elementIteratorIndex = 0; }

            HierarchyElement element = elements[elementIteratorIndex];

            if(element == null)
            {
                if (selectedGameObject == null)
                {
                    RemoveHierarchyElementAtIndex(elementIteratorIndex);
                }
            }
            else
            {
                element.Refresh();
            }

            elementIteratorIndex++;

            SendCustomEventDelayedFrames(nameof(IterateHierarchyElements), 0);
        }

        private void RemoveHierarchyElementAtIndex(int index)
        {
            depthLookup = depthLookup.RemoveAt(index);

            elements = elements.RemoveAt(index);

            expandedStates = expandedStates.RemoveAt(index);

            objectCount--;
        }

        #region Initialization Methods
        internal void AssignLogger(UdonLogger udonLogger)
        {
            logger = udonLogger;
        }
        #endregion

        internal void FrameObjectBySelection()
        {
            udonity.FrameSelected();
        }

        #region Primitive Creation
        internal void CreateEmpty()
        {
            GameObject newGameObject = Instantiate(emptyGameObject);

            HierarchyElement newHierarchyElement = CreateNewHierarchyElement(newGameObject.transform, out int depth);

            elements = elements.Add(newHierarchyElement);

            depthLookup = depthLookup.Add(depth);

            expandedStates = expandedStates.Add(true);

            objectCount++;

            if (elements[0].Expanded) { newHierarchyElement.gameObject.SetActive(true); }
        }

        internal void CreateEmptyChild()
        {

        }

        internal void DuplicateObject()
        {
            if(selectedGameObject == null) { return; }

            Transform t = selectedGameObject.transform;

            GameObject duplicate = Instantiate(selectedGameObject, t.position, t.rotation, t.parent);

            int elementIndex = activeHierarchyElement.transform.GetSiblingIndex();

            AddRootToHierarchy(duplicate.transform, elementIndex);

            elementIteratorIndex = 0;
        }

        internal void DeleteObject()
        {
            if(selectedGameObject == null) { return; }

            Destroy(selectedGameObject);

            int selectedObjectIndex = elements.IndexOf(activeHierarchyElement);

            int selectedObjectDepth = depthLookup[selectedObjectIndex];

            int hierarchyCount = 0;

            int lookupIndex = selectedObjectIndex;

            int depthLookupLength = depthLookup.Length;

            while (depthLookup[lookupIndex] > selectedObjectDepth && lookupIndex < depthLookupLength)
            {
                lookupIndex = selectedObjectIndex + hierarchyCount++;
            }

            elements = elements.RemoveRange(selectedObjectIndex, hierarchyCount);

            depthLookup = depthLookup.RemoveRange(selectedObjectIndex, hierarchyCount);

            expandedStates = expandedStates.RemoveRange(selectedObjectIndex, hierarchyCount);

            objectCount -= hierarchyCount;

            elementIteratorIndex = 0;
        }
        #endregion

        #region Selected Object Highlighting
        private void SetTargetHiglightedState(bool highlighted)
        {
            if (selectedGameObject == null) { return; }

            if (recursiveSelectionHiglighting)
            {
                foreach (Renderer renderer in selectedGameObject.GetComponentsInChildren<Renderer>(true))
                {
                    if (!VRC.SDKBase.Utilities.IsValid(renderer)) { continue; }

                    InputManager.EnableObjectHighlight(renderer.gameObject, highlighted);
                }
            }
            else
            {
                InputManager.EnableObjectHighlight(selectedGameObject.gameObject, highlighted);
            }
        }

        public void EnableTargetHighlight()
        {
            if (selectedGameObject == null) { return; }

            SetTargetHiglightedState(true);

            SendCustomEventDelayedSeconds(nameof(EnableTargetHighlight), 1f);
        }

        internal void SetRecursiveHighlightingEnabled(bool enabled)
        {
            SetTargetHiglightedState(false);

            recursiveSelectionHiglighting = enabled;

            EnableTargetHighlight();
        }

        private void AddRootToHierarchy(Transform root)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);

            int oldObjectCount = objectCount;

            int foundObjectTransformCount = transforms.Length;

            objectCount += foundObjectTransformCount;

            depthLookup = depthLookup.AddRange(new int[foundObjectTransformCount]);

            expandedStates = expandedStates.AddRange(new bool[foundObjectTransformCount]);

            elements = elements.AddRange(new HierarchyElement[foundObjectTransformCount]);

            for (int i = 0; i < foundObjectTransformCount; i++)
            {
                Transform child = transforms[i];

                if (child)
                {
                    HierarchyElement newElement = CreateNewHierarchyElement(child, out int depth);

                    int index = oldObjectCount + i;

                    elements[index] = newElement;

                    depthLookup[index] = depth;
                }
            }

            elementIteratorIndex = 0;
        }

        private void AddRootToHierarchy(Transform root, int hierarchyIndex)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);

            int foundObjectTransformCount = transforms.Length;

            objectCount += foundObjectTransformCount;

            int currentDepth = depthLookup[hierarchyIndex];

            int insertionIndex = hierarchyIndex;

            while (insertionIndex < depthLookup.Length && depthLookup[insertionIndex] >= currentDepth)
            {
                insertionIndex++;
            }

            depthLookup = depthLookup.InsertRange(insertionIndex, new int[foundObjectTransformCount]);

            expandedStates = expandedStates.InsertRange(insertionIndex, new bool[foundObjectTransformCount]);

            elements = elements.InsertRange(insertionIndex, new HierarchyElement[foundObjectTransformCount]);

            for (int i = 0; i < foundObjectTransformCount; i++)
            {
                Transform child = transforms[i];

                if (child)
                {
                    HierarchyElement newElement = CreateNewHierarchyElement(child, out int depth);

                    newElement.name = "DUPLICATED";

                    if(depth <= currentDepth) { newElement.gameObject.SetActive(true); }

                    int index = insertionIndex + i;

                    newElement.transform.SetSiblingIndex(index);

                    elements[index] = newElement;

                    depthLookup[index] = depth;
                }
            }

            elementIteratorIndex = 0;
        }

        public void Search()
        {
            string searchInput = searchInputField.text;

            GameObject foundObject = GameObject.Find(searchInput);

            if(foundObject == null)
            {
                LogError($"Couldn't find object: '<color=#888>{searchInput}</color>'");

                return;
            }

            AddRootToHierarchy(foundObject.transform);
        }
        #endregion

        #region Logging Methods
        private void Log(string message)
        {
            if (logger)
            {
                logger.Log(string.Format("{0} {1}", LOG_PREFIX, message));
            }
        }

        private void LogError(string message)
        {
            if (logger)
            {
                logger.LogError(string.Format("{0} {1}", LOG_PREFIX, message));
            }
        }
        #endregion
    }
}
