using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public GameObject clockPrefab;
    public Transform[] spawnPoints;

    public float minDelay = 30f;
    public float maxDelay = 100f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(SpawnFruits());
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject spawnedFruit = Instantiate(clockPrefab, spawnPoint.position, spawnPoint.rotation);

        }
    }
}
