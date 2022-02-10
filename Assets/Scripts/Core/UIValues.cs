using UnityEngine;
using UnityEngine.SceneManagement;

public class UIValues : MonoBehaviour
{
    public PlayerVacuum playerReference;
    public RectTransform gameEndPopup;
    public SoundEffects sfx;
    //public TMPro.TextMeshProUGUI inventory;
    public TMPro.TextMeshProUGUI timer;
    public TMPro.TextMeshProUGUI objective;
    public TMPro.TextMeshProUGUI gameEndText1, gameEndText2;
    public string victoryMessage1, victoryMessage2, defeatMessage1, defeatMessage2;

    int inventoryPigs;
    int totalPigs;
    uint fencedPigs;
    int seconds;
    AudioSource _source;

    void Start()
    {
        if (playerReference == null)
        {
            GameObject scenePlayer = GameObject.Find("Player");
            if (scenePlayer) playerReference = scenePlayer.GetComponent<PlayerVacuum>();
        }
        SetUI();
        _source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GameManager.GameWon += GameWon;
        GameManager.GameLost += GameLost;
    }

    void OnDisable()
    {
        GameManager.GameWon -= GameWon;
        GameManager.GameLost -= GameLost;
    }

    void Update()
    {
        ReadInventory();
        ReadSeconds();
        ReadObjective();
    }

    void SetUI()
    {
        WriteObjective();
        //WriteInventory();
    }

    void ReadInventory()
    {
        if (playerReference == null) return;

        if (inventoryPigs != playerReference.Inventory)
        {
            inventoryPigs = playerReference.Inventory; ;
            //WriteInventory();
        }
    }

    //void WriteInventory()
    //{
    //    inventory.text = inventoryPigs.ToString();
    //}

    void ReadSeconds()
    {
        int readSeconds = (int)GameManager.Singleton.TimeRemaining;
        if (readSeconds != seconds)
        {
            seconds = readSeconds;
            WriteSeconds();
        }
    }

    void WriteSeconds()
    {
        int minutes = seconds / 60;
        timer.text = minutes.ToString() + ":" + (seconds - 60 * minutes).ToString();
    }

    void ReadObjective()
    {
        uint fenced = GameManager.Singleton.PigsFenced;
        if (fenced != fencedPigs)
        {
            fencedPigs = fenced;
            WriteObjective();
        }
    }

    void WriteObjective()
    {
        objective.text = GameManager.Singleton.PigsFenced.ToString() + " / " + GameManager.Singleton.PigsRequired.ToString();
    }

    public void MainMenu()
    {
        _source.PlayOneShot(sfx.menuClick);
        SceneManager.LoadSceneAsync("Title");
    }

    public void Replay()
    {
        _source.PlayOneShot(sfx.menuClick);
        SceneManager.LoadSceneAsync("Level Design");
    }

    void GameWon()
    {
        gameEndText1.text = victoryMessage1;
        gameEndText2.text = victoryMessage2;
        gameEndPopup.gameObject.SetActive(true);
    }

    void GameLost()
    {
        gameEndText1.text = defeatMessage1;
        gameEndText2.text = defeatMessage2;
        gameEndPopup.gameObject.SetActive(true);
    }
}
