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

    private void Awake()
    {
        CollectLabels();
    }
    
    [Button]
    private void CollectLabels()
    {
        _labels = GameObject.Find("SpaceObjectLabelTypes").GetComponentsInChildren<SpaceObjectLabel>().ToList();
    }

    public void OnGazeEnter()
    {
        ShowOutline(true);
        ShowLabel(true);
    }

    private void ShowOutline(bool isShow)
    {
        gameObject.GetComponent<Outlinable>().enabled = isShow;
    }

    private void ShowLabel(bool isShow)
    {
        if (isShow)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Asteroid"))
            {
                var label = _labels.Single(p => p.type == SpaceObjectLabelType.Asteroid);
                label.Show(GetLabel(label.type));
                label.gameObject.SetActive(true);
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Planet"))
            {
                var label = _labels.Single(p => p.type == SpaceObjectLabelType.Planet);
                label.Show(GetLabel(label.type));
                label.gameObject.SetActive(true);
                
                // HEO TODO: go to different planet scene
                label.RegisterEnterButtonEvent(() =>
                {
                    ApplicationContext.Instance.LoadScene("JunglePlanet"); // SCENE NAME
                });
            }
        }
        else
        {
            _labels.ForEach(p => p.gameObject.SetActive(false));
        }
    }

    private string GetLabel(SpaceObjectLabelType type)
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