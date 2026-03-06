using System.Collections;
using UnityEngine;

public class JockEnemy : MonoBehaviour
{
    public GameObject ball;
    public Sprite idle;
    public Sprite attack;
    private GameObject player;
    private SpriteRenderer sr;
    public float cooldown = 3f;
    public float thresh = 5f;
    private float elapsed = 0f;
    private float dir = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        elapsed += Time.deltaTime;

        if (player.transform.position.x > transform.position.x)
        {
            sr.flipX = true;
            dir = 1f;
        }
        else
        {
            sr.flipX = false;
            dir = -1f;
        }

        if (elapsed > cooldown && Vector2.Distance(player.transform.position, transform.position) < thresh)
        {
            elapsed = 0f;
            StartCoroutine(ThrowBall());
        }
    }

    IEnumerator ThrowBall()
    {
        sr.sprite = attack;
        yield return new WaitForSeconds(0.2f);
        Instantiate(ball, transform.position + new Vector3(dir * 1f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        sr.sprite = idle;
    }
}
