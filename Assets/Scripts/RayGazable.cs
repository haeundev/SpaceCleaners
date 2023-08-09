using System.Collections.Generic;
using System.Linq;
using EPOOutline;
using LiveLarson.BootAndLoad;
using Sirenix.OdinInspector;
using UnityEngine;

public class RayGazable : MonoBehaviour
{
    private List<SpaceObjectLabel> _labels;
    private string _generatedName = "";
    private bool _isNeverGazed = true;
    
    private void Start()
    {
        CollectLabels();
    }
    
    [Button]
    private void CollectLabels()
    {
        _labels = SpaceObjectLabels.Instance.labels;
        if (_labels.Count != 3)
        {
            Debug.LogError("why is it not 3?");
        }
    }

    public void OnGazeEnter()
    {
        ShowOutline(true);
        ShowLabel(true);
    }

    private void ShowOutline(bool isShow)
    {
        var outline = gameObject.GetComponent<Outlinable>();
        if (_isNeverGazed)
        {
            _isNeverGazed = false;
            outline.AddAllChildRenderersToRenderingList(RenderersAddingMode.MeshRenderer);
        }
        outline.enabled = isShow;
    }

    private void ShowLabel(bool isShow)
    {
        if (isShow)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Asteroid"))
            {
                var label = _labels.Single(p => p.type == SpaceObjectLabelType.Asteroid);
                label.Show(GetLabelText(label.type));
                label.gameObject.SetActive(true);
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Debris"))
            {
                var label = _labels.Single(p => p.type == SpaceObjectLabelType.Debris);
                var debrisLabel = label.GetComponent<DebrisLabel>();
                debrisLabel.SetDebris(gameObject);
                label.Show(GetLabelText(label.type));
                label.gameObject.SetActive(true);
                label.RegisterEnterButtonEvent(() =>
                {
                    OuterSpaceEvent.Trigger_ShootGadget(gameObject);
                });
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Planet"))
            {
                var label = _labels.Single(p => p.type == SpaceObjectLabelType.Planet);
                label.Show(GetLabelText(label.type));
                label.gameObject.SetActive(true);
                var planetInfo = gameObject.GetComponent<PlanetInfo>();
                
                // HEO TODO: go to different planet scene
                label.RegisterEnterButtonEvent(() =>
                {
                    ApplicationContext.Instance.LoadScene(planetInfo.SceneName); // SCENE NAME
                });
            }
        }
        else
        {
            _labels.ForEach(p => p.gameObject.SetActive(false));
        }
    }

    private string GetLabelText(SpaceObjectLabelType type)
    {
        if (type == SpaceObjectLabelType.Asteroid)
        {
            if (_generatedName == "")
                _generatedName = GenerateRandomAsteroidName();
            return _generatedName;
        }
        
        return gameObject.name;
    }

    private string GenerateRandomAsteroidName()
    {
        return "Asteroid #" + GetRandomAlphabet() + Random.Range(0, 10) + GetRandomAlphabet() + Random.Range(0, 10);
    }

    private char GetRandomAlphabet()
    {
        return (char)Random.Range(65, 91); // A-Z
    }

    public void OnGazeExit()
    {
        ShowOutline(false);
        ShowLabel(false);
    }

    public void OnGazeStay()
    {
    }
}