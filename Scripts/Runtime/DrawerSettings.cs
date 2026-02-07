using System.Collections.Generic;
using System.IO;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace UI.Themes
{
    #region DRAWER SETTINGS
    [CreateAssetMenu(fileName = "DrawerSettings", menuName = "UI/Themes/Drawer Settings")]
    public class DrawerSettings : ScriptableObject
    {
        public string DefaultPath;
    }
    #endregion

    #region DRAWER
    public static class Drawer
    {
        public static string LanguageKey;
        public static float CharacterSpacing;
        public static float WordSpacing;
        public static TMP_FontAsset FontAsset;

        static DrawerSettings Settings;
        static UnityEvent OnThemeChange;
        static Dictionary<string, Element.Data> ThemeData;

        public static void Prepare(DrawerSettings settings)
        {
            OnThemeChange = new UnityEvent();
            ThemeData = new Dictionary<string, Element.Data>();

            SetTheme(Manifest.Cast(Resources.Load<TextAsset>(settings.DefaultPath).text), settings.DefaultPath.Replace("manifest", ""), true);
        }
        public static void AddListener(UnityAction action) => OnThemeChange.AddListener(action);
        public static void Invoke() => OnThemeChange.Invoke();
        public static void RemoveListener(UnityAction action) => OnThemeChange.RemoveListener(action);
        public static void SetTheme(Manifest_V2 manifest, string path, bool fromResources)
        {
            LanguageKey = GetLanguageKey();
            CharacterSpacing = manifest.font.characterSpacing;
            WordSpacing = manifest.font.wordSpacing;
            FontAsset = LoadFont();

            //var color = manifest.font.color.ToColor();
            var color = Color.black;

            if (manifest.icons != null)
                for (int i = 0; i < manifest.icons.Length; i++)
                {
                    var icon = manifest.icons[i];
                    var sprite = TryLoadSprite(icon);
                    if (sprite)
                        ThemeData[icon.fileName.Replace(".png", "").ToLower()] = new TheIcon.IconData
                        {
                            Sprite = sprite
                        };
                }

            if (manifest.elements != null)
                for (int s = 0; s < manifest.elements.Length; s++)
                {
                    var info = manifest.elements[s];
                    var data = new Drawable.DrawData
                    {
                        Base = TryLoadSprite(info.@base),
                        Mask = TryLoadSprite(info.mask, info.@base),
                        Overlay = TryLoadSprite(info.overlay),

                        _Selectable = LoadSelectable(info.selectable, info.@base),
                        _Text = LoadText(info.text),
                    };

                    ThemeData[info.key] = data;
                }

            string GetLanguageKey()
            {
                if (string.IsNullOrEmpty(manifest.languageKey))
                    return "default";

                return manifest.languageKey;
            }
            TMP_FontAsset LoadFont()
            {
                if (string.IsNullOrEmpty(manifest.font.assetName))
                    return TMP_Settings.defaultFontAsset;

                var result = TMP_Settings.defaultFontAsset;
                var filePath = path + manifest.font.assetName;

                if (fromResources)
                    result = Resources.Load<TMP_FontAsset>(filePath.Replace(".asset", ""));
                else if (File.Exists(filePath))
                {
                    var bundle = AssetBundle.LoadFromFile(filePath.Replace(".manifest", ""));

                    bundle.LoadAsset("tempfontholder");
                    result = bundle.LoadAsset<TMP_FontAsset>(bundle.GetAllAssetNames()[0]);
                    bundle.Unload(false);
                }

                return result ?? TMP_Settings.defaultFontAsset;
            }
            Sprite TryLoadSprite(SpriteData sprite, CustomSprite param = null)
            {
                if (sprite == null)
                    return null;

                if (param == null)
                    param = sprite as CustomSprite;

                var filePath = path + $"{sprite.fileName}";
                if (fromResources || File.Exists(filePath))
                {
                    var text = new Texture2D(2, 2);
                    if (fromResources)
                    {
                        filePath = filePath.Replace(".png", "");
                        text = Resources.Load<Texture2D>(filePath);

                        if (!text)
                            return null;
                    }
                    else if (text.LoadImage(File.ReadAllBytes(filePath)))
                    {
                        //text.alphaIsTransparency = true;
                        //text.Apply();
                    }
                    else
                        return null;

                    text.filterMode = (FilterMode)param.filterMode;
                    var result = Sprite
                        .Create(text,
                        new Rect(0f, 0f, text.width, text.height),
                        new Vector2(text.width / 2f, text.height / 2f),
                        (param.pixelPerUnit > 0 ? param.pixelPerUnit : 1),
                        0,
                        SpriteMeshType.FullRect,
                        new Vector4(param.borders.left, param.borders.bottom, param.borders.right, param.borders.top),
                        false);

                    result.name = sprite.fileName;

                    return result;
                }

                return null;
            }
            Drawable.DrawData.Text LoadText(TextData text)
            {
                if (text == null)
                    return null;

                return new Drawable.DrawData.Text
                {
                    FontSize = text.fontSize,
                    Color = text.color.magnitude > 0.0001f ? text.color : color,
                    Offset = new Vector3(text.xOffset, text.yOffset),
                };
            }
            Drawable.DrawData.Selectable LoadSelectable(SelectableData selectable, CustomSprite @base)
            {
                if (selectable == null)
                    return null;

                return new Drawable.DrawData.Selectable
                {
                    Transition = selectable.transition == 0 ? UnityEngine.UI.Selectable.Transition.ColorTint : UnityEngine.UI.Selectable.Transition.SpriteSwap,

                    NormalColor = selectable.normalColor.ToColor(),
                    HighlightedColor = selectable.highlightedColor.ToColor(),
                    PressedColor = selectable.pressedColor.ToColor(),
                    SelectedColor = selectable.selectedColor.ToColor(),
                    DisabledColor = selectable.disabledColor.ToColor(),

                    HighlightedSprite = TryLoadSprite(selectable.highlightedSprite, @base),
                    PressedSprite = TryLoadSprite(selectable.pressedSprite, @base),
                    SelectedSprite = TryLoadSprite(selectable.selectedSprite, @base),
                    DisabledSprite = TryLoadSprite(selectable.disabledSprite, @base),
                };
            }
        }
        public static bool TryGetData(string key, out Element.Data data) => ThemeData.TryGetValue(key, out data);
    }
    #endregion
}