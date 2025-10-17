using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // 적 프리팹 배열
    public Transform[] spawnPoints;   // 스폰 위치 배열
    public float timeBetweenWaves = 5f; // 웨이브 간 시간 간격
    private float countdown = 2f; // 다음 웨이브까지 남은 시간
    private int waveIndex = 0; // 현재 웨이브 인덱스
    public Transform Target;
    private void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }
    private void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        enemy.GetComponent<Enemy>().SetTaget(Target);
    }
    private IEnumerator SpawnWave()
    {
        waveIndex++;
        int enemiesToSpawn = waveIndex * 2; // 웨이브마다 적 수 증가
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f); // 적 간 스폰 딜레이
        }
    }
}
