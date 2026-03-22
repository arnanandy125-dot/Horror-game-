using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GameObject winUI;   // Drag YouWinText here in Inspector

    Rigidbody2D rb;
    Vector2 move;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
        move.Normalize();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Touched something: " + other.name);

    if (other.CompareTag("Goal"))
    {
        Debug.Log("GOAL HIT");
        Time.timeScale = 0f;
        winUI.SetActive(true);
    }
}
}