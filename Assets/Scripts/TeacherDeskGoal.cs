using UnityEngine;

public class TeacherDeskGoal : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        GameManager gm = GameObject.FindWithTag("GameController")?.GetComponent<GameManager>();
        if (gm != null && !gm.gameOver)
        {
            gm.winGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager gm = GameObject.FindWithTag("GameController")?.GetComponent<GameManager>();
        if (gm != null && !gm.gameOver)
        {
            gm.winGame();
        }
    }
}
