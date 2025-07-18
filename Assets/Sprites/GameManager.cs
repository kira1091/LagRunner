using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton for global access
    public int playerHealth = 100; // Hitesh's health
    public int score = 0; // Player score
    public Slider healthBar; // Reference to the health bar UI

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (healthBar != null) healthBar.value = playerHealth; // Set initial health bar
    }

    // Update health (called by HiteshController)
    public void UpdateHealth(int change)
    {
        playerHealth += change;
        if (playerHealth > 100) playerHealth = 100; // Cap at 100
        if (playerHealth < 0) playerHealth = 0; // Prevent negative
        if (healthBar != null) healthBar.value = playerHealth; // Update bar
        Debug.Log("Health: " + playerHealth); // Log health
    }

    // Update score (called by HiteshController for collectibles)
    public void UpdateScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score); // Log score
    }
}