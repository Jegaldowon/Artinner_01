using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public int health;
    public int MaxHealth = 10;

    public Slider healthBar; // ü�¹� ����



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


    // �޴� ������ ��
    public void TakeDamage(int amount)
    {
        health -= amount;

        // ü�¹�
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
