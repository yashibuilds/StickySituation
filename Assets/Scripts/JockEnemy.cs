<<<<<<< HEAD
using System.Collections;
using System.Runtime.CompilerServices;
=======
>>>>>>> 5222bed8c13ffe2c7caf3e8ae3ccd33ec1fcaebc
using UnityEngine;

public class JockEnemy : MonoBehaviour
{
<<<<<<< HEAD
    public GameObject ball;
    public Sprite idle;
    public Sprite attack;
    private GameObject player;
    private SpriteRenderer sr;
    public float cooldown = 3;
    public float thresh = 5;
    private float elapsed = 0;
    private float dir = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (player.transform.position.x > transform.position.x)
        {
            sr.flipX = true;
            dir = 1;
        } else
        {
            sr.flipX = false;
            dir = -1;
        }
        if (elapsed > cooldown)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < thresh)
            {
                elapsed = 0;
                StartCoroutine(throwBall());
            }
        }
    }

    IEnumerator throwBall()
    {
       
        sr.sprite = attack;
        yield return new WaitForSeconds(0.2f);
        Instantiate(ball,transform.position+ new Vector3(dir*1,0), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        sr.sprite = idle;
=======
    public GameObject basketballPrefab;
    public Transform throwPoint;
    public float aggroRange = 8f;
    public float throwCooldown = 2.25f;
    public float projectileSpeed = 8f;
    public float maxAimYOffset = 1.8f;
    public float throwPointYOffset = 0.8f;

    private GameObject player;
    private GameManager gm;
    private float throwTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        throwTimer = throwCooldown * 0.6f;
    }

    void Update()
    {
        if (player == null || gm == null || gm.gameOver) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > aggroRange) return;

        throwTimer -= Time.deltaTime;
        if (throwTimer > 0f) return;

        ThrowBasketball();
        throwTimer = throwCooldown;
    }

    private void ThrowBasketball()
    {
        Vector3 spawnPos = throwPoint != null
            ? throwPoint.position
            : transform.position + new Vector3(0f, throwPointYOffset, 0f);

        GameObject projectileObj = basketballPrefab != null
            ? Instantiate(basketballPrefab, spawnPos, Quaternion.identity)
            : CreateFallbackBasketball(spawnPos);

        Vector2 target = player.transform.position;
        float yOffset = Mathf.Clamp(target.y - spawnPos.y, -maxAimYOffset, maxAimYOffset);
        Vector2 dir = new Vector2(target.x - spawnPos.x, yOffset).normalized;

        BasketballProjectile projectile = projectileObj.GetComponent<BasketballProjectile>();
        if (projectile == null)
        {
            projectile = projectileObj.AddComponent<BasketballProjectile>();
        }
        projectile.Initialize(dir, projectileSpeed);
    }

    private GameObject CreateFallbackBasketball(Vector3 spawnPos)
    {
        GameObject ball = new GameObject("Basketball");
        ball.transform.position = spawnPos;

        CircleCollider2D col = ball.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.2f;

        Rigidbody2D rb = ball.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        SpriteRenderer renderer = ball.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 0.45f, 0f, 1f);
        renderer.sortingOrder = 3;

        return ball;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.55f, 0.1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}

public class BasketballProjectile : MonoBehaviour
{
    public float lifeTime = 3.5f;
    public float hitCooldown = 0.15f;

    private Vector2 direction;
    private float speed;
    private float hitTimer;
    private bool initialized;

    public void Initialize(Vector2 moveDirection, float moveSpeed)
    {
        direction = moveDirection.sqrMagnitude < 0.0001f ? Vector2.right : moveDirection.normalized;
        speed = Mathf.Max(0f, moveSpeed);
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        hitTimer -= Time.deltaTime;

        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!initialized || hitTimer > 0f) return;

        if (collision.CompareTag("Player"))
        {
            GameManager gm = GameObject.FindWithTag("GameController")?.GetComponent<GameManager>();
            if (gm != null && !gm.gameOver)
            {
                gm.useGum(true);
            }
            Destroy(gameObject);
            return;
        }

        if (!collision.isTrigger && !collision.CompareTag("Jock"))
        {
            Destroy(gameObject);
            return;
        }

        hitTimer = hitCooldown;
>>>>>>> 5222bed8c13ffe2c7caf3e8ae3ccd33ec1fcaebc
    }
}
