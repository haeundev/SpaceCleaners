using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonumentEnemySpawn : MonoBehaviour
{

    public GameObject rangeObject;
    BoxCollider rangeCollider;
    public GameObject MonumentMonster;

    private void Awake()
    {
        rangeCollider = rangeObject.GetComponent<BoxCollider>();
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;

        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X/2)*-1, range_X/2);
        range_Z = Random.Range ((range_Z/2)-1, range_Z/2);
        Vector3 RandomPosition = new Vector3(range_X,0f,range_Z);

        Vector3 respawnPosition = originPosition + RandomPosition;
        return respawnPosition;
    }

    private void Start()
    {
        StartCoroutine(RandomRespawn_Coroutine());
    }

    IEnumerator RandomRespawn_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            GameObject monumentmonster = Instantiate(MonumentMonster, Return_RandomPosition(),Quaternion.identity);
        }
    }


}