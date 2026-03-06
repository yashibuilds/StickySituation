using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private int floorContactCount = 0;

    void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    bool isFloor(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer("Ground");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFloor(collision.gameObject)) return;

        floorContactCount++;
        if (playerMovement != null)
        {
            playerMovement.onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isFloor(collision.gameObject)) return;

        floorContactCount = Mathf.Max(0, floorContactCount - 1);
        if (playerMovement != null)
        {
            playerMovement.onGround = floorContactCount > 0;
        }
    }
}
