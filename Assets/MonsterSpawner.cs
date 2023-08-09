using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject theEnemy;
    public GameObject rangeObject;
    BoxCollider RangeCollider;
    public int enemyCount = 30 ;
    public int currentCount;

    private void Awake()    
    {
        RangeCollider = rangeObject.GetComponent<BoxCollider>();
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;
        float range_X = RangeCollider.bounds.size.x;
        float range_Z = RangeCollider.bounds.size.z;

        float yPosition =0f;

        range_X = Random.Range( (range_X/2)*-1, range_X/2);
        range_Z = Random.Range((range_Z/2)*-1, range_Z/2);
        Vector3 RandomPosition = new Vector3(range_X, 0f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPosition;

         RaycastHit hit;
    if (Physics.Raycast(respawnPosition + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
    {
        respawnPosition.y = hit.point.y + yPosition;
    }
        return respawnPosition;
    }
    void Start(){
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
         while (currentCount<enemyCount)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject instantCapsul = Instantiate(theEnemy, Return_RandomPosition(), Quaternion.identity);
            currentCount++;
        }

       /* while(enemyCount<10){
            xPos = Random.Range(-50,50);
            zPos = Random.Range(-31,31);
            Instantiate(theEnemy, new Vector3(xPos,2,zPos),Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1; */
                    }
    }



