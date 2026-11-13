using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

// The settings screen (GDD "Sound"): a gear in the top right corner of every screen (or Esc) opens a panel with the
// music and effects volumes, How to Play and a Quit button for builds. How to Play also opens by itself the first time
// the game starts on this computer, for players who have never seen the game. It is a prefab of its own (Resources/SettingsOverlay) that
// RunManager creates once and keeps through scene loads, so no scene has to hold a copy.
public class SettingsUI : MonoBehaviour
{
    public Button gearButton;
    public GameObject panel;
    public Slider musicSlider;
    public TextMeshProUGUI musicValue;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValue;
    public Button closeButton;
    public Button quitButton;
    // The short rules page (GDD "Playtest").
    public Button helpButton;
    public GameObject helpPanel;
    public Button helpCloseButton;

    private const string SeenHelpKey = "seenHowToPlay";

    void Start(){
        musicSlider.value = AudioManager.MusicVolume;
        sfxSlider.value = AudioManager.SfxVolume;
        ShowValues();
        musicSlider.onValueChanged.AddListener(v => { AudioManager.MusicVolume = v; ShowValues(); });
        // Moving the effects slider plays a sample at the new volume (at most a few times a second), so the player
        // hears what they chose.
        sfxSlider.onValueChanged.AddListener(v => {
            AudioManager.SfxVolume = v;
            ShowValues();
            if(Time.unscaledTime - lastSample > 0.25f){
                lastSample = Time.unscaledTime;
                AudioManager.Play(Sfx.Buy);
            }
        });
        gearButton.onClick.AddListener(Toggle);
        closeButton.onClick.AddListener(() => SetOpen(false));
        quitButton.onClick.AddListener(Quit);
        // Quitting means nothing inside the Editor or in a web build.
        bool canQuit = !Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer;
        quitButton.gameObject.SetActive(canQuit);
        if(!canQuit){
            RectTransform close = (RectTransform)closeButton.transform;
            close.anchoredPosition = new Vector2(0f, close.anchoredPosition.y);
        }
        SetOpen(false);
        if(helpPanel != null){
            helpButton.onClick.AddListener(() => ShowHelp(true));
            helpCloseButton.onClick.AddListener(() => ShowHelp(false));
            bool firstTime = PlayerPrefs.GetInt(SeenHelpKey, 0) == 0;
            ShowHelp(firstTime);
            if(firstTime){
                PlayerPrefs.SetInt(SeenHelpKey, 1);
                PlayerPrefs.Save();
            }
        }
    }

    void Update(){
        if(Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if(helpPanel != null && helpPanel.activeSelf) ShowHelp(false);
        else Toggle();
    }

    private void ShowHelp(bool show){
        helpPanel.SetActive(show);
        if(show) panel.SetActive(false);
    }

    private float lastSample;

    private void Toggle(){
        SetOpen(!panel.activeSelf);
    }

    private void SetOpen(bool open){
        panel.SetActive(open);
        if(!open) PlayerPrefs.Save();
    }

    private void ShowValues(){
        musicValue.text = Mathf.RoundToInt(musicSlider.value * 100f) + "%";
        sfxValue.text = Mathf.RoundToInt(sfxSlider.value * 100f) + "%";
    }

    private void Quit(){
        PlayerPrefs.Save();
        Application.Quit();
    }
}
