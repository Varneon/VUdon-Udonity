using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Editor
{
    /// <summary>
    /// Loader for builtin editor icons
    /// </summary>
    public static class BuiltinEditorIconLoader
    {
        private static UdonityDataLocator DataLocator
        {
            get
            {
                if (dataLocator == null)
                {
                    dataLocator = AssetDatabase.FindAssets("t:UdonityDataLocator").Select(a => AssetDatabase.LoadAssetAtPath<UdonityDataLocator>(AssetDatabase.GUIDToAssetPath(a))).FirstOrDefault();
                }

                return dataLocator;
            }
        }

        private static UdonityDataLocator dataLocator;

        private static string DataDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dataDirectory))
                {
                    dataDirectory = DataLocator == null ? DEFAULT_UDONITY_DATA_FOLDER : Path.GetDirectoryName(AssetDatabase.GetAssetPath(DataLocator));
                }

                return dataDirectory;
            }
        }

        private static string dataDirectory;

        private static string IconDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(iconDirectory))
                {
                    iconDirectory = Path.Combine(DataDirectory, ICON_FOLDER);
                }

                return iconDirectory;
            }
        }

        private static string iconDirectory;

        private const string DEFAULT_UDONITY_DATA_FOLDER = "Assets/Udonity";

        private const string ICON_FOLDER = "Builtin Editor Icons";

        internal static ImmutableArray<string> GetAllBuiltinEditorIconNames() => new HashSet<string>(Resources.FindObjectsOfTypeAll<BuiltinEditorIconImage>().Select(i => i.IconName).Where(i => !string.IsNullOrWhiteSpace(i))).ToImmutableArray();

        internal static ImmutableArray<string> GetLoadedBuiltinEditorIconNames() => Directory.GetFiles(IconDirectory, "*.png").Select(f => Path.GetFileNameWithoutExtension(f)).ToImmutableArray();

        internal static bool AreBuiltinEditorIconsLoaded()
        {
            if(DataLocator == null) { return false; }

            ImmutableArray<string> loadedIcons = GetLoadedBuiltinEditorIconNames();

            return GetAllBuiltinEditorIconNames().All(i => loadedIcons.Contains(i));
        }

        [MenuItem("Varneon/VUdon/Udonity/Load Builtin Editor Icons")]
        internal static void LoadBuiltinEditorIcons()
        {
            // Ensure that the icon directory exists before we start saving icons
            if (!Directory.Exists(IconDirectory))
            {
                Directory.CreateDirectory(IconDirectory);
            }

            ImmutableArray<string> icons = GetAllBuiltinEditorIconNames();

            foreach (string iconName in icons)
            {
                try
                {
                    Texture2D icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;

                    Texture2D readableIcon = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount, true);

                    readableIcon.name = icon.name;

                    Graphics.CopyTexture(icon, readableIcon);

                    SaveIcon(readableIcon);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            AssetDatabase.Refresh();

            try
            {
                TextureImporter[] importers = icons.Select(i => (TextureImporter)AssetImporter.GetAtPath(GetIconSavePath(i))).ToArray();

                AssetDatabase.StartAssetEditing();

                foreach (TextureImporter importer in importers)
                {
                    importer.textureType = TextureImporterType.Sprite;

                    importer.maxTextureSize = 64;

                    importer.textureCompression = TextureImporterCompression.Uncompressed;

                    importer.SaveAndReimport();
                }
            }
            finally
            {
                if (DataLocator == null)
                {
                    UdonityDataLocator newDataLocator = ScriptableObject.CreateInstance<UdonityDataLocator>();

                    AssetDatabase.CreateAsset(newDataLocator, Path.Combine(DataDirectory, string.Concat(nameof(UdonityDataLocator), ".asset")));
                }

                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();

            Sprite[] sprites = icons.Select(i => AssetDatabase.LoadAssetAtPath<Sprite>(GetIconSavePath(i))).ToArray();

            SpriteAtlas atlas = new SpriteAtlas();

            atlas.Add(sprites);

            AssetDatabase.CreateAsset(atlas, Path.Combine(IconDirectory, "BuiltinEditorIconAtlas.spriteatlas"));

            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(atlas);
        }

        internal static void Initialize(this BuiltinEditorIconImage image)
        {
            image.GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(IconDirectory, $"{image.IconName}.png"));

            UnityEngine.Object.DestroyImmediate(image);
        }

        private static void SaveIcon(Texture2D icon)
        {
            File.WriteAllBytes(GetIconSavePath(icon.name), icon.EncodeToPNG());
        }

        private static string GetIconSavePath(string iconName) => Path.Combine(IconDirectory, $"{iconName}.png");
    }
}
