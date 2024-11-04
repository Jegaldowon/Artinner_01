using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int health;
    public int MaxHealth = 10;


    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    // 받는 데미지 양
    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Destroy(gameObject);

        }
    }
}
