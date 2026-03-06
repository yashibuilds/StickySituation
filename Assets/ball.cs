using UnityEngine;
using UnityEngine.UIElements;

public class ball : MonoBehaviour
{
    public float force = 100.0f;
    private float decayTimer = 3f;
    private Rigidbody2D rb;
    public GameObject player;
    public GameManager gm;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        Vector3 dir = player.transform.position - transform.position;
        dir = dir.normalized;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(dir*force);
    }

    void Update()
    {
        decayTimer -= Time.deltaTime;
        if (decayTimer < 0)
        {
            Destroy(gameObject);
        }

    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            rb.linearVelocityY = -rb.linearVelocityY;
        }
        else if (collision.transform.CompareTag("Player"))
        {
            gm.useGum(true);
            Destroy(gameObject);
        }
    }

   
}
