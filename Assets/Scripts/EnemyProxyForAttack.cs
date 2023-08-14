using UnityEngine;

public class EnemyProxyForAttack : MonoBehaviour
{
    private MonumentMonster _monumentMonster;

    private void Awake()
    {
        _monumentMonster = transform.parent.GetComponentInChildren<MonumentMonster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _monumentMonster.OnPlayerEnterAttackProxy(other.gameObject);
            Debug.Log($"[Proxy] Player entered attack proxy");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            var weaponObj = other.gameObject;
            _monumentMonster.OnGetHit();
           
            Debug.Log($"[Proxy] Weapon OnTriggerEnter: {weaponObj.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        _monumentMonster.OnPlayerExitAttackProxy(other.gameObject);
        Debug.Log($"[Proxy] Player exited attack proxy");
    }
}