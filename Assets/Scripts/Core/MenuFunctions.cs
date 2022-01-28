using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public RectTransform mainMenuButtons;
    public RectTransform tutorialInfo;

    public void StartGame()
    {
        SceneManager.LoadScene("Level Design");
    }

    public void Tutorial()
    {
        mainMenuButtons.gameObject.SetActive(false);
        tutorialInfo.gameObject.SetActive(true);
    }

    public void CloseTutorial()
    {
        mainMenuButtons.gameObject.SetActive(true);
        tutorialInfo.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
