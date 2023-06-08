using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.ArrayExtensions;
using Varneon.VUdon.Logger.Abstract;
using Varneon.VUdon.Udonity.Editors;
using Varneon.VUdon.Udonity.Editors.Abstract;
using Varneon.VUdon.Udonity.Utilities;
using Varneon.VUdon.Udonity.Windows.Hierarchy;
using Varneon.VUdon.Udonity.Windows.UdonMonitor;
using VRC.Udon;

namespace Varneon.VUdon.Udonity.Windows.Inspector
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Inspector : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(300f, 200f);

        public GameObjectEditor ActiveGameObjectEditor => activeGameObjectEditor;

        [SerializeField, HideInInspector]
        internal UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase;

        [SerializeField, HideInInspector]
        internal Animator.AnimatorWindow animatorWindow;

        [SerializeField, HideInInspector]
        internal UdonMonitorWindow udonMonitorWindow;

        [SerializeField, HideInInspector]
        internal UdonProgramDataStorage udonProgramDataStorage;

        [SerializeField, HideInInspector]
        internal Type[] availableComponentEditorTypes;

        [SerializeField]
        private Handle handle;

        [SerializeField]
        private RectTransform container;

        [SerializeField]
        internal GameObjectEditor gameObjectEditor;

        [SerializeField]
        internal ComponentEditor[] componentEditors;

        [SerializeField]
        internal DefaultComponentEditor defaultComponentEditor;

        [SerializeField]
        internal DefaultComponentEditor defaultVRCInternalComponentEditor;

        [SerializeField]
        internal DefaultComponentEditor missingComponentEditor;

        [SerializeField]
        internal string[] availableMaterialEditors;

        [SerializeField]
        internal MaterialEditor[] materialEditors;

        [SerializeField]
        internal MaterialEditor materialEditor;

        [SerializeField]
        private CrashWatcher crashWatcher;

        [SerializeField]
        private UdonLogger logger;

        private GameObjectEditor activeGameObjectEditor;

        private bool[] collapsedComponents;

        private GameObject target;

        private void Start()
        {
            int componentEditorCount = componentEditors.Length;

            collapsedComponents = new bool[componentEditorCount];
        }

        internal void ClearContainer()
        {
            for(int i = 0; i < container.childCount; i++)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }

        internal void Initialize(HierarchyElement element)
        {
            target = element.Target;

            ClearContainer();

            bool hasMaterialEditors = false;

            activeGameObjectEditor = Instantiate(gameObjectEditor.gameObject, container, false).GetComponent<GameObjectEditor>();

            activeGameObjectEditor.Initialize(element);

            if(target == null)
            {
                logger.LogError(string.Format("[<color=#F00>Udonity</color>]: Inspected GameObject is null! Has the object been destroyed?"));

                return;
            }

            Component[] components = target.GetComponents<Component>();

            int componentCount = components.Length;

            ComponentEditor[] editors = new ComponentEditor[componentCount];

            for(int i = 0; i < componentCount; i++)
            {
                Component component = components[i];

                if(component == null)
                {
                    Instantiate(missingComponentEditor.gameObject, container, false).GetComponent<DefaultComponentEditor>().Initialize(null);

                    continue;
                }

                Type componentType = component.GetType();

                int editorIndex = availableComponentEditorTypes.IndexOf(componentType);

                ComponentEditor editor;

                if (componentType == typeof(UnityEngine.Animator))
                {
                    if(animatorWindow == null) { logger.LogError(string.Format("[<color=#F00>Udonity</color>]: AnimatorWindow is null!")); continue; }

                    animatorWindow.TryOpenAnimator((UnityEngine.Animator)component);
                }

                if (editorIndex >= 0)
                {
                    if(componentType == typeof(UdonBehaviour) && udonProgramDataStorage.TryGetProgramIndex((UdonBehaviour)component, out int index, out long id, out string name))
                    {
                        int udonSharpBehaviourEditorIndex = availableComponentEditorTypes.IndexOf(Type.GetType("UdonSharp.UdonSharpBehaviour"));

                        UdonSharpBehaviourEditor newUdonSharpBehaviourEditor = Instantiate(componentEditors[udonSharpBehaviourEditorIndex].gameObject, container, false).GetComponent<UdonSharpBehaviourEditor>();

                        newUdonSharpBehaviourEditor.Initialize(component, this, collapsedComponents[udonSharpBehaviourEditorIndex]);

                        newUdonSharpBehaviourEditor.InitializeUdonSharpBehaviourEditor(name, udonProgramDataStorage.GetProgramEntryPoints(index).Contains("_start"), udonProgramDataStorage.programScripts[index], udonProgramDataStorage.programScriptIcons[index]);
                    }

                    editor = Instantiate(componentEditors[editorIndex].gameObject, container, false).GetComponent<ComponentEditor>();

                    editor.Initialize(component, this, collapsedComponents[editorIndex]);

                    if(componentType == typeof(MeshRenderer))
                    {
                        hasMaterialEditors = true;

                        MeshRenderer meshRenderer = (MeshRenderer)component;

                        foreach (Material material in meshRenderer.sharedMaterials)
                        {
                            int shaderIndex = availableMaterialEditors.IndexOf(material.shader.name);

                            if(shaderIndex < 0)
                            {
                                MaterialEditor newMaterialEditor = Instantiate(materialEditor.gameObject, container, false).GetComponent<MaterialEditor>();

                                newMaterialEditor.Initialize(material, this, udonAssetDatabase);
                            }
                            else
                            {
                                MaterialEditor newMaterialEditor = Instantiate(materialEditors[shaderIndex].gameObject, container, false).GetComponent<MaterialEditor>();

                                newMaterialEditor.Initialize(material, this);
                            }
                        }
                    }
                }
                else
                {
                    editor = Instantiate(defaultComponentEditor.gameObject, container, false).GetComponent<DefaultComponentEditor>();

                    editor.Initialize(component);
                }

                editors[i] = editor;
            }

            if (hasMaterialEditors)
            {
                foreach (MaterialEditor editor in container.GetComponentsInChildren<MaterialEditor>())
                {
                    editor.transform.SetAsLastSibling();
                }
            }

            crashWatcher.SetComponentEditors(editors);

            handle.SetTarget(target.transform);

            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }

        internal void AssignLogger(UdonLogger udonLogger)
        {
            logger = udonLogger;
        }

        internal void SetTypeCollapsedState(Type type, bool collapsed)
        {
            int index = availableComponentEditorTypes.IndexOf(type);

            if(index < 0) { return; }

            collapsedComponents[index] = collapsed;
        }

        internal void OnEditorCrash(ComponentEditor editor)
        {
            logger.LogError(string.Format("[<color=#F00>Udonity</color>]: Component inspector <color=#888>{0}</color> has crashed!", editor.name));

            Destroy(editor.gameObject);

            // If the target is null after a crash, the inspected object has likely been destroyed
            if(target == null)
            {
                ClearContainer();
            }
        }

        internal void RebuildLayout()
        {
            LayoutGroup[] layoutGroups = GetComponentsInChildren<LayoutGroup>(true);

            for(int i = layoutGroups.Length - 1; i >= 0; i--)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i].GetComponent<RectTransform>());
            }
        }
    }
}
