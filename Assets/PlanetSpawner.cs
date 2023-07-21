using System.Collections.Generic;
using LiveLarson.Util;
using ProceduralPlanets;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] private List<AssetReference> planetRefs;
    [SerializeField] private List<Transform> spawnPoints;
    
    private void Start()
    {
        var randomPoint = spawnPoints.PeekRandom();
        foreach (var p in planetRefs)
        {
            var handle = Addressables.InstantiateAsync(p);
            handle.Completed += op =>
            {
                var planetObj = op.Result;
                planetObj.GetComponent<Planet>().GeneratePlanet();
                planetObj.GetComponent<Planet>().GeneratePlanet();
                planetObj.transform.position = randomPoint.position;
            };
        }
    }
}