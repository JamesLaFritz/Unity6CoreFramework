using System.IO;
using UnityEditor;
using UnityEngine;

namespace CoreFramework.Tools
{
    public static class NoiseEditorHelper
    {
        /// <summary>Create/resize a Texture2D.</summary>
        public static Texture2D EnsureTex(Texture2D tex, int w, int h)
        {
            if (tex != null && tex.width == w && tex.height == h) return tex;
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            return t;
        }

        /// <summary>Upload a scalar buffer to a texture using the gradient.</summary>
        public static void ApplyToTexture(Texture2D tex, float[] buffer, (float min, float max) range, Gradient gradient)
        {
            var (min, max) = range;
            var inv = (Mathf.Abs(max - min) < 1e-6f) ? 0f : 1f / (max - min);
            var cols = new Color[buffer.Length];

            for (var i = 0; i < buffer.Length; i++)
            {
                var t = inv == 0f ? 0f : Mathf.Clamp01((buffer[i] - min) * inv);
                cols[i] = gradient.Evaluate(t);
            }

            tex.SetPixels(cols);
            tex.Apply(false);
        }

        /// <summary>Draw a label and a scaled texture row inside the scroll view.</summary>
        public static void DrawRow(string rowTitle, Texture2D tex, bool autoFit, float windowWidth, int previewScale)
        {
            EditorGUILayout.LabelField(rowTitle, EditorStyles.miniBoldLabel);
            if (tex == null) return;

            var availW = autoFit
                ? Mathf.Max(64f, windowWidth - 30f) // window width minus margins/scrollbar
                : windowWidth * previewScale;

            var aspect = tex.height / (float)tex.width;
            var drawH = availW * aspect;

            // Reserve space and draw scaled
            var r = GUILayoutUtility.GetRect(availW, drawH, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, false);
            EditorGUILayout.Space(6f);
        }

        /// <summary>Save a texture as PNG via a file panel.</summary>
        public static void SavePng(Texture2D tex, string nameBase)
        {
            if (tex == null) return;
            var path = EditorUtility.SaveFilePanel("Save Heatmap PNG", Application.dataPath, $"{nameBase}.png", "png");
            if (string.IsNullOrEmpty(path)) return;

            var png = tex.EncodeToPNG();
            File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();
        }
    }
}