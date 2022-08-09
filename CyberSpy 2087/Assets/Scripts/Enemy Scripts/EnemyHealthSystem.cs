using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    EnemyUICanvasController enemyUICanvasController;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        enemyUICanvasController = GetComponentInChildren<EnemyUICanvasController>();
        enemyUICanvasController.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) 
    {
        currentHealth -= damage;

        enemyUICanvasController.SetHealth(currentHealth);

        if(currentHealth <= 0)
            Destroy(gameObject);
    }
}
