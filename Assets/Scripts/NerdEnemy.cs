using UnityEngine;
using UnityEngine.UI;
public class NerdEnemy : MonoBehaviour
{
    public bool isSilenced = false; // tells you if the nerd has gum on his face or not
    public float detectionRange = 3.67f; // tells you how close the player gets before the nerd starts tattling.
    public Sprite[] sprites;
    private GameObject player; // storing the player so we can access position info about them
    private GameManager gm;
    private SpriteRenderer sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        sprite =  GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSilenced) { // first check if nerd is silenced. if so, nothing happens.
            sprite.sprite=sprites[1];
            return;
        }

        float playerNerdDistance = Vector2.Distance(transform.position, player.transform.position);
        if (playerNerdDistance < detectionRange) {
            Debug.Log("Nerd sees you!");
            GetSilenced();
            gm.useGum(true);
        }
    }

    public void GetSilenced() {
        isSilenced = true;
        
        // use a gum (life). haven't set up GameManager so i'll wait to do this
    }

    // if gum collides with nerd, we need to trigger GetSilenced
    // write function later 
}
