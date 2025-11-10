using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    public float spawnInterval = 1.0f;
    public float xRange = 4f;
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }
    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, objectsToSpawn.Length);
        float currentX = transform.position.x;
        float randomX = Mathf.Clamp(currentX + Random.Range(-xRange, xRange), -xRange, xRange);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);
        Instantiate(objectsToSpawn[randomIndex], spawnPosition, objectsToSpawn[randomIndex].transform.rotation);
    }
}
