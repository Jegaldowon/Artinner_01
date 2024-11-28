using UnityEngine;
using UnityEngine.AI;  // NavMeshAgent ����� ���� �߰�
using UnityEngine.UI;  // UI ����� ���� �߰�

public class Enemy : MonoBehaviour
{
    public int health = 10;       // ���� �ִ� ü��
    public int attackDamage = 2;  // �÷��̾�� �� ������
    public Player playerHealth;   // Player ��ũ��Ʈ ���� -> TakeDamage ȣ��

    public float speed = 1f;
    public int followDistance = 100000;
    private Transform player;
    private NavMeshAgent navAgent;  // NavMeshAgent ���� �߰�

    public delegate void EnemyDeathDelegate(Enemy enemy);   // �� ��� �� ȣ���� ��������Ʈ
    public event EnemyDeathDelegate OnEnemyDeath;  // �� ��� �̺�Ʈ

    [Header("ü�¹� ����")]
    [SerializeField] private Transform hp;    // ü�¹� Canvas�� Transform
    [SerializeField] private Slider healthBar; // ü�¹� Slider ����

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // NavMeshAgent �ʱ�ȭ
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component�� �����ϴ�.");
        }
        else
        {
            navAgent.speed = speed;  // NavMeshAgent �ӵ� ����
        }

        // ü�¹� �ʱ� ����
        if (healthBar != null)
        {
            healthBar.maxValue = health; // ü�¹� �ִ밪�� ü�°� �����ϰ� ����
            healthBar.value = health;    // ü�¹��� ���簪�� �ʱ� ü������ ����
        }
    }

    void Update()
    {
        // �� ü�¹ٰ� ī�޶� ���ϵ��� ����
        if (hp != null)
        {
            hp.LookAt(Camera.main.transform);  // ī�޶� ������ ���ϵ��� ����
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // ���� �Ÿ� ���Ϸ� �÷��̾ ���� ����
        if (distance < followDistance)
        {
            FollowPlayer();
        }
    }



    // �÷��̾�� �浹 �� ������
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);  // �÷��̾�� ������
            }
        }
    }

    // ���� �������� �޾��� ��
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        // ü�¹� �� ������Ʈ
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        // ü���� 0 ������ ��� �� ��� ó��
        if (health <= 0)
        {
            Die();
        }
    }

    // �� ��� ó��
    private void Die()
    {
        // �� ��� �̺�Ʈ ȣ��
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject); // �� ������Ʈ ����
    }
    

    // �÷��̾ ����
    private void FollowPlayer()
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(player.position);  // NavMeshAgent�� ����Ͽ� �÷��̾� ��ġ�� �̵�
        }
    }
}
