using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;
    public float timeLeft = 60f;
    public int gumCount = 3;
    public bool gameOver = false;
    public Image[] gums;
    public Sprite hp;
    public Sprite dmg;
    public GameObject textobj;
    private TextMeshProUGUI text;
    private PlayerMovement player;
    public GameObject winPanel;
    public GameObject losePanel;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
        gumCount = 3;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        foreach (Image gum in gums) {
            gum.sprite = hp;
        }
        text = textobj.GetComponent<TextMeshProUGUI>();
    }
    public void useGum(bool hit = false)
    {
        gumCount = gumCount - 1;
        gums[gumCount].sprite = dmg;
        if (hit) { player.takeHit(); }
        if (gumCount <= 0)
        {
            gameOver = true;
            loseGame();
        }
    }

    public void loseGame()
    {
        gameOver = true;
        losePanel.SetActive(true);

    }
    public void winGame()
    {
        gameOver = true;
        winPanel.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        timeLeft = timeLeft - Time.deltaTime;
        text.text = "Remaining Time: " + Mathf.Max(Mathf.FloorToInt(timeLeft),0).ToString();
    }
}
