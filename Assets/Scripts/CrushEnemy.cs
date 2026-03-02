using UnityEngine;

public class CrushEnemy : MonoBehaviour
{
    public float starStruckDuration = 3f;
    public float detectionRange = 3.67f;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
