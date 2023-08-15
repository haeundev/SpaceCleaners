using System;
using System.Collections.Generic;
using System.Linq;
using EPOOutline;
using LiveLarson.BootAndLoad;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayGazable : MonoBehaviour
{
    private List<SpaceObjectLabel> _labels;
    public List<SpaceObjectLabel> Labels
    {
        get
        {
            if (_labels == default || _labels.Count == 0)
                _labels = FindObjectOfType<SpaceObjectLabels>()?.labels;
            return _labels;
        }
    }
    
    private string _generatedName = "";
    private bool _isNeverGazed = true;

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
        if (Labels.Count != 3)
        {
            Debug.LogError("why is it not 3?");
            return;
        }
        
        if (isShow)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Asteroid"))
            {
                var label = Labels.Single(p => p.type == SpaceObjectLabelType.Asteroid);
                label.Show(GetLabelText(label.type));
                label.gameObject.SetActive(true);
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Debris"))
            {
                var label = Labels.Single(p => p.type == SpaceObjectLabelType.Debris);
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
                var label = Labels.Single(p => p.type == SpaceObjectLabelType.Planet);
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
            Labels.ForEach(p => p.gameObject.SetActive(false));
        }
    }

    private string GetLabelText(SpaceObjectLabelType type)
    {
        switch (type)
        {
            case SpaceObjectLabelType.Planet:
                return "Planet";
            case SpaceObjectLabelType.Asteroid:
                if (_generatedName == "")
                    _generatedName = GenerateRandomAsteroidName();
                return _generatedName;
            case SpaceObjectLabelType.Debris:
                return "Debris";
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