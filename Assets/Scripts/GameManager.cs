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
    [Tooltip("Set true to suppress lose panel/game over while testing.")]
    public bool suppressLoseScreen = false;
    [Tooltip("Optional: shows hints like 'Press SPACE to use gum' when near the nerd.")]
    public TextMeshProUGUI hintText;

    private void ResolvePlayerRefs()
    {
        if (play == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found != null) play = found;
        }

        if (player == null && play != null)
        {
            player = play.GetComponent<PlayerMovement>();
        }
    }

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
        ResolvePlayerRefs();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        suppressLoseScreen = false;
        ResolvePlayerRefs();

        foreach (Image gum in gums) {
            gum.sprite = hp;
        }
        if (textobj != null)
        {
            text = textobj.GetComponent<TextMeshProUGUI>();
        }

        if (GetComponent<EnemySpawner>() == null)
        {
            gameObject.AddComponent<EnemySpawner>();
        }
    }
    public void useGum(bool hit = false)
    {
        if (gameOver) return;
        ResolvePlayerRefs();

        if (gumCount <= 0)
        {
            loseGame();
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

        if (gumCount <= 0)
        {
            loseGame();
        }
    }

    public void loseGame()
    {
        gameOver = true;
        ResolvePlayerRefs();

        if (suppressLoseScreen)
        {
            SetHint("", false);
            return;
        }

        if (losePanel != null) losePanel.SetActive(true);
        SetHint("", false);
        if (play != null) play.SetActive(false);
    }
    public void winGame()
    {
        gameOver = true;
        ResolvePlayerRefs();
        if (winPanel != null) winPanel.SetActive(true);
        SetHint("", false);
        if (play != null) play.SetActive(false);
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
        ResolvePlayerRefs();

        if (text == null && textobj != null)
        {
            text = textobj.GetComponent<TextMeshProUGUI>();
        }

        if (gumCount <= 0)
        {
            loseGame();
            return;
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            loseGame();
        }
        if (text != null)
        {
            text.text = "Remaining Time: " + Mathf.Max(Mathf.FloorToInt(timeLeft),0).ToString();
        }
    }
}
