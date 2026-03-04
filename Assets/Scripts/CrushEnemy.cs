using System.Collections;
using UnityEngine;

public class CrushEnemy : MonoBehaviour
{
    public float starStruckDuration = 3f; // how long our player is frozen for
    public float detectionRange = 3f; // how close our player can get before he gets starstruck

    private GameObject player;
    private PlayerMovement pc;
    private GameManager gm;
    private bool triggered = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerMovement>();
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        if (triggered) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (dist < detectionRange)
        {
            triggered = true;
            StartCoroutine(StarStruck());
        }
    }

    IEnumerator StarStruck()
    {
        pc.isStarStruck = true;
        gm.SetHint("You saw your crush... can't move!", true);

        yield return new WaitForSeconds(starStruckDuration);

        pc.isStarStruck = false;
        gm.SetHint("", false);
        triggered = false; // lets it trigger again if player walks back into range
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.8f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}