using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Directory where the icons will be saved
        /// </summary>
        public const string ICON_DIRECTORY = "Assets/Udonity/Builtin Editor Icons";

        [MenuItem("Varneon/VUdon/Udonity/Load Builtin Editor Icons")]
        internal static void LoadBuiltinEditorIcons()
        {
            HashSet<string> icons = new HashSet<string>(Resources.FindObjectsOfTypeAll<BuiltinEditorIconImage>().Select(i => i.IconName).Where(i => !string.IsNullOrWhiteSpace(i)));

            foreach(string iconName in icons)
            {
                try
                {
                    Texture2D icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;

                    Texture2D readableIcon = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount, true);

                    readableIcon.name = icon.name;

                    Graphics.CopyTexture(icon, readableIcon);

                    SaveIcon(readableIcon);
                }
                catch(Exception e)
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

                    importer.SaveAndReimport();
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();

            Sprite[] sprites = icons.Select(i => AssetDatabase.LoadAssetAtPath<Sprite>(GetIconSavePath(i))).ToArray();

            SpriteAtlas atlas = new SpriteAtlas();

            atlas.Add(sprites);

            AssetDatabase.CreateAsset(atlas, Path.Combine(ICON_DIRECTORY, "BuiltinEditorIconAtlas.spriteatlas"));

            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(atlas);
        }

        internal static void Initialize(this BuiltinEditorIconImage image)
        {
            image.GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(ICON_DIRECTORY, $"{image.IconName}.png"));

            UnityEngine.Object.DestroyImmediate(image);
        }

        private static void SaveIcon(Texture2D icon)
        {
            if (!Directory.Exists(ICON_DIRECTORY))
            {
                Directory.CreateDirectory(ICON_DIRECTORY);
            }

            File.WriteAllBytes(GetIconSavePath(icon.name), icon.EncodeToPNG());
        }

        private static string GetIconSavePath(string iconName) => Path.Combine(ICON_DIRECTORY, $"{iconName}.png");
    }
}
