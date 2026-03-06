using System.Collections;
using UnityEngine;

public class CrushEnemy : MonoBehaviour
{
    public float starStruckDuration = 1.2f;
    public float detectionRange = 3f;
    public float retriggerPadding = 0.75f;
    public float cooldownAfterEffect = 0.5f;

    private GameObject player;
    private PlayerMovement pc;
    private GameManager gm;
    private bool waitingForExit = false;
    private bool effectRunning = false;
    private float nextAllowedTriggerTime = 0f;
    private bool hasTriggeredOnce = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            pc = player.GetComponent<PlayerMovement>();
        }

        GameObject gmObj = GameObject.FindWithTag("GameController");
        if (gmObj != null)
        {
            gm = gmObj.GetComponent<GameManager>();
        }
    }

    void Update()
    {
        if (player == null || pc == null || gm == null || gm.gameOver) return;
        if (hasTriggeredOnce) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (waitingForExit)
        {
            if (dist > detectionRange + retriggerPadding)
            {
                waitingForExit = false;
            }
            return;
        }

        if (!effectRunning && Time.time >= nextAllowedTriggerTime && dist <= detectionRange)
        {
            waitingForExit = true;
            hasTriggeredOnce = true;
            StartCoroutine(StarStruck());
        }
    }

    IEnumerator StarStruck()
    {
        effectRunning = true;
        pc.SetStarStruckState(true);
        gm.SetHint("You saw your crush... can't move!", true);

        float timer = 0f;
        while (timer < starStruckDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        pc.SetStarStruckState(false);
        gm.SetHint("", false);
        nextAllowedTriggerTime = Time.time + cooldownAfterEffect;
        effectRunning = false;
    }

    private void OnDisable()
    {
        if (pc != null)
        {
            pc.SetStarStruckState(false);
        }
    }

    public void ResetForSpawn()
    {
        waitingForExit = false;
        effectRunning = false;
        nextAllowedTriggerTime = 0f;
        hasTriggeredOnce = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.8f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
