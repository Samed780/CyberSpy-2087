using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float deathTimer = 3f;

    public void Respawn()
    {
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(deathTimer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
