using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float maxSpeed = 3.0f;
    public float jumpForce = 100f;
    public float hangMoveSpeed = 2.25f;
    public float ceilingCheckDistance = 1.1f;
    public KeyCode hangKey = KeyCode.LeftShift;
    public LayerMask hangableCeilingMask;
    public float hangOffset = 0.08f;
    public float starStruckMoveMultiplier = 0.06f;
    public float starStruckMaxSpeedMultiplier = 0.12f;
    public bool isStarStruck = false;
    public bool isHanging = false;
    private Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    private Collider2D playerCol;
    private float defaultGravityScale;
    public bool onGround;
    public Sprite[] sprites;
    public Sprite heartEyesSprite;
    public GameObject gum;
    private GameManager gm;
    private Coroutine hitRoutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerCol = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        defaultGravityScale = rb.gravityScale;
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        if (hangableCeilingMask.value == 0)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer >= 0)
            {
                hangableCeilingMask = 1 << groundLayer;
            }
        }
    }

    // Update is called once per frame

    void Update()
    {
        if (gm == null || gm.gameOver) {
            if (isHanging) StopHanging();
            return;
        }

        if (isStarStruck && isHanging)
        {
            StopHanging();
        }

        if (Input.GetKeyDown(hangKey))
        {
            if (isStarStruck) return;
            if (isHanging) StopHanging();
            else TryHangFromCeiling();
        }

        if (isHanging)
        {
            HandleHangingMovement();

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StopHanging();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                NerdEnemy.TrySilenceNearby(transform.position);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ShootToMouse();
            }
            return;
        }

        float MoveHor = Input.GetAxisRaw("Horizontal");
        float moveMultiplier = isStarStruck ? starStruckMoveMultiplier : 1f;
        float speedCapMultiplier = isStarStruck ? starStruckMaxSpeedMultiplier : 1f;

        Vector2 movement = new Vector2(MoveHor * moveSpeed * moveMultiplier, 0);
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
        float currentMaxSpeed = maxSpeed * speedCapMultiplier;
        if (rb.linearVelocity.x > currentMaxSpeed)
        {
            rb.linearVelocity = new Vector2(currentMaxSpeed, rb.linearVelocity.y);
        }
        if (rb.linearVelocity.x < -currentMaxSpeed)
        {
            rb.linearVelocity = new Vector2(-currentMaxSpeed, rb.linearVelocity.y);
        }

        if (!isStarStruck && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canJump())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NerdEnemy.TrySilenceNearby(transform.position);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ShootToMouse();
        }
    }

    private void TryHangFromCeiling()
    {
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, ceilingCheckDistance, hangableCeilingMask);
        if (!hit.collider)
        {
            return;
        }

        isHanging = true;
        onGround = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        if (playerCol != null)
        {
            float topExtent = playerCol.bounds.extents.y;
            transform.position = new Vector3(transform.position.x, hit.point.y - topExtent - hangOffset, transform.position.z);
        }
    }

    private void StopHanging()
    {
        isHanging = false;
        rb.gravityScale = defaultGravityScale;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -0.5f);
    }

    private void HandleHangingMovement()
    {
        float moveHor = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveHor * hangMoveSpeed, 0f);

        if (moveHor > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveHor < 0)
        {
            spriteRenderer.flipX = false;
        }
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, ceilingCheckDistance, hangableCeilingMask);
        if (!hit.collider)
        {
            StopHanging();
        }
    }

    private void ShootFacingDirection()
    {
        if (gm.gumCount <= 0) return;

        GameObject gumObj = Instantiate(gum, transform.position, Quaternion.identity);
        gum gumScript = gumObj.GetComponent<gum>();
        if (gumScript != null)
        {
            Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;
            gumScript.SetDirection(direction);
        }
        gm.useGum(false);
    }

    private void ShootToMouse()
    {
        if (gm.gumCount <= 1) return;
        Instantiate(gum, transform.position, Quaternion.identity);
        gm.useGum(false);
    }

    bool canJump()
    {
        return onGround;
    }

    public void takeHit()
    {
        // Hurt visuals are intentionally disabled for now.
        return;
    }

    public void SetStarStruckState(bool active)
    {
        isStarStruck = active;

        if (active)
        {
            if (hitRoutine != null)
            {
                StopCoroutine(hitRoutine);
                hitRoutine = null;
            }
            spriteRenderer.color = Color.white;
            if (isHanging) StopHanging();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.2f, rb.linearVelocity.y);
            }

            ApplyStarStruckSpriteOrFallback();
        }
        else if (sprites != null && sprites.Length > 0)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = sprites[0];
        }
    }

    private void ApplyStarStruckSpriteOrFallback()
    {
        if (heartEyesSprite != null)
        {
            spriteRenderer.sprite = heartEyesSprite;
        }
        else if (sprites != null && sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }
    }

    IEnumerator ow()
    {
        spriteRenderer.sprite = sprites[1];
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(1f,.6f,0.6f);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = new Color(1f, 1f, 1f);
        yield return new WaitForSeconds(0.25f);
        if (isStarStruck)
        {
            ApplyStarStruckSpriteOrFallback();
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
        }

        hitRoutine = null;
    }
}
