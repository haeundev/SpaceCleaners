using System.Collections.Generic;
using System.Linq;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<AssetReference> monsterPrefabRefs;
    [SerializeField] private int minionCount = 30;
    [SerializeField] private MonsterSpawnPositions positions;
    private readonly HashSet<Transform> _occupiedPositions = new();
    
    private void Awake()
    {
        for (var i = 0; i < minionCount; i++)
        {
            Spawn();
        }
    }
    
    private void Spawn()
    {
        var handle = Addressables.InstantiateAsync(monsterPrefabRefs.PeekRandom());
        handle.Completed += op =>
        {
            // when prefab is instantiated
            var enemyObj = op.Result.gameObject;
            enemyObj.transform.position = GetRandomPositionFromPool();
        };
    }

    private Vector3 GetRandomPositionFromPool()
    {
        var randomPos = positions.positions.Where(p => _occupiedPositions.Contains(p) == false).PeekRandom();
        _occupiedPositions.Add(randomPos);
        return randomPos.position;
    }
}