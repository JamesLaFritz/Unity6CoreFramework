#region Header
// CellularHeatmapWindow.cs
// Author: James LaFritz
// Purpose: Visual heatmaps for Worley/Cellular (F1, F2, F2-F1) to sanity-check "wormy vs blocky" presets.
// Usage: Window ▶ CoreFramework ▶ Cellular Heatmap
#endregion

using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using CoreFramework.Random;
using static CoreFramework.Random.HashBasedNoiseUtils;

namespace CoreFramework.Tools
{
    /// <summary>
    /// Editor window that renders small heatmaps for Cellular/Worley diagnostics:
    /// F1, F2, and F2-F1 in both Euclidean (wormy) and Chebyshev (blocky) metrics.
    /// </summary>
    public sealed class CellularHeatmapWindow : EditorWindow
    {
        #region Fields

        /// <summary>Output texture width (pixels).</summary>
        [SerializeField] private int _width = 256;

        /// <summary>Output texture height (pixels).</summary>
        [SerializeField] private int _height = 256;

        /// <summary>World-space sampling origin.</summary>
        [SerializeField] private float2 _origin = new float2(0, 0);

        /// <summary>World units per pixel step along X/Y.</summary>
        [SerializeField] private float _pixelStep = 1f;

        /// <summary>Hash seed for feature placement.</summary>
        [SerializeField] private uint _seed;

        /// <summary>Noise type for cell hashing.</summary>
        [SerializeField] private NoiseType _noiseType = NoiseType.MangledBitsBalancedMix;

        /// <summary>Cells-per-world-unit multiplier.</summary>
        [SerializeField] private float _frequency = 0.01f;

        /// <summary>0=center; 1=random within cell.</summary>
        [SerializeField] private float _jitter = 1.0f;

        /// <summary>Visual scaling for F2-F1 edge map.</summary>
        [SerializeField] private float _edgeWidth = 0.12f;

        /// <summary>Normalize F1/F2 by observed min/max.</summary>
        [SerializeField] private bool _autoNormalize = true;

        /// <summary>Color ramp for preview.</summary>
        [SerializeField] private Gradient _gradient;

        /// <summary>If true, previews auto-fit to window width.</summary>
        [SerializeField] private bool _autoFit = true;

        /// <summary>When Auto-Fit is off, each preview width = _width * this scale.</summary>
        [SerializeField] private int _previewScale = 2;

        /// <summary>Scroll position for the preview list.</summary>
        private Vector2 _scroll;

        // Textures
        private Texture2D _texF1;
        private Texture2D _texF2;
        private Texture2D _texEdgesEuclid;
        private Texture2D _texEdgesCheby;
        private Texture2D _texEdgesManhattan;

        // Buffers
        private float[] _bufF1;
        private float[] _bufF2;
        private float[] _bufEdgesE;
        private float[] _bufEdgesC;
        private float[] _bufEdgesM;

        #endregion

        #region Unity Messages

        /// <summary>Menu entry.</summary>
        [MenuItem("Core Framework/Tools/Cellular Heatmap")]
        private static void Open() => GetWindow<CellularHeatmapWindow>("Cellular Heatmap");

        /// <summary>Initialize defaults and gradient.</summary>
        private void OnEnable()
        {
            if (_gradient == null)
            {
                var g = new[]
                {
                    new GradientColorKey(new Color(0,0,0), 0f),
                    new GradientColorKey(new Color(0,0,0.5f), 0.25f),
                    new GradientColorKey(new Color(0,1,1), 0.5f),
                    new GradientColorKey(new Color(1,1,0), 0.75f),
                    new GradientColorKey(new Color(1,1,1), 1f),
                };
                var a = new[] { new GradientAlphaKey(1, 0f), new GradientAlphaKey(1, 1f) };
                _gradient = new Gradient();
                _gradient.SetKeys(g, a);
            }

            Allocate();
            Regenerate();
        }

