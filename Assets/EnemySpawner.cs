using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject SentryRobot;  // �� ������ SentryRobot
    [SerializeField] GameObject Robot_Guardian; // �� ������ Robot_Guardian
    [SerializeField] Transform[] createEnemy;  // �� ���� ��ġ �迭
    [SerializeField] float create_time;  // ���� �ð� ����

    // Start is called before the first frame update
    void Start()
    {
        // InvokeRepeating�� ����Ͽ� create �Լ��� ���� �ð����� ȣ��ǵ��� ����
        InvokeRepeating("create", 0, create_time);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void create()
    {
        Debug.Log("�� ������");

        if (SentryRobot == null && Robot_Guardian == null)
        {
            Debug.LogError("Both SentryRobot �̶�  Robot_Guardian �Ҵ� ���� ����");
            return;
        }

        GameObject enemyToSpawn = Random.Range(0, 2) == 0 ? SentryRobot : Robot_Guardian;
        Debug.Log($"���� �� �� ����: {enemyToSpawn.name}");

        if (enemyToSpawn == null)
        {
            Debug.LogError("���õ� Enemy ���� null �Դϴ�.");
            return;
        }

        if (createEnemy == null || createEnemy.Length == 0)
        {
            Debug.LogError("���� ������ ����");
            return;
        }

        int spawnIndex = Random.Range(0, createEnemy.Length);
        Debug.Log($"���� �ε��� ����: {spawnIndex}");

        if (createEnemy[spawnIndex] == null)
        {
            Debug.LogError($"�ε��� {spawnIndex} �� ���������� null �Դϴ�.");
            return;
        }

        Instantiate(enemyToSpawn, createEnemy[spawnIndex].position, createEnemy[spawnIndex].rotation);
        Debug.Log("�� �� ���� �Ǿ����ϴ�. ");
    }

}
