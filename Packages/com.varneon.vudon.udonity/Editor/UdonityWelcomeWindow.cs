using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Varneon.VUdon.Udonity.Editor
{
    [InitializeOnLoad]
    public class UdonityWelcomeWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset windowUxml;

        [SerializeField]
        private Texture2D windowIcon;

        private static bool dataLocatorExists;

        private static bool isSpritePackingEnabled;

        static UdonityWelcomeWindow()
        {
            EditorApplication.delayCall += DelayedInitialization;
        }

        private static void DelayedInitialization()
        {
            UdonityDataLocator dataLocator = AssetDatabase.FindAssets("t:UdonityDataLocator").Select(a => AssetDatabase.LoadAssetAtPath<UdonityDataLocator>(AssetDatabase.GUIDToAssetPath(a))).FirstOrDefault();

            dataLocatorExists = dataLocator != null;

            SpritePackerMode spritePackerMode = EditorSettings.spritePackerMode;

            isSpritePackingEnabled = spritePackerMode == SpritePackerMode.AlwaysOnAtlas;

            if (!dataLocatorExists || !isSpritePackingEnabled)
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    EditorApplication.update += OpenWindowDelayed;

                    return;
                }

                GetWindow<UdonityWelcomeWindow>();
            }
        }

        private static void OpenWindowDelayed()
        {
            EditorApplication.update -= OpenWindowDelayed;

            GetWindow<UdonityWelcomeWindow>();
        }

        [MenuItem("Varneon/VUdon/Udonity/Welcome Window")]
        private static void OpenWindow() => GetWindow<UdonityWelcomeWindow>();

        private void OnEnable()
        {
            titleContent = new GUIContent("Udonity Editor", windowIcon);

            minSize = new Vector2(350f, 400f);

            windowUxml.CloneTree(rootVisualElement);

            rootVisualElement.Q<Button>("Link_GitHub").clicked += () => Application.OpenURL("https://github.com/Varneon");
            rootVisualElement.Q<Button>("Link_Twitter").clicked += () => Application.OpenURL("https://twitter.com/Varneon");
            rootVisualElement.Q<Button>("Link_Discord").clicked += () => Application.OpenURL("https://discord.gg/bPF9Ha6");
            rootVisualElement.Q<Button>("Link_Patreon").clicked += () => Application.OpenURL("https://www.patreon.com/Varneon");

            VisualElement enableSpritePackingAction = rootVisualElement.Q("Action_EnableSpritePacking");
            VisualElement loadBuiltinEditorIconsAction = rootVisualElement.Q("Action_LoadBuiltInEditorIcons");

            if (isSpritePackingEnabled) { HideElement(enableSpritePackingAction); }
            else { enableSpritePackingAction.Q<Button>().clicked += () => { EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas; HideElement(enableSpritePackingAction); }; }

            if (dataLocatorExists) { HideElement(loadBuiltinEditorIconsAction); }
            else { loadBuiltinEditorIconsAction.Q<Button>().clicked += () => { BuiltinEditorIconLoader.LoadBuiltinEditorIcons(); HideElement(loadBuiltinEditorIconsAction); }; }

            GameObject[] sceneRoots = SceneManager.GetActiveScene().GetRootGameObjects();

            bool hasUdonityEditorInScene = sceneRoots.Any(r => r.GetComponentInChildren<UdonityEditorDescriptor>()) || sceneRoots.Any(r => r.GetComponentInChildren<Udonity>());

            if(isSpritePackingEnabled && dataLocatorExists && !hasUdonityEditorInScene)
            {
                Button addUdonityEditorToSceneButton = rootVisualElement.Q<Button>("Button_AddUdonityEditorToScene");

                addUdonityEditorToSceneButton.RemoveFromClassList("hidden");
                addUdonityEditorToSceneButton.clicked += () =>
                {
                    UdonityEditorUtilities.AddUdonityEditorDescriptorToScene(true);
                    HideElement(addUdonityEditorToSceneButton);
                };
            }
        }

        private static void HideElement(VisualElement element)
        {
            element.AddToClassList("hidden");
        }
    }
}
