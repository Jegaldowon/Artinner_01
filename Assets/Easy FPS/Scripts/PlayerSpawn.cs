using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("���� ��ġ ����")]
    [SerializeField]
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(0,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0)

    };





    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
        
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Length)];
        

        transform.position = spawnPosition;




    }


}
