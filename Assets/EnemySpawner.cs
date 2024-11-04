using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Àû»ý¼º")]
    [SerializeField] GameObject enemy;
    [SerializeField] Transform[] createEnemy;
    [SerializeField] float create_time;




    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("create", 0, create_time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void create()
    {
        int i = Random.Range(0, 4);



        Instantiate(enemy, createEnemy[i].position, createEnemy[i].rotation);   

    }
}


