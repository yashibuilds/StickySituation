using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class JockEnemy : MonoBehaviour
{
    public GameObject ball;
    public Sprite idle;
    public Sprite attack;
    private GameObject player;
    private SpriteRenderer sr;
    public float cooldown = 3;
    public float thresh = 5;
    private float elapsed = 0;
    private float dir = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (player.transform.position.x > transform.position.x)
        {
            sr.flipX = true;
            dir = 1;
        } else
        {
            sr.flipX = false;
            dir = -1;
        }
        if (elapsed > cooldown)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < thresh)
            {
                elapsed = 0;
                StartCoroutine(throwBall());
            }
        }
    }

    IEnumerator throwBall()
    {
       
        sr.sprite = attack;
        yield return new WaitForSeconds(0.2f);
        Instantiate(ball,transform.position+ new Vector3(dir*1,0), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        sr.sprite = idle;
    }
}
