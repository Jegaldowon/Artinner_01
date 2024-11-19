using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("적생성")]
    [SerializeField] GameObject SentryRobot;  // 적 프리팹 SentryRobot
    [SerializeField] GameObject Robot_Guardian; // 적 프리팹 Robot_Guardian
    [SerializeField] Transform[] createEnemy;  // 적 생성 위치 배열
    [SerializeField] float create_time;  // 생성 시간 간격

    // Start is called before the first frame update
    void Start()
    {
        // InvokeRepeating을 사용하여 create 함수가 일정 시간마다 호출되도록 설정
        InvokeRepeating("create", 0, create_time);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void create()
    {
        Debug.Log("적 생성중");

        if (SentryRobot == null && Robot_Guardian == null)
        {
            Debug.LogError("Both SentryRobot 이랑  Robot_Guardian 할당 되지 않음");
            return;
        }

        GameObject enemyToSpawn = Random.Range(0, 2) == 0 ? SentryRobot : Robot_Guardian;
        Debug.Log($"스폰 될 적 선택: {enemyToSpawn.name}");

        if (enemyToSpawn == null)
        {
            Debug.LogError("선택된 Enemy 값이 null 입니다.");
            return;
        }

        if (createEnemy == null || createEnemy.Length == 0)
        {
            Debug.LogError("생성 지점이 없음");
            return;
        }

        int spawnIndex = Random.Range(0, createEnemy.Length);
        Debug.Log($"스폰 인덱스 선택: {spawnIndex}");

        if (createEnemy[spawnIndex] == null)
        {
            Debug.LogError($"인덱스 {spawnIndex} 의 생성지점이 null 입니다.");
            return;
        }

        Instantiate(enemyToSpawn, createEnemy[spawnIndex].position, createEnemy[spawnIndex].rotation);
        Debug.Log("적 이 생성 되었습니다. ");
    }

}
