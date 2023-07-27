using UnityEngine;

public enum PlanetType
{
    Jungle,
    Monument,
    Earth
}

public class PlanetInfo : MonoBehaviour
{
    [SerializeField] private PlanetType planetType;

    public PlanetType PlanetType => planetType;
    public string SceneName { get; set; }

    private void Awake()
    {
        SceneName = planetType switch
        {
            // SCENE NAME
            PlanetType.Jungle => "JunglePlanet",
            PlanetType.Monument => "MonumentPlanet",
            _ => "EndingCutscene"
        };
    }
}