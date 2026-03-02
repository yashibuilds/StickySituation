using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float maxSpeed = 3.0f;
    public float jumpForce = 100f;
    private Rigidbody2D rb;
    public bool onGround;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame

    void Update()
    {
        float MoveHor = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(MoveHor * moveSpeed, 0);
        movement = movement * Time.deltaTime;
        rb.AddForce(movement);
        if (rb.linearVelocity.x > maxSpeed)
        {
            rb.linearVelocity = new Vector2(maxSpeed, rb.linearVelocity.y);
        }
        if (rb.linearVelocity.x < -maxSpeed)
        {
            rb.linearVelocity = new Vector2(-maxSpeed, rb.linearVelocity.y);
        }
        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce));
        }

    }

    bool canJump()
    {
        return onGround;
        //TASK 2
    }
}