        /// <summary>Draw the UI and scrollable previews.</summary>
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Sampling", EditorStyles.boldLabel);
            _width = Mathf.Clamp(EditorGUILayout.IntField("Width", _width), 32, 2048);
            _height = Mathf.Clamp(EditorGUILayout.IntField("Height", _height), 32, 2048);
            _pixelStep = Mathf.Max(0.0001f, EditorGUILayout.FloatField("Pixel Step (world units)", _pixelStep));
            _origin.x = EditorGUILayout.FloatField("Origin X", _origin.x);
            _origin.y = EditorGUILayout.FloatField("Origin Y", _origin.y);
            _seed = (uint)EditorGUILayout.LongField("Seed (uint)", _seed);
            _noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", _noiseType);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cellular Controls", EditorStyles.boldLabel);
            _frequency = Mathf.Max(1e-6f, EditorGUILayout.FloatField("Frequency", _frequency));
            _jitter = Mathf.Clamp01(EditorGUILayout.Slider("Jitter", _jitter, 0f, 1f));
            _edgeWidth = Mathf.Clamp(EditorGUILayout.FloatField("Edge Width (visual)", _edgeWidth), 1e-4f, 10f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);
            _autoNormalize = EditorGUILayout.Toggle("Auto Normalize", _autoNormalize);
            _autoFit = EditorGUILayout.Toggle("Auto-Fit Previews to Window", _autoFit);
            using (new EditorGUI.DisabledScope(_autoFit))
            {
                _previewScale = Mathf.Clamp(EditorGUILayout.IntField("Preview Scale", _previewScale), 1, 8);
            }
            _gradient = EditorGUILayout.GradientField("Gradient", _gradient);

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Regenerate"))
                {
                    Allocate();
                    Regenerate();
                }
                if (GUILayout.Button("Save PNGs"))
                {
                    NoiseEditorHelper.SavePng(_texF1, "F1");
                    NoiseEditorHelper.SavePng(_texF2, "F2");
                    NoiseEditorHelper.SavePng(_texEdgesEuclid, "Edges_Euclidean");
                    NoiseEditorHelper.SavePng(_texEdgesCheby, "Edges_Chebyshev");
                    NoiseEditorHelper.SavePng(_texEdgesManhattan, "Edges_Manhattan");
                    Repaint();
                }
            }

            EditorGUILayout.Space();

            // --- Scrollable previews ---
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
            NoiseEditorHelper.DrawRow("F1 (nearest distance)", _texF1, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow("F2 (second-nearest)", _texF2, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow("Edges (F2 - F1) — Euclidean (wormy)", _texEdgesEuclid, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow("Edges (F2 - F1) — Chebyshev (blocky)", _texEdgesCheby, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow("Edges (F2 - F1) — Manhattan (diamond)", _texEdgesManhattan, _autoFit, position.width, _previewScale);
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Methods

        /// <summary>Allocate buffers & textures for the configured dimensions.</summary>
        private void Allocate()
        {
            var len = _width * _height;

            if (_bufF1 == null || _bufF1.Length != len) _bufF1 = new float[len];
            if (_bufF2 == null || _bufF2.Length != len) _bufF2 = new float[len];
            if (_bufEdgesE == null || _bufEdgesE.Length != len) _bufEdgesE = new float[len];
            if (_bufEdgesC == null || _bufEdgesC.Length != len) _bufEdgesC = new float[len];
            if (_bufEdgesM == null || _bufEdgesM.Length != len) _bufEdgesM = new float[len];

            _texF1 = NoiseEditorHelper.EnsureTex(_texF1, _width, _height);
            _texF2 = NoiseEditorHelper.EnsureTex(_texF2, _width, _height);
            _texEdgesEuclid = NoiseEditorHelper.EnsureTex(_texEdgesEuclid, _width, _height);
            _texEdgesCheby = NoiseEditorHelper.EnsureTex(_texEdgesCheby, _width, _height);
            _texEdgesManhattan = NoiseEditorHelper.EnsureTex(_texEdgesManhattan, _width, _height);
        }

        /// <summary>Compute F1/F2 and edges for both metrics, write into textures.</summary>
        private void Regenerate()
        {
            float minF1 = float.PositiveInfinity, maxF1 = float.NegativeInfinity;
            float minF2 = float.PositiveInfinity, maxF2 = float.NegativeInfinity;
            float minE = float.PositiveInfinity, maxE = float.NegativeInfinity;
            float minC = float.PositiveInfinity, maxC = float.NegativeInfinity;
            float minM = float.PositiveInfinity, maxM = float.NegativeInfinity;

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    var i = y * _width + x;
                    var p = _origin + new float2(x * _pixelStep, y * _pixelStep);

                    var rE = SquirrelNoise32Bit.Cellular2D(p, _seed, _frequency, _jitter, CellularDistance.Euclidean, _noiseType);
                    var rC = SquirrelNoise32Bit.Cellular2D(p, _seed, _frequency, _jitter, CellularDistance.Chebyshev, _noiseType);
                    var rM = SquirrelNoise32Bit.Cellular2D(p, _seed, _frequency, _jitter, CellularDistance.Manhattan, _noiseType);

                    _bufF1[i] = rE.F1;
                    _bufF2[i] = rE.F2;

                    var e = rE.F2 - rE.F1; // wormy
                    var c = rC.F2 - rC.F1; // blocky
                    var m = rM.F2 - rM.F1; // blocky

                    if (_edgeWidth > 0f)
                    {
                        e = Mathf.Clamp01(e / _edgeWidth);
                        c = Mathf.Clamp01(c / _edgeWidth);
                        m = Mathf.Clamp01(m / _edgeWidth);
                    }

                    _bufEdgesE[i] = e;
                    _bufEdgesC[i] = c;
                    _bufEdgesM[i] = m;

                    if (rE.F1 < minF1) minF1 = rE.F1; if (rE.F1 > maxF1) maxF1 = rE.F1;
                    if (rE.F2 < minF2) minF2 = rE.F2; if (rE.F2 > maxF2) maxF2 = rE.F2;
                    if (e < minE) minE = e; if (e > maxE) maxE = e;
                    if (c < minC) minC = c; if (c > maxC) maxC = c;
                    if (m < minM) minM = m; if (c > maxM) maxM = m;
                }
            }

            NoiseEditorHelper.ApplyToTexture(_texF1, _bufF1, _autoNormalize ? (minF1, maxF1) : (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_texF2, _bufF2, _autoNormalize ? (minF2, maxF2) : (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_texEdgesEuclid, _bufEdgesE, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_texEdgesCheby, _bufEdgesC, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_texEdgesManhattan, _bufEdgesM, (0f, 1f), _gradient);

            Repaint();
        }

        #endregion
    }
}