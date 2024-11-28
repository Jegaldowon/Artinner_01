using UnityEngine;

public class GameController2 : MonoBehaviour
{
    public Transform[] spawnPoints;  // Wave 2의 스폰 포인트들
    public GameObject enemyPrefab;   // Enemy 프리팹
    public int enemiesPerWave = 10;  // Wave 2에서 생성될 적 수

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private WaveManager waveManager; // WaveManager 참조

    void Start()
    {
        //// 씬에 존재하는 기존 적들을 비활성화하거나 삭제
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
        SpawnEnemies(); // 웨이브 2 적 소환 시작
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab이 설정되지 않았습니다!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 설정되지 않았습니다!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (enemiesSpawned >= enemiesPerWave)
                break;

            if (spawnPoint == null)
            {
                Debug.LogWarning("스폰 포인트가 null입니다! 건너뛰겠습니다.");
                continue;
            }

            // 적 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            if (enemy == null)
            {
                Debug.LogError("적을 생성할 수 없습니다!");
                continue;
            }

            enemiesSpawned++;

            // 적의 사망 이벤트 구독
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnEnemyDeath += HandleEnemyDeath;
            }
            else
            {
                Debug.LogError("Enemy 스크립트가 적 프리팹에 연결되어 있지 않습니다!");
            }
        }
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        enemiesDefeated++;

        Debug.Log($"Wave 2 적 처치: {enemiesDefeated}/{enemiesPerWave}");

        // 모든 적이 처치되었을 때
        if (enemiesDefeated >= enemiesPerWave)
        {
            if (waveManager != null)
            {
                waveManager.OnWaveCompleted(); // 다음 웨이브 알림
            }
            else
            {
                Debug.LogError("WaveManager가 null입니다. 다음 웨이브로 진행할 수 없습니다.");
            }
        }
    }
}
