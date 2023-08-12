using System.Collections.Generic;
using System.Linq;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private string prefabPath = "Prefabs/Enemy/MonumentMonster.prefab";
    [SerializeField] private int minionCount = 20;
    [SerializeField] private MonsterSpawnPositions positions;
    private HashSet<Transform> _occupiedPositions = new();
    
    private void Awake()
    {
        for (var i = 0; i < minionCount; i++)
        {
            Spawn(MonsterLevelType.Boss, MonsterItemType.Badge);
        }
    }
    
    private void Spawn(MonsterLevelType levelType, MonsterItemType itemType)
    {
        var handle = Addressables.InstantiateAsync(prefabPath);
        handle.Completed += op =>
        {
            // when prefab is instantiated
            var enemyObj = op.Result.gameObject;
            enemyObj.transform.position = GetPosition();
            var mm = enemyObj.GetComponentInChildren<MonumentMonster>();
            mm.SetItemType(itemType);
            mm.SetLevelType(levelType);
        };
    }

    private Vector3 GetPosition()
    {
        var randomPos = positions.positions.Where(p => _occupiedPositions.Contains(p) == false).PeekRandom();
        _occupiedPositions.Add(randomPos);
        return randomPos.position;
    }
}