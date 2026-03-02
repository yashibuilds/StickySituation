using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float moveSpeed = 6.7f;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(h, v).normalized * moveSpeed;
    }
}
