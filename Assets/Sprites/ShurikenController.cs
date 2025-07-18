using UnityEngine;

public class ShurikenController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Auto destroy after 2 sec
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime); // Move right
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Shuriken hit: " + collision.name);

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Enemy detected, destroying...");
            Destroy(collision.gameObject); // Kill enemy
            Destroy(gameObject); // Destroy shuriken
        }
    }
}
