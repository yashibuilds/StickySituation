using UnityEngine;

public class gum : MonoBehaviour
{
    public float speed = 1.0f;
    private Vector3 dir;
    private float decayTimer = 3f;
    private bool hasCustomDirection = false;

    public void SetDirection(Vector2 direction)
    {
        dir = new Vector3(direction.x, direction.y, 0f).normalized;
        hasCustomDirection = true;
    }

    void Start()
    {
        if (!hasCustomDirection)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            dir = mouseWorld - transform.position;
            dir = dir.normalized;
        }
    }

    void Update()
    {
        decayTimer -= Time.deltaTime;
        if (decayTimer < 0)
        {
            Destroy(gameObject);
        }
        transform.position += dir * speed * Time.deltaTime;
    }
    private void HandleHit(Transform hitTransform)
    {
        if (hitTransform.CompareTag("Nerd"))
        {
            NerdEnemy nerd = hitTransform.gameObject.GetComponent<NerdEnemy>();
            if (nerd != null)
            {
                nerd.GetSilenced();
            }
            Destroy(gameObject);
        }
        else if (hitTransform.CompareTag("Jock"))
        {
            Destroy(gameObject);
        }
        else if (hitTransform.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.transform);
    }
}
