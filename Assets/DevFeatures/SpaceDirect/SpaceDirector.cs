using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceDirector : MonoBehaviour
{
    public SpaceDirector instance;
    
    // queue 대신 sorted dic ==> 
    private static readonly SortedDictionary<SpaceDirectType, ISpaceDirect> Directs = new();
    private static ISpaceDirect _currentDirect;
    
    private void Awake()
    {
        instance = this;
    }
    
    public static void Reserve(ISpaceDirect direct)
    {
        Debug.Log($"[Director] Reserved {direct.DirectType}");
        Directs.Add(direct.DirectType, direct);
    }

    private void LateUpdate()
    {
        if (_currentDirect == default) // if null
        {
            RunFirstDirect();
        }
    }

    private void RunFirstDirect()
    {
        var (type, direct) = Directs.First();
        direct.OnDirect(type);
        _currentDirect = direct;
    }
}