using UnityEngine;

public class GameController3 : MonoBehaviour
{
    public Transform[] spawnPoints;  // 적 스폰 포인트
    public GameObject enemyPrefab;   // 적 프리팹
    public int enemiesPerWave = 15;  // 한 웨이브당 생성될 적 수

    private int enemiesSpawned = 0;  // 생성된 적의 수
    private int enemiesDefeated = 0; // 처치된 적의 수

    private WaveManager waveManager; // WaveManager 참조

    void Start()
    {
        // 씬에 존재하는 기존 적들을 비활성화하거나 삭제
        //GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Dummie");
        //foreach (GameObject enemy in existingEnemies)
        //{
        //    Destroy(enemy); // 기존 적 삭제
        //                    // 또는 적을 비활성화 할 수도 있습니다: enemy.SetActive(false);
        //}

        // 적 프리팹이 할당되어 있는지 확인
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy 프리팹이 설정되지 않았습니다!");
            return;
        }
        enemyPrefab.SetActive(true);
        waveManager = FindObjectOfType<WaveManager>(); // WaveManager 찾기
        SpawnEnemies(); // 웨이브 3 적 소환 시작
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab이 설정되지 않았습니다!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (enemiesSpawned >= enemiesPerWave)
                break;

            // 적 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemiesSpawned++;

            // 적의 사망 이벤트 구독
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnEnemyDeath += HandleEnemyDeath;
            }
        }
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        enemiesDefeated++;

        Debug.Log($"적 처치: {enemiesDefeated}/{enemiesPerWave}");

        // 모든 적이 처치되었을 때
        if (enemiesDefeated >= enemiesPerWave)
        {
            Debug.Log("Wave 완료!");
            waveManager.OnWaveCompleted(); // WaveManager에 알림
        }
    }
}
