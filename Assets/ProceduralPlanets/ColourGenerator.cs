﻿using UnityEngine;

namespace ProceduralPlanets
{
    public class ColourGenerator
    {
        private ColourSettings _settings;
        private Texture2D _texture;
        private const int TextureResolution = 50;
        private INoiseFilter _biomeNoiseFilter;
        private static readonly int TextureProperty = Shader.PropertyToID("_texture");
        private static readonly int ElevationMinMaxProperty = Shader.PropertyToID("_elevationMinMax");

        public void UpdateSettings(ColourSettings settings)
        {
            _settings = settings;
            if (_texture == null || _texture.height != settings.biomeColourSettings.biomes.Length)
                _texture = new Texture2D(TextureResolution * 2, settings.biomeColourSettings.biomes.Length,
                    TextureFormat.RGBA32, false);
            _biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            _settings.planetMaterial.SetVector(ElevationMinMaxProperty,
                new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
        {
            var heightPercent = (pointOnUnitSphere.y + 1) / 2f;
            heightPercent += (_biomeNoiseFilter.Evaluate(pointOnUnitSphere) - _settings.biomeColourSettings.noiseOffset) *
                             _settings.biomeColourSettings.noiseStrength;
            float biomeIndex = 0;
            var numBiomes = _settings.biomeColourSettings.biomes.Length;
            var blendRange = _settings.biomeColourSettings.blendAmount / 2f + .001f;

            for (var i = 0; i < numBiomes; i++)
            {
                var dst = heightPercent - _settings.biomeColourSettings.biomes[i].startHeight;
                var weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
                biomeIndex *= 1 - weight;
                biomeIndex += i * weight;
            }

            return biomeIndex / Mathf.Max(1, numBiomes - 1);
        }

        public void UpdateColours()
        {
            var colours = new Color[_texture.width * _texture.height];
            var colourIndex = 0;
            foreach (var biome in _settings.biomeColourSettings.biomes)
                for (var i = 0; i < TextureResolution * 2; i++)
                {
                    Color gradientCol;
                    if (i < TextureResolution)
                        gradientCol = _settings.oceanColour.Evaluate(i / (TextureResolution - 1f));
                    else
                        gradientCol = biome.gradient.Evaluate((i - TextureResolution) / (TextureResolution - 1f));
                    var tintCol = biome.tint;
                    colours[colourIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                    colourIndex++;
                }

            _texture.SetPixels(colours);
            _texture.Apply();
            _settings.planetMaterial.SetTexture(TextureProperty, _texture);
        }
    }
}