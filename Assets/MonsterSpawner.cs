using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private int totalCount = 30;
    public GameObject theEnemy;
    public GameObject rangeObject;
    BoxCollider RangeCollider;
    public int enemyCount;
    private int _currentCount;

    private void Awake()    
    {
        RangeCollider = rangeObject.GetComponent<BoxCollider>();
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;
        float range_X = RangeCollider.bounds.size.x;
        float range_Z = RangeCollider.bounds.size.z;

        float yPosition = 1.0f;

        range_X = Random.Range( (range_X/2)*-1, range_X/2);
        range_Z = Random.Range((range_Z/2)*-1, range_Z/2);
        Vector3 RandomPosition = new Vector3(range_X, yPosition, range_Z);

        Vector3 respawnPosition = originPosition + RandomPosition;

        return respawnPosition;
    }
    void Start(){
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
         while (_currentCount < totalCount)
        {
            yield return new WaitForSeconds(0.1f);

            // 생성 위치 부분에 위에서 만든 함수 Return_RandomPosition() 함수 대입
            GameObject instantCapsul = Instantiate(theEnemy, Return_RandomPosition(), Quaternion.identity);
            _currentCount++;
        }

       /* while(enemyCount<10){
            xPos = Random.Range(-50,50);
            zPos = Random.Range(-31,31);
            Instantiate(theEnemy, new Vector3(xPos,2,zPos),Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1; */
                    }
    }



