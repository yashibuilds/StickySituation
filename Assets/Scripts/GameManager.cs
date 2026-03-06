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
    private GameObject play; 
    private PlayerMovement player;
    public GameObject winPanel;
    public GameObject losePanel;
    [Tooltip("Optional: shows hints like 'Press SPACE to use gum' when near the nerd.")]
    public TextMeshProUGUI hintText;

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
        play = GameObject.FindWithTag("Player");
        player = play.GetComponent<PlayerMovement>();
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
        if (gameOver) return;
        if (hit && player != null && player.isStarStruck) return;

        if (gumCount <= 0)
        {
            if (hit)
            {
                loseGame();
            }
            return;
        }

        gumCount = gumCount - 1;
        if (gumCount >= 0 && gumCount < gums.Length)
        {
            gums[gumCount].sprite = dmg;
        }

        if (hit && player != null)
        {
            player.takeHit();
        }

        if (hit && gumCount <= 0)
        {
            gameOver = true;
            loseGame();
        }
    }

    public void loseGame()
    {
        gameOver = true;
        losePanel.SetActive(true);
        SetHint("", false);
        play.SetActive(false);
    }
    public void winGame()
    {
        gameOver = true;
        winPanel.SetActive(true);
        SetHint("", false);
        play.SetActive(false);  
    }

    public void SetHint(string message, bool show)
    {
        if (hintText == null) return;
        hintText.gameObject.SetActive(show);
        if (show) hintText.text = message;
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        timeLeft = timeLeft - Time.deltaTime;
        
=======
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            loseGame();
        }

>>>>>>> 5222bed8c13ffe2c7caf3e8ae3ccd33ec1fcaebc
        text.text = "Remaining Time: " + Mathf.Max(Mathf.FloorToInt(timeLeft),0).ToString();
        if (timeLeft<0)
        {
            loseGame() ;
        }
    }
}
