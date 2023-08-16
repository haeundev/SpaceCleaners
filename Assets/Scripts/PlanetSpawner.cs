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
    [SerializeField] private List<GameObject> planetRefs;
    [SerializeField] private List<Transform> spawnPoints;

    private void Awake()
    {
        Instance = this;
        // OuterSpaceEvent.PlayerPositionWrapped += OnPlayerPositionWrapped;
        OuterSpaceEvent.PlayerRefusePlanet += OnPlayerRefusePlanet;
    }

    public void SpawnPlanet(PlanetType planetType)
    {
        var index = 0;
        switch (planetType)
        {
            case PlanetType.Jungle:
                index = 0;
                break;
            case PlanetType.Monument:
                index = 1;
                break;
            case PlanetType.Earth:
                index = 2;
                Debug.Log("Not implemented yet");
                break;
        }

        if (_currentSpawnedPlanet != default)
            Destroy(_currentSpawnedPlanet);

        var randomPoint = spawnPoints.PeekRandom();
        var planet = Instantiate(planetRefs[index]);
        planet.GetComponent<PlanetGenerator>().GeneratePlanet();
        planet.GetComponent<PlanetGenerator>().GeneratePlanet();
        planet.AddComponent<RayGazable>();
        planet.AddComponent<Outlinable>();
        var outlinable = planet.GetComponent<Outlinable>();
        outlinable.RenderStyle = RenderStyle.FrontBack;
        outlinable.BackParameters.Enabled = false;
        outlinable.AddAllChildRenderersToRenderingList(RenderersAddingMode.MeshRenderer);
        planet.transform.position = randomPoint.position;
        _currentSpawnedPlanet = planet;
        Debug.Log($"[PlanetSpawner] Spawned planet {planetType.ToString()}");
    }

    // private void OnPlayerPositionWrapped()
    // {
    //     RandomSpawnPlanet();
    // }

    /// test
    private void Start()
    {
        // RandomSpawnPlanet();
    }

    // private void RandomSpawnPlanet()
    // {
    //     if (_currentSpawnedPlanet != default)
    //         Destroy(_currentSpawnedPlanet);
    //
    //     var randomPoint = spawnPoints.PeekRandom();
    //     var randomModel = planetRefs.PeekRandom();
    //     var handle = Addressables.InstantiateAsync(randomModel);
    //     handle.Completed += op =>
    //     {
    //         var planetObj = op.Result;
    //         planetObj.GetComponent<PlanetGenerator>().GeneratePlanet();
    //         planetObj.GetComponent<PlanetGenerator>().GeneratePlanet();
    //         planetObj.AddComponent<RayGazable>();
    //         var outlinable = planetObj.AddComponent<Outlinable>();
    //         outlinable.RenderStyle = RenderStyle.FrontBack;
    //         outlinable.BackParameters.Enabled = false;
    //         outlinable.AddAllChildRenderersToRenderingList(RenderersAddingMode.MeshRenderer);
    //         planetObj.transform.position = randomPoint.position;
    //         _currentSpawnedPlanet = planetObj;
    //     };
    // }

    private void OnPlayerRefusePlanet()
    {
    }
}