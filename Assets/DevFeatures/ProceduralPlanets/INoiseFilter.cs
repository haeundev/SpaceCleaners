using UnityEngine;

namespace ProceduralPlanets
{
    public interface INoiseFilter {

        float Evaluate(Vector3 point);
    }
}
