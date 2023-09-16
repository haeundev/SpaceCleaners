using System.Collections.Generic;
using UnityEngine;

public class SceneLoadedChecker : MonoBehaviour
{
    [SerializeField] public List<GameObject> shouldNotBeNull = new();
    
    private void Update()
    {
        if (shouldNotBeNull.TrueForAll(go => go != default))
        {
            OnComplete();
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnComplete()
    {
        Debug.Log("All GameObjects are not null");
    }
}