using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button respawnButton;
    public Button mainMenuButton;

    [Header("Scene Names")]
    public string gameSceneName = "Demo_Scene";      // Your game scene
    public string mainMenuSceneName = "MainMenu";    // Your main menu scene

    [Header("Audio")]
    public AudioClip buttonClickSound;

    private AudioSource audioSource;

    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Make sure time is running
        Time.timeScale = 1f;

        // Setup button listeners
        if (respawnButton) respawnButton.onClick.AddListener(OnRespawnClicked);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void OnRespawnClicked()
    {
        PlayClickSound();

        // Reset time scale in case it was paused
        Time.timeScale = 1f;

        // Load the game scene (this will reset everything)
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnMainMenuClicked()
    {
        PlayClickSound();

        // Reset time scale
        Time.timeScale = 1f;

        // Load main menu scene
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