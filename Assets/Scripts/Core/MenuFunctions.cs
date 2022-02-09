using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public RectTransform mainMenuButtons;
    public RectTransform tutorialInfo;
    public SoundEffects sfx;

    AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        _source.PlayOneShot(sfx.pigIdle);
        SceneManager.LoadSceneAsync("Level Design");
    }

    public void Tutorial()
    {
        _source.PlayOneShot(sfx.menuClick);
        mainMenuButtons.gameObject.SetActive(false);
        tutorialInfo.gameObject.SetActive(true);
    }

    public void CloseTutorial()
    {
        _source.PlayOneShot(sfx.menuClick);
        mainMenuButtons.gameObject.SetActive(true);
        tutorialInfo.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
