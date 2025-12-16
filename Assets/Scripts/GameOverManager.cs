using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button respawnButton;
    public Button mainMenuButton;

    [Header("Scene Names")]
    public string gameSceneName = "Demo_Scene";      
    public string mainMenuSceneName = "MainMenu";  

    [Header("Audio")]
    public AudioClip buttonClickSound;

    private AudioSource audioSource;

    void Start()
    {
        // audio setup (if time to implement)
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

        // shows cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ensures time is running
        Time.timeScale = 1f;

        // button listeners
        if (respawnButton) respawnButton.onClick.AddListener(OnRespawnClicked);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void OnRespawnClicked()
    {
        PlayClickSound();
        Time.timeScale = 1f;

        // loads game scene
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnMainMenuClicked()
    {
        PlayClickSound();
        Time.timeScale = 1f;

        // loads main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void PlayClickSound()
    {
        if (buttonClickSound && audioSource)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}