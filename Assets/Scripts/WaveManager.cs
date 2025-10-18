using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Enemy Settings")]
    [Tooltip("Enum 순서와 일치해야 합니다!")]
    public GameObject[] enemyPrefabs; // 인덱스 = EnemyType 순서
    public Transform[] spawnPoints;
    public Transform Target;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    private float countdown = 3f;
    private int waveIndex = 0;

    [Header("UI")]
    public TMP_Text waveTimerText;
    public TMP_Text enemyCountText;

    [HideInInspector] public int EnemyCount = 0;

    // 각 EnemyType별 ObjectPool 관리용 딕셔너리
    public Dictionary<EnemyType, IObjectPool<GameObject>> enemyPools = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // enum 기반 풀 초기화
        foreach (EnemyType type in System.Enum.GetValues(typeof(EnemyType)))
        {
            enemyPools[type] = CreatePool(type);
        }
    }

    private void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        waveTimerText.text = $"Next Wave In: {Mathf.Ceil(countdown)}";
        enemyCountText.text = $"Enemies Left: {EnemyCount}";
    }

    // 특정 타입의 풀 생성
    private IObjectPool<GameObject> CreatePool(EnemyType type)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => CreateEnemy(type),
            actionOnGet: OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );
    }

    private GameObject CreateEnemy(EnemyType type)
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject prefab = enemyPrefabs[(int)type];
        GameObject enemy = Instantiate(prefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation,transform);
        EnemyCount++;
        enemy.GetComponent<Enemy>().SetTaget(Target);
        enemy.GetComponent<Enemy>().SetPool(enemyPools[type]); // 자신이 속한 풀 저장
        return enemy;
    }

    private void OnGetEnemy(GameObject enemy)
    {
        enemy.SetActive(true);
        enemy.transform.SetPositionAndRotation(
            spawnPoints[Random.Range(0, spawnPoints.Length)].position,
            Quaternion.identity
        );
        EnemyCount++;
    }

    private void OnReleaseEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        EnemyCount--;
    }

    private void OnDestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        EnemyCount--;
    }

    private IEnumerator SpawnWave()
    {
        waveIndex++;
        int enemiesToSpawn = waveIndex * 2;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // 스폰 시 랜덤 타입 선택
            EnemyType randomType = (EnemyType)Random.Range(0, enemyPrefabs.Length);
            var pool = enemyPools[randomType];
            pool.Get(); // Get()이 알아서 생성 or 재사용
            yield return new WaitForSeconds(0.5f);
        }
    }
}
