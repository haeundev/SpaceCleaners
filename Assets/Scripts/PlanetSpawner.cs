using System.Collections.Generic;
using EPOOutline;
using LiveLarson.Util;
using ProceduralPlanets;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlanetSpawner : MonoBehaviour
{
    public static PlanetSpawner Instance;
    private GameObject _currentSpawnedPlanet;
    [SerializeField] private List<AssetReference> planetRefs;
    [SerializeField] private List<Transform> spawnPoints;

    private void Awake()
    {
        Instance = this;
        OuterSpaceEvent.PlayerPositionWrapped += OnPlayerPositionWrapped;
        OuterSpaceEvent.PlayerRefusePlanet += OnPlayerRefusePlanet;
    }

    private void OnPlayerPositionWrapped()
    {
        RandomSpawnPlanet();
    }

    /// test
    private void Start()
    {
        // RandomSpawnPlanet();
    }

    private void RandomSpawnPlanet()
    {
        if (_currentSpawnedPlanet != default)
            Destroy(_currentSpawnedPlanet);

        var randomPoint = spawnPoints.PeekRandom();
        var randomModel = planetRefs.PeekRandom();
        var handle = Addressables.InstantiateAsync(randomModel);
        handle.Completed += op =>
        {
            var planetObj = op.Result;
            planetObj.GetComponent<PlanetGenerator>().GeneratePlanet();
            planetObj.GetComponent<PlanetGenerator>().GeneratePlanet();
            planetObj.AddComponent<RayGazable>();
            var outlinable = planetObj.AddComponent<Outlinable>();
            outlinable.RenderStyle = RenderStyle.FrontBack;
            outlinable.BackParameters.Enabled = false;
            outlinable.AddAllChildRenderersToRenderingList(RenderersAddingMode.MeshRenderer);
            planetObj.transform.position = randomPoint.position;
            _currentSpawnedPlanet = planetObj;
        };
    }

    private void OnPlayerRefusePlanet()
    {

    }
}