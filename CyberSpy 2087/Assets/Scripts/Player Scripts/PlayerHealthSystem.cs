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

        AudioManager.instance.PlaySFX(4);

        canvasController.SetHealth(currentHealth);

        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);
            FindObjectOfType<GameManager>().Respawn();
            AudioManager.instance.StopBGMusic();
            AudioManager.instance.PlaySFX(3);
        }
    }

    public void Heal(int heal)
    {
        currentHealth += heal;

        if(currentHealth > maxHealth)
            currentHealth = maxHealth;

        canvasController.SetHealth(currentHealth);
    }
}
