using UnityEngine;
using UnityEditor;
using System;

namespace Pixyz.Commons.Utilities
{
    public class TexturePackerWindow : EditorWindow
    {
        private Texture2D _srcTexture = null;
        private TexturePacker.ChannelInfo _red = new TexturePacker.ChannelInfo(0, null);
        private TexturePacker.ChannelInfo _green = new TexturePacker.ChannelInfo(1, null);
        private TexturePacker.ChannelInfo _blue = new TexturePacker.ChannelInfo(2, null);
        private TexturePacker.ChannelInfo _alpha = new TexturePacker.ChannelInfo(3, null);

        private Vector2 _pos;
        private Action _operation;

        //[MenuItem("Pixyz/TexturePacker test", false, 1)]
        public static void Open()
        {
            var window = GetWindow<TexturePackerWindow>();
            window.titleContent = new GUIContent("Texture packer");
            window.Focus();
            window.Repaint();
            window.InitChannels();
        }
        public void OnGUI()
        {
            _srcTexture = EditorGUILayout.ObjectField("Texture", _srcTexture, typeof(Texture2D), false) as Texture2D;

            if (GUILayout.Button("Init Textures"))
            {
                InitChannels();
            }

            if (GUILayout.Button("Execute Unpack"))
            {
                TexturePacker.UnPack(_srcTexture, _red, _green, _blue, _alpha);
            }

            if (GUILayout.Button("Execute pack"))
            {
                TexturePacker.Pack(out _srcTexture, _red, _green, _blue, _alpha);
            }

            if (Event.current.type == EventType.Repaint && _operation != null)
            {
                _operation.Invoke();
                _operation = null;
            }

            _pos = EditorGUILayout.BeginScrollView(_pos);

            if (_srcTexture != null)
            {
                EditorGUILayout.LabelField("Src Tex");
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(512), GUILayout.Height(512));
                GUI.DrawTexture(r, _srcTexture, ScaleMode.ScaleToFit);
            }

            if (_red != null && _red.texture != null)
            {
                EditorGUILayout.LabelField("Red Tex");
                _red.texture = EditorGUILayout.ObjectField("Texture", _red.texture, typeof(Texture2D), false) as Texture2D;
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(512), GUILayout.Height(512));
                GUI.DrawTexture(r, _red.texture, ScaleMode.ScaleToFit);
            }

            if (_green != null && _green.texture != null)
            {
                EditorGUILayout.LabelField("Green Tex");
                _green.texture = EditorGUILayout.ObjectField("Texture", _green.texture, typeof(Texture2D), false) as Texture2D;
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(512), GUILayout.Height(512));
                GUI.DrawTexture(r, _green.texture, ScaleMode.ScaleToFit);
            }

            if (_blue != null && _blue.texture != null)
            {
                EditorGUILayout.LabelField("Blue Tex");
                _blue.texture = EditorGUILayout.ObjectField("Texture", _blue.texture, typeof(Texture2D), false) as Texture2D;
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(512), GUILayout.Height(512));
                GUI.DrawTexture(r, _blue.texture, ScaleMode.ScaleToFit);
            }

            if (_alpha != null && _alpha.texture != null)
            {
                EditorGUILayout.LabelField("Alpha Tex");
                _alpha.texture = EditorGUILayout.ObjectField("Texture", _alpha.texture, typeof(Texture2D), false) as Texture2D;
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(512), GUILayout.Height(512));
                GUI.DrawTexture(r, _alpha.texture, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.EndScrollView();
        }

        public void InitChannels()
        {
            _red = new TexturePacker.ChannelInfo(0, new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false));
            _green = new TexturePacker.ChannelInfo(1, new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false));
            _blue = new TexturePacker.ChannelInfo(2, new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false));
            _alpha = new TexturePacker.ChannelInfo(3, new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false));

            for (int i = 0; i < 1024; ++i)
            {
                for (int j = 0; j < 1024; ++j)
                {
                    _red.texture.SetPixel(i, j, Color.red);
                    _green.texture.SetPixel(i, j, Color.green);
                    _blue.texture.SetPixel(i, j, Color.blue);
                    _alpha.texture.SetPixel(i, j, new Color(0, 0, 0, 1));
                }
            }
            _red.texture.Apply();
            _green.texture.Apply();
            _blue.texture.Apply();
            _alpha.texture.Apply();
        }
    }

}