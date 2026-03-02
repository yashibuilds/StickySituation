using UnityEngine;

public class NerdEnemy : MonoBehaviour
{
    public bool isSilenced = false; // tells you if the nerd has gum on his face or not
    public float detectionRange = 3.67f; // tells you how close the player gets before the nerd starts tattling.
    private GameObject player; // storing the player so we can access position info about them

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isSilenced) { // first check if nerd is silenced. if so, nothing happens.
            return;
        }

        float playerNerdDistance = Vector2.Distance(transform.position, player.transform.position);
        if (playerNerdDistance < detectionRange) {
            Debug.Log("Nerd sees you!");
        }
    }

    public void GetSilenced() {
        isSilenced = true;
        // use a gum (life). haven't set up GameManager so i'll wait to do this
    }

    // if gum collides with nerd, we need to trigger GetSilenced
    // write function later 
}
