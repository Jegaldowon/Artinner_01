using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public int health;
    public int MaxHealth = 10;

    public Slider healthBar; // 체력바 생성



    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = MaxHealth;
            healthBar.value = health;
            
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 받는 데미지 양
    public void TakeDamage(int amount)
    {
        health -= amount;

        // 체력바
        if (healthBar != null)
        {
            healthBar.value = health;

            
        }



        if (health <= 0)
        {
            Destroy(gameObject);

        }
    }

   


}
