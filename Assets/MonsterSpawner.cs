using System;
using System.Collections.Generic;
using System.Linq;
using LiveLarson.Util;
using UniRx;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabRefs;
    [SerializeField] private int minionCount;
    [SerializeField] private MonsterSpawnPositions positions;
    private readonly HashSet<Transform> _occupiedPositions = new();

    private void Awake()
    {
        for (var i = 0; i < minionCount; i++) Spawn();
    }

    private void Start()
    {
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            MonumentEvents.Trigger_SceneLoaded(); // 꼭 여기 있을 이유 없음
        });
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