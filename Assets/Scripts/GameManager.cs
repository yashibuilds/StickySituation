using TMPro;
using UnityEngine;
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
    [Tooltip("Temporary: disables fail screen and game over state when losing.")]
    public bool suppressLoseScreen = true;
    [Tooltip("Optional: shows hints like 'Press SPACE to use gum' when near the nerd.")]
    public TextMeshProUGUI hintText;

    private void Awake()
    {
        suppressLoseScreen = true;
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
            loseGame();
        }
    }

    public void loseGame()
    {
        if (suppressLoseScreen)
        {
            SetHint("", false);
            return;
        }

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
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            loseGame();
        }
        text.text = "Remaining Time: " + Mathf.Max(Mathf.FloorToInt(timeLeft),0).ToString();
    }
}
