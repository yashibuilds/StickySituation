using UnityEngine;
using UnityEngine.UI;
public class NerdEnemy : MonoBehaviour
{
    public bool isSilenced = false; // tells you if the nerd has gum on his face or not
    public float detectionRange = 0.67f; // tells you how close the player gets before the nerd can start tattling.
    public float detectionTime = 1.0f;
    private float detectionTimer = 0f;
    public Sprite[] sprites;
    private GameObject player; // storing the player so we can access position info about them
    private GameManager gm;
    private SpriteRenderer sprite;
    private LineRenderer rangeCircle;
    public bool showRangeInGame = true;

    void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();

        if (showRangeInGame)
            DrawRangeCircle();
    }

    void DrawRangeCircle() // the amount of docs i had to read just for this circle bruh.
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

    // Update is called once per frame
    void Update()
    {
        if (isSilenced) { // first check if nerd is silenced. if so, just show the silnced sprite and nothing else happens.
            sprite.sprite = sprites[1];
            gm.SetHint("", false);
            if (rangeCircle != null) rangeCircle.enabled = false; // hide circle when silenced
            return;
        }
        if (rangeCircle != null) {
            rangeCircle.enabled = true;
        }

        // if not silenced yet, use normal nerd face without gum
        sprite.sprite = sprites[0];

        // we're checking how close the player is to nerd. 
        float playerNerdDistance = Vector2.Distance(transform.position, player.transform.position);

        if (playerNerdDistance < detectionRange)
        {
            // show hint: no "shooting". SPACE uses gum when you're in the circle
            if (gm.gumCount > 0)
                gm.SetHint("In the red circle → press SPACE to use gum", true);
            else
                gm.SetHint("No gum left — run through the circle quickly!", true);

            // if player presses Space and has gum, silence the nerd (no projectile)
            if (gm.gumCount > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                GetSilenced();
                gm.SetHint("", false);
                return;
            }

            // otherwise count time in range, tattle after detectionTime seconds
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionTime)
            {
                Debug.Log("Nerd tattles on you!");
                gm.loseGame();
            }
        }
        else
        {
            // stepped out of range, reset so you can try again or dash through
            detectionTimer = 0f;
            gm.SetHint("", false);
        }
    }

    public void GetSilenced() {
        // if our nerd is already silenced, do nothing 
        if (isSilenced) {
            return;
        }
        // silence snitch nerd. literally #1 opp, would catch hands with him if he was real tbh
        isSilenced = true;
        gm.useGum(true); // spend one gum (lives decrease basically)
        
    }

    // draw detection range in editor 
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Vector3 center = transform.position;
        Gizmos.DrawWireSphere(center, detectionRange);
    }
}
