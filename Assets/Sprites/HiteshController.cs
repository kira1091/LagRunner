using UnityEngine;
using UnityEngine.UI; // Required for Slider

public class HiteshController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 12f;
    public float slideTime = 0.5f;
    public int health = 100;

    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip collectSound;

    public Slider healthBar; // Drag the Slider here in Inspector
    [SerializeField] private GameObject shurikenProjectilePrefab; // projectile prefab
    private bool hasShinobiggs = false; // flag for Shinobiggs

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isSliding = false;
    private float slideTimer;

    private BoxCollider2D boxCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (healthBar != null)
        {
            healthBar.maxValue = 100;
            healthBar.value = health; // Set initial health
        }
        else
        {
            Debug.LogError("Health Bar not assigned! Drag the Slider into the Inspector.");
        }
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Flip character based on direction
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.Play("Jump");
            if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            animator.SetBool("isJumping", true);
        }
        else if (rb.linearVelocity.y < 0)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        // Slide
        if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = slideTime;
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
            animator.Play("Slide");
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                boxCollider.size = originalColliderSize;
                boxCollider.offset = originalColliderOffset;
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.Play("Hurt");
        }

        // Shoot with Shinobiggs
        if (Input.GetKeyDown(KeyCode.F) && hasShinobiggs)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (shurikenProjectilePrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0.5f * Mathf.Sign(transform.localScale.x), 0, 0); // Offset slightly forward
            GameObject projectile = Instantiate(shurikenProjectilePrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = new Vector2(12f * Mathf.Sign(transform.localScale.x), 0); // Direct velocity
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            transform.up = Vector3.up;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            health -= 20;
            if (health < 0) health = 0;
            animator.Play("Hurt");
            if (hitSound != null) audioSource.PlayOneShot(hitSound);
            Destroy(collision.gameObject);
            Debug.Log("Health: " + health);
            if (healthBar != null) healthBar.value = health; // Update health bar
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            if (collision.gameObject.name == "Shinobiggs")
            {
                hasShinobiggs = true;
                Destroy(collision.gameObject);
                Debug.Log("Shinobiggs Acquired!");
            }
            else
            {
                health += 25;
                if (health > 100) health = 100;
                if (collectSound != null) audioSource.PlayOneShot(collectSound);
                Debug.Log("Health: " + health);
                Destroy(collision.gameObject);
                if (healthBar != null) healthBar.value = health; // Update health bar
            }
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Level Complete! You Win, Hitesh!");
        }
    }
}