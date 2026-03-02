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
        if (isSilenced) { // first check if nerd is silenced. if so, just show the silnced sprite and nothing else happens.
            sprite.sprite=sprites[1];
            return;
        } else {
            // if not silenced yet, use normal nerd face without gum
            sprite.sprite = sprites[0];
        }

        // we're checking how close the player is to nerd. 
        float playerNerdDistance = Vector2.Distance(transform.position, player.transform.position);

        // if player is too close and the nerd wasn't silenced, you lose
        if (playerNerdDistance < detectionRange) {
            Debug.Log("Nerd sees you!");
            gm.loseGame();
            // GetSilenced(); playing around with the behavior. for now, if you don't silence nerd in time, you lose
            // gm.useGum(true);
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

    private void OnTriggerEnter2D(Collider2D other) {
        // When gum hits nerd's face, silence him
        if (other.CompareTag("Gum")) {
            GetSilenced();
            Destroy(other.gameObject);
        }
    }
}
