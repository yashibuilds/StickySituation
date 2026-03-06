using UnityEngine;
using System.Collections.Generic;

public class NerdEnemy : MonoBehaviour
{
    private static readonly List<NerdEnemy> ActiveNerds = new List<NerdEnemy>();

    public bool isSilenced = false; // tells you whether our nerd has been shot with gum or not
    public float detectionRange = 2.8f; // how close can you get to the nerd before he starts tattling
    public float detectionTime = 1.35f; // how long you have to make it out of his circle before you fail
    private float detectionTimer = 0f; // how long you've been in the circle 
    public Sprite[] sprites; // array containing sprites 
    private GameObject player;
    private GameManager gm;
    private SpriteRenderer sprite;
    private LineRenderer rangeCircle;
    public bool showRangeInGame = true; // this is for testing purposes. trying to see if game works intuitively without this

    void Start()
    {
        GameObject gmObj = GameObject.FindWithTag("GameController");
        if (gmObj != null)
        {
            gm = gmObj.GetComponent<GameManager>();
        }
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();

        if (showRangeInGame)
            DrawRangeCircle();
    }

    void OnEnable() // run this when a nerd appears in the level
    {
        if (!ActiveNerds.Contains(this))
        {
            ActiveNerds.Add(this);
        }
    }

    void OnDisable() // run when a nerd disappears 
    {
        ActiveNerds.Remove(this);
    }

    public static bool TrySilenceNearby(Vector2 playerPosition)
    {
        NerdEnemy closest = GetClosestNearby(playerPosition);
        if (closest == null) return false;
        closest.GetSilenced();
        return true;
    }

    public static NerdEnemy GetClosestNearby(Vector2 playerPosition)
    {
        NerdEnemy closest = null;
        float closestDist = float.MaxValue;

        for (int i = 0; i < ActiveNerds.Count; i++)
        {
            NerdEnemy nerd = ActiveNerds[i];
            if (nerd == null || nerd.isSilenced || nerd.gm == null || nerd.gm.gumCount <= 0 || nerd.gm.gameOver)
            {
                continue;
            }

            float dist = Vector2.Distance(playerPosition, nerd.transform.position);
            if (dist <= nerd.detectionRange && dist < closestDist)
            {
                closest = nerd;
                closestDist = dist;
            }
        }

        return closest;
    }

    void DrawRangeCircle()
    {
        GameObject circleObj = new GameObject("NerdRangeCircle");
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = Vector3.zero;

        rangeCircle = circleObj.AddComponent<LineRenderer>();
        rangeCircle.loop = true;
        rangeCircle.positionCount = 40;
        rangeCircle.useWorldSpace = false;
        rangeCircle.startWidth = 0.15f;
        rangeCircle.endWidth = 0.15f;
        rangeCircle.material = new Material(Shader.Find("Sprites/Default"));
        rangeCircle.startColor = new Color(1f, 0.2f, 0.2f, 0.6f);
        rangeCircle.endColor = new Color(1f, 0.2f, 0.2f, 0.6f);

        float angleStep = 360f / 40f * Mathf.Deg2Rad;
        for (int i = 0; i < 40; i++)
        {
            float angle = i * angleStep;
            rangeCircle.SetPosition(i, new Vector3(Mathf.Cos(angle) * detectionRange, Mathf.Sin(angle) * detectionRange, 0f));
        }
    }

    void Update()
    {
        if (player == null || gm == null)
        {
            return;
        }

        if (isSilenced)
        {
            if (sprites != null && sprites.Length > 1)
            {
                sprite.sprite = sprites[1];
            }
            gm.SetHint("", false);
            if (rangeCircle != null) rangeCircle.enabled = false;
            return;
        }

        if (rangeCircle != null)
        {
            rangeCircle.enabled = true;
        }

        if (sprites != null && sprites.Length > 0)
        {
            sprite.sprite = sprites[0];
        }

        float playerNerdDistance = Vector2.Distance(transform.position, player.transform.position);

        if (playerNerdDistance < detectionRange)
        {
            if (gm.gumCount > 0)
                gm.SetHint("Press SPACE in the red zone to gum the nerd, or sprint through fast.", true);
            else
                gm.SetHint("No gum left — run through the circle quickly!", true);

            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionTime)
            {
                Debug.Log("Nerd tattled. You lose one gum life.");
                gm.useGum(true);
                detectionTimer = 0f;
            }
        }
        else
        {
            detectionTimer = 0f;
            gm.SetHint("", false);
        }
    }

    public void GetSilenced()
    {
        if (isSilenced || gm == null)
        {
            return;
        }

        isSilenced = true;
        detectionTimer = 0f;
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (sprite != null && sprites != null && sprites.Length > 1)
        {
            sprite.sprite = sprites[1];
        }
        if (rangeCircle != null)
        {
            rangeCircle.enabled = false;
        }
        gm.SetHint("", false);
        gm.useGum(false);
    }

    public void ResetForSpawn()
    {
        isSilenced = false;
        detectionTimer = 0f;

        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (sprite != null && sprites != null && sprites.Length > 0)
        {
            sprite.sprite = sprites[0];
        }
        if (rangeCircle != null)
        {
            rangeCircle.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Vector3 center = transform.position;
        Gizmos.DrawWireSphere(center, detectionRange);
    }
}
