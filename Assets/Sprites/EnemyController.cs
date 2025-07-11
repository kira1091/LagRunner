using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public float jumpForce = 8f; // New: Jump strength
    public Transform player;
    private Rigidbody2D rb;
    private float nextJumpTime; // To control jump timing
    public float jumpInterval = 2f; // Jump every 2 seconds

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        nextJumpTime = Time.time + jumpInterval; // Set first jump time
    }

    void Update()
    {
        if (player != null)
        {
            // Move toward player
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.position, step);

            // Flip enemy to face player
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            // Jump logic
            if (Time.time >= nextJumpTime)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset Y velocity
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                nextJumpTime = Time.time + jumpInterval; // Set next jump time
                Debug.Log(name + " is jumping!");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Enemy disappears on contact
        }
    }
}