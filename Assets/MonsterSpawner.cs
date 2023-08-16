using System.Collections.Generic;
using System.Linq;
using LiveLarson.Util;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabRefs;
    [SerializeField] private int minionCount;
    [SerializeField] private MonsterSpawnPositions positions;
    private readonly HashSet<Transform> _occupiedPositions = new();

    private void Awake()
    {
        for (var i = 0; i < minionCount; i++)
        {
            Spawn();
        }
    }

    private void OnEnable()
    {
        // 이 부분 여기 있는 이유 명확하지 않음. 있을 이유 없음.
        MonumentEvents.Trigger_SceneLoaded();
    }

    private void Spawn()
    {
        var monster = Instantiate(monsterPrefabRefs.PeekRandom());
        monster.transform.position = GetRandomPositionFromPool();
    }

    private Vector3 GetRandomPositionFromPool()
    {
        var randomPos = positions.positions.Where(p => _occupiedPositions.Contains(p) == false).PeekRandom();
        _occupiedPositions.Add(randomPos);
        return randomPos.position;
    }
}