using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public int numberOfAsteroids = 100;
    public Vector3 minSpawnRange = new Vector3(-5f, -5f, -5f);
    public Vector3 maxSpawnRange = new Vector3(5f, 5f, 5f);

    void Start()
    {
        GenerateAsteroids();
    }

    void GenerateAsteroids()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 cubeCenter = transform.position;
        Vector3 cubeExtents = transform.localScale * 0.5f;

        float randomX = Random.Range(minSpawnRange.x, maxSpawnRange.x);
        float randomY = Random.Range(minSpawnRange.y, maxSpawnRange.y);
        float randomZ = Random.Range(minSpawnRange.z, maxSpawnRange.z);

        Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);

        return cubeCenter + Vector3.Scale(randomOffset, cubeExtents);
    }
}