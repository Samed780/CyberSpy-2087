using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    UICanvasController canvasController;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        canvasController = FindObjectOfType<UICanvasController>();
        canvasController.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        canvasController.SetHealth(currentHealth);

        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);
            FindObjectOfType<GameManager>().Respawn();
        }
    }
}
