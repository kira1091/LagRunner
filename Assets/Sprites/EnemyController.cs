using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public float jumpForce = 5f;
    public float jumpInterval = 2f; // Jumps every 2 seconds
    public Transform player;
    private Rigidbody2D rb;
    private float nextJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        nextJumpTime = Time.time + jumpInterval;
    }

    void Update()
    {
        // Move toward player if available
        if (player != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.position, step);

            // Flip enemy to face player
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // Jump logic
        if (Time.time >= nextJumpTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            nextJumpTime = Time.time + jumpInterval;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Enemy disappears on contact
        }
        // Optionally handle ground contact here
    }
}