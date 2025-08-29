using CoreFramework.Random;
using UnityEditor;
using UnityEngine;

namespace CoreFramework.Tools
{
    public class SquirrelNoiseHeatMapWindow : EditorWindow
    {
        #region Fields

        /// <summary>Output texture width (pixels).</summary>
        [SerializeField] private int _width = 256;

        /// <summary>Output texture height (pixels).</summary>
        [SerializeField] private int _height = 256;

        /// <summary>Hash seed for feature placement.</summary>
        [SerializeField] private uint _seed;

        /// <summary>Noise type for cell hashing.</summary>
        [SerializeField] private NoiseType _noiseType = NoiseType.MangledBitsBalancedMix;

        /// <summary>Color ramp for preview.</summary>
        [SerializeField] private Gradient _gradient;

        /// <summary>If true, previews auto-fit to window width.</summary>
        [SerializeField] private bool _autoFit = true;

        /// <summary>When Auto-Fit is off, each preview width = _width * this scale.</summary>
        [SerializeField] private int _previewScale = 2;

        /// <summary>Scroll position for the preview list.</summary>
        private Vector2 _scroll;

        // Textures
        private Texture2D _tex1D32;
        private Texture2D _tex1D64;
        private Texture2D _tex2D32;
        private Texture2D _tex2D64;
        private Texture2D _tex3D32;
        private Texture2D _tex3D64;

        // Buffers
        private float[] _buf1D32;
        private float[] _buf1D64;
        private float[] _buf2D32;
        private float[] _buf2D64;
        private float[] _buf3D32;
        private float[] _buf3D64;

        #endregion

        #region Unity Messages

        /// <summary>Menu entry.</summary>
        [MenuItem("Core Framework/Tools/Squirrel Noise Heatmap")]
        private static void Open() =>GetWindow<SquirrelNoiseHeatMapWindow>("Squirrel Noise Heatmap");
        
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
            _seed = (uint)EditorGUILayout.LongField("Seed (uint)", _seed);
            _noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", _noiseType);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);
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
                    NoiseEditorHelper.SavePng(_tex1D32, $"Noise 1D 32 Bit {_noiseType}");
                    NoiseEditorHelper.SavePng(_tex1D64, $"Noise 1D 64 Bit {_noiseType}");
                    NoiseEditorHelper.SavePng(_tex2D32, $"Noise 2D 32 Bit {_noiseType}");
                    NoiseEditorHelper.SavePng(_tex2D64, $"Noise 2D 64 Bit {_noiseType}");
                    NoiseEditorHelper.SavePng(_tex3D32, $"Noise 3D 32 Bit {_noiseType}");
                    NoiseEditorHelper.SavePng(_tex3D64, $"Noise 3D 64 Bit {_noiseType}");
                    Repaint();
                }
            }

            EditorGUILayout.Space();

            // --- Scrollable previews ---
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
            NoiseEditorHelper.DrawRow($"Noise 1D 32 Bit {_noiseType}", _tex1D32, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow($"Noise 1D 64 Bit {_noiseType}", _tex1D64, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow($"Noise 2D 32 Bit {_noiseType}", _tex2D32, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow($"Noise 2D 64 Bit {_noiseType}", _tex2D64, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow($"Noise 3D 32 Bit {_noiseType}", _tex3D32, _autoFit, position.width, _previewScale);
            NoiseEditorHelper.DrawRow($"Noise 3D 64 Bit {_noiseType}", _tex3D64, _autoFit, position.width, _previewScale);
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Methods

        /// <summary>Allocate buffers & textures for the configured dimensions.</summary>
        private void Allocate()
        {
            var width2 = _width * _width;     // flatten (x,z) into a single row span
            var len2D  = _width * _height;    // w×h
            var len3D  = _height * width2;    // (y rows) × (x,z columns)

            // 1D buffers: length = width
            if (_buf1D32 == null || _buf1D32.Length != _width) _buf1D32 = new float[_width];
            if (_buf1D64 == null || _buf1D64.Length != _width) _buf1D64 = new float[_width];

            // 2D buffers: length = width * height
            if (_buf2D32 == null || _buf2D32.Length != len2D) _buf2D32 = new float[len2D];
            if (_buf2D64 == null || _buf2D64.Length != len2D) _buf2D64 = new float[len2D];

            // 3D buffers flattened to a (height rows) × (width*width columns) image
            if (_buf3D32 == null || _buf3D32.Length != len3D) _buf3D32 = new float[len3D];
            if (_buf3D64 == null || _buf3D64.Length != len3D) _buf3D64 = new float[len3D];

            // Textures
            _tex1D32 = NoiseEditorHelper.EnsureTex(_tex1D32, _width, 1);
            _tex1D64 = NoiseEditorHelper.EnsureTex(_tex1D64, _width, 1);
            _tex2D32 = NoiseEditorHelper.EnsureTex(_tex2D32, _width, _height);
            _tex2D64 = NoiseEditorHelper.EnsureTex(_tex2D64, _width, _height);
            _tex3D32 = NoiseEditorHelper.EnsureTex(_tex3D32, width2, _height); // columns = (z*_width + x)
            _tex3D64 = NoiseEditorHelper.EnsureTex(_tex3D64, width2, _height);
        }

        /// <summary>Compute F1/F2 and edges for both metrics, write into textures.</summary>
        private void Regenerate()
        {
            // --- 1D rows (x axis only) ---
            for (var x = 0; x < _width; x++)
            {
                _buf1D32[x] = SquirrelNoise32Bit.Get1DNoise01(x, _seed, _noiseType);
                _buf1D64[x] = SquirrelNoise64Bit.Get1DNoise01((ulong)x, _seed, _noiseType);
            }

            var width2 = _width * _width;

            // --- 2D & 3D ---
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    // 2D linear index (row-major)
                    var i2D = y * _width + x;

                    _buf2D32[i2D] = SquirrelNoise32Bit.Get2DNoise01(x, y, _seed, _noiseType);
                    _buf2D64[i2D] = SquirrelNoise64Bit.Get2DNoise01((ulong)x, (ulong)y, _seed, _noiseType);

                    // Pack 3D as a strip of Z-slices next to each other on the X axis:
                    // tex width = width2, so column = z*_width + x, row = y
                    for (var z = 0; z < _width; z++)
                    {
                        var col3D = z * _width + x;           // 0..(width2-1)
                        var i3D   = y * width2 + col3D;       // row-major over (height × width2)

                        _buf3D32[i3D] = SquirrelNoise32Bit.Get3DNoise01(x, y, z, _seed, _noiseType);
                        _buf3D64[i3D] = SquirrelNoise64Bit.Get3DNoise01((ulong)x, (ulong)y, (ulong)z, _seed, _noiseType);
                    }
                }
            }

            // Upload
            NoiseEditorHelper.ApplyToTexture(_tex1D32, _buf1D32, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_tex1D64, _buf1D64, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_tex2D32, _buf2D32, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_tex2D64, _buf2D64, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_tex3D32, _buf3D32, (0f, 1f), _gradient);
            NoiseEditorHelper.ApplyToTexture(_tex3D64, _buf3D64, (0f, 1f), _gradient);

            Repaint();
        }

        #endregion
    }
}