using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject playPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public void OnPlayButton()
    {
        mainMenuPanel.SetActive(false);
        playPanel.SetActive(true);
    }

    public void OnSettingsButton()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnCreditsButton()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OnBackToMainMenuButton()
    {
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
        Debug.Log("Quit button pressed. Application will close.");
    }

    //TRACKS

    public enum Track
    {
        D1,
        Germany,
        Italy,
        Japan,
        USA,
    }

    public void LoadTrack(int trackIndex)
    {
        Track SelectedTrack = (Track)trackIndex;
        SceneManager.LoadScene(SelectedTrack.ToString());
    }
    
}
