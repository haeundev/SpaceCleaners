using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudiosourceManager : MonoBehaviour
{
    public static AudiosourceManager instance;
    private List<AudioSource> _sources;
    private int _currentSourceIndex;

    private void Awake()
    {
        instance = this;

        CollectSources();
    }

    private void CollectSources()
    {
        _sources = GetComponentsInChildren<AudioSource>().ToList();
    }

    public void PlayClip(AudioClip clip)
    {
        var source = _sources[_currentSourceIndex];
        source.clip = clip;
        source.Play();

        if (_currentSourceIndex == _sources.Count - 1) // used the last one before
            _currentSourceIndex = 0;
        else
            _currentSourceIndex++;
    }
}