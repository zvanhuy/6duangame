using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePairPrefab;
    [SerializeField] private float spawnInterval = 2.5f;
    [SerializeField] private float minSpawnY = -1.2f;
    [SerializeField] private float maxSpawnY = 1.2f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnPipePair();
        }
    }

    private void SpawnPipePair()
    {
        float randomY = Random.Range(minSpawnY, maxSpawnY);
        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, transform.position.z);

        Instantiate(pipePairPrefab, spawnPosition, Quaternion.identity);
    }
}