using UnityEngine;
using UnityEngine.AI;  // NavMeshAgent 사용을 위해 추가
using UnityEngine.UI;  // UI 사용을 위해 추가

public class Enemy : MonoBehaviour
{
    public int health = 10;       // 적의 최대 체력
    public int attackDamage = 2;  // 플레이어에게 줄 데미지
    public Player playerHealth;   // Player 스크립트 참조 -> TakeDamage 호출

    public float speed = 1f;
    public int followDistance = 100000;
    private Transform player;
    private NavMeshAgent navAgent;  // NavMeshAgent 변수 추가

    public delegate void EnemyDeathDelegate(Enemy enemy);   // 적 사망 시 호출할 델리게이트
    public event EnemyDeathDelegate OnEnemyDeath;  // 적 사망 이벤트

    [Header("체력바 설정")]
    [SerializeField] private Transform hp;    // 체력바 Canvas의 Transform
    [SerializeField] private Slider healthBar; // 체력바 Slider 참조

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // NavMeshAgent 초기화
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component가 없습니다.");
        }
        else
        {
            navAgent.speed = speed;  // NavMeshAgent 속도 설정
        }

        // 체력바 초기 설정
        if (healthBar != null)
        {
            healthBar.maxValue = health; // 체력바 최대값을 체력과 동일하게 설정
            healthBar.value = health;    // 체력바의 현재값을 초기 체력으로 설정
        }
    }

    void Update()
    {
        // 적 체력바가 카메라를 향하도록 설정
        if (hp != null)
        {
            hp.LookAt(Camera.main.transform);  // 카메라 방향을 향하도록 설정
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // 일정 거리 이하로 플레이어가 오면 추적
        if (distance < followDistance)
        {
            FollowPlayer();
        }
    }



    // 플레이어와 충돌 시 데미지
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);  // 플레이어에게 데미지
            }
        }
    }

    // 적이 데미지를 받았을 때
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        // 체력바 값 업데이트
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        // 체력이 0 이하일 경우 적 사망 처리
        if (health <= 0)
        {
            Die();
        }
    }

    // 적 사망 처리
    private void Die()
    {
        // 적 사망 이벤트 호출
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject); // 적 오브젝트 삭제
    }
    

    // 플레이어를 추적
    private void FollowPlayer()
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(player.position);  // NavMeshAgent를 사용하여 플레이어 위치로 이동
        }
    }
}
