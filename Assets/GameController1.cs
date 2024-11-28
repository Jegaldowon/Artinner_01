using UnityEngine;

public class GameController1 : MonoBehaviour
{
    public Transform[] spawnPoints;  // �� ���� ����Ʈ
    public GameObject enemyPrefab;   // �� ������
    public int enemiesPerWave = 15;  // �� ���̺�� ������ �� ��

    private int enemiesSpawned = 0;  // ������ ���� ��
    private int enemiesDefeated = 0; // óġ�� ���� ��

    private WaveManager waveManager; // WaveManager ����

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>(); // WaveManager ã��
        SpawnEnemies(); // �� ����7
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (enemiesSpawned >= enemiesPerWave)
                break;

            // �� ����
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemiesSpawned++;

            // ���� ��� �̺�Ʈ ����
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

        Debug.Log($"�� óġ: {enemiesDefeated}/{enemiesPerWave}");

        // ��� ���� óġ�Ǿ��� ��
        if (enemiesDefeated >= enemiesPerWave)
        {
            Debug.Log("Wave �Ϸ�!");
            waveManager.OnWaveCompleted(); // WaveManager�� �˸�
        }
    }
}
