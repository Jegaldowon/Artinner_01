using UnityEngine;

public class GameController2 : MonoBehaviour
{
    public Transform[] spawnPoints;  // Wave 2�� ���� ����Ʈ��
    public GameObject enemyPrefab;   // Enemy ������
    public int enemiesPerWave = 10;  // Wave 2���� ������ �� ��

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private WaveManager waveManager; // WaveManager ����

    void Start()
    {
        //// ���� �����ϴ� ���� ������ ��Ȱ��ȭ�ϰų� ����
        //GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Dummie");
        //foreach (GameObject enemy in existingEnemies)
        //{
        //    Destroy(enemy); // ���� �� ����
        //                    // �Ǵ� ���� ��Ȱ��ȭ �� ���� �ֽ��ϴ�: enemy.SetActive(false);
        //}

        // �� �������� �Ҵ�Ǿ� �ִ��� Ȯ��
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy �������� �������� �ʾҽ��ϴ�!");
            return;
        }
        enemyPrefab.SetActive(true);
        waveManager = FindObjectOfType<WaveManager>(); // WaveManager ã��
        SpawnEnemies(); // ���̺� 2 �� ��ȯ ����
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("���� ����Ʈ�� �������� �ʾҽ��ϴ�!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (enemiesSpawned >= enemiesPerWave)
                break;

            if (spawnPoint == null)
            {
                Debug.LogWarning("���� ����Ʈ�� null�Դϴ�! �ǳʶٰڽ��ϴ�.");
                continue;
            }

            // �� ����
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            if (enemy == null)
            {
                Debug.LogError("���� ������ �� �����ϴ�!");
                continue;
            }

            enemiesSpawned++;

            // ���� ��� �̺�Ʈ ����
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnEnemyDeath += HandleEnemyDeath;
            }
            else
            {
                Debug.LogError("Enemy ��ũ��Ʈ�� �� �����տ� ����Ǿ� ���� �ʽ��ϴ�!");
            }
        }
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        enemiesDefeated++;

        Debug.Log($"Wave 2 �� óġ: {enemiesDefeated}/{enemiesPerWave}");

        // ��� ���� óġ�Ǿ��� ��
        if (enemiesDefeated >= enemiesPerWave)
        {
            if (waveManager != null)
            {
                waveManager.OnWaveCompleted(); // ���� ���̺� �˸�
            }
            else
            {
                Debug.LogError("WaveManager�� null�Դϴ�. ���� ���̺�� ������ �� �����ϴ�.");
            }
        }
    }
}
