using UnityEngine;

namespace ProceduralPlanets
{
    public class RigidNoiseFilter : INoiseFilter
    {
        private readonly NoiseSettings.RigidNoiseSettings _settings;
        private readonly Noise _noise = new();

        public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
        {
            _settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            float noiseValue = 0;
            var frequency = _settings.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            for (var i = 0; i < _settings.numLayers; i++)
            {
                var v = 1 - Mathf.Abs(_noise.Evaluate(point * frequency + _settings.centre));
                v *= v;
                v *= weight;
                weight = Mathf.Clamp01(v * _settings.weightMultiplier);

                noiseValue += v * amplitude;
                frequency *= _settings.roughness;
                amplitude *= _settings.persistence;
            }

            noiseValue -= _settings.minValue;
            return noiseValue * _settings.strength;
        }
    }
}