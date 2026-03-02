using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool isFloor(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer("Ground");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFloor(collision.gameObject))
        {
            GetComponentInParent<PlayerMovement>().onGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GetComponentInParent<PlayerMovement>().onGround = false;
    }
}
