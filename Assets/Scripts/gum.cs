using UnityEngine;

public class gum : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 1.0f;
    private Vector3 dir;
    private float decayTimer = 3f;
    void Start()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f; // Important for 2D

        dir = mouseWorld - transform.position;
        dir = dir.normalized;
    }

    // Update is called once per frame
    void Update()
    {  
        decayTimer -= Time.deltaTime;
        if (decayTimer < 0)
        {
            Destroy(gameObject);
        }
        transform.position += dir * speed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Nerd"))
        {
            collision.gameObject.GetComponent<NerdEnemy>().GetSilenced();
            Destroy(gameObject);
        }else if (collision.transform.CompareTag("Jock"))
        {
            Destroy(gameObject) ;
        } 
        else if (collision.transform.CompareTag("Ground"))
        {
            Destroy(gameObject) ;
        }
    }
}
