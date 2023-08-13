using System;
using Pathfinding;
using UnityEngine;

public class EnemyProxyForFollow : MonoBehaviour
{
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private AIPath aiPath;
    private MonumentMonster _monumentMonster;
    private Transform _lookAtTarget;

    private void Awake()
    {
        _monumentMonster = transform.parent.GetComponentInChildren<MonumentMonster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        _monumentMonster.OnPlayerEnterFollowProxy(other.gameObject);
        aiPath.enabled = true;
        _lookAtTarget = other.transform;
        _monumentMonster.transform.LookAt(other.transform);
        FollowPlayer(other.transform);
        Debug.Log($"[Proxy] Player entered follow proxy");
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        _monumentMonster.OnPlayerExitFollowProxy(other.gameObject);
        aiPath.enabled = false;
        _lookAtTarget = default;
        FollowPlayer(default); // remove the target to follow
        Debug.Log($"[Proxy] Player exited follow proxy");
    }

    private void Update()
    {
        if (_lookAtTarget != default)
        {
            var monsterTransform = _monumentMonster.transform;
            var dir = _lookAtTarget.position - monsterTransform.position;
            dir.Normalize();
            dir.y = 0;
            _monumentMonster.transform.rotation = Quaternion.Lerp(_monumentMonster.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 0.1f);
        }
    }
    
    private void FollowPlayer(Transform target)
    {
        if (aiDestinationSetter == default)
        {
            Debug.LogError("aiDestinationSetter is null");
            return;
        }
        aiDestinationSetter.target = target;
    }
}