using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public RectTransform mainMenuButtons;
    public RectTransform buttonsImage;
    public RectTransform tutorialInfo;
    public SoundEffects sfx;

    AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        _source.PlayOneShot(sfx.menuClick);
        SceneManager.LoadSceneAsync("Level Design");
    }

    public void Tutorial()
    {
        _source.PlayOneShot(sfx.menuClick);
        mainMenuButtons.gameObject.SetActive(false);
        tutorialInfo.gameObject.SetActive(true);
        buttonsImage.gameObject.SetActive(false);
    }

    public void CloseTutorial()
    {
        _source.PlayOneShot(sfx.menuClick);
        mainMenuButtons.gameObject.SetActive(true);
        tutorialInfo.gameObject.SetActive(false);
        buttonsImage.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
