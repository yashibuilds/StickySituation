using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float maxSpeed = 3.0f;
    public float jumpForce = 100f;
    public bool isStarStruck = false;
    private Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    public bool onGround;
    public Sprite[] sprites;
    public GameObject gum;
    private GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame

    void Update()
    {
        if (isStarStruck) {
            return;
        } 
        float MoveHor = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(MoveHor * moveSpeed, 0);
        movement = movement * Time.deltaTime;
        rb.AddForce(movement);
        if (MoveHor > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (MoveHor < 0) 
        {
            spriteRenderer.flipX = false;
        }
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
        if (Input.GetMouseButtonUp(0))
        {
            shoot();
        }

    }
    private void shoot()
    {
        Instantiate(gum, transform.position, Quaternion.identity);
        gm.useGum(false);
    }

    bool canJump()
    {
        return onGround;
    }

    public void takeHit()
    {
        StartCoroutine(ow());

    }
    IEnumerator ow()
    {
        spriteRenderer.sprite = sprites[1];
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(1f,.6f,0.6f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(1f, 1f, 1f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.sprite = sprites[0];

    }
}
