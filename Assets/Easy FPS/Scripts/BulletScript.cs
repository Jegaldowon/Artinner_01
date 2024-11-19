using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{

    [Tooltip("총알이 목표를 찾기 위해 최대 거리")]
    public float maxDistance = 1000000;
    RaycastHit hit;
    [Tooltip("벽에 닿았을 때 데칼 프리팹. 객체는 데칼을 생성하기 위해 'LevelPart' 태그가 필요합니다.")]
    public GameObject decalHitWall;
    [Tooltip("데칼이 벽에서 약간 앞으로 위치해야 하므로 렌더링 문제를 피할 수 있습니다. 최상의 느낌을 위해 0.01에서 0.1 사이의 값을 설정하세요.")]
    public float floatInfrontOfWall;
    [Tooltip("총알이 적을 맞추었을 때 생성할 피 particle 프리팹")]
    public GameObject bloodEffect;
    [Tooltip("무기 레이어와 플레이어 레이어는 총알 레이캐스트가 무시하도록 설정하세요.")]
    public LayerMask ignoreLayer;

    /*
    * 이 스크립트가 부착된 총알이 생성되면,
    * 총알은 해당 태그를 찾기 위해 레이캐스트를 생성합니다.
    * 레이캐스트가 무언가를 찾으면 해당 태그의 데칼을 생성합니다.
    */
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            if (decalHitWall)
            {

                // 주석 처리함 
                //if (hit.transform.CompareTag("LevelPart"))
                //{
                //    Instantiate(decalHitWall, hit.point + hit.normal * floatInfrontOfWall, Quaternion.LookRotation(hit.normal));
                //    Destroy(gameObject);
                //}
                if (hit.transform.CompareTag("Dummie"))
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));

                    // 적에게 데미지를 주는 로직
                    Enemy enemy = hit.transform.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(50); // 예: 50의 데미지 주기
                    }

                    Destroy(gameObject);
                }
            }
            Destroy(gameObject);
        }
        Destroy(gameObject, 0.1f);
    }


}
