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

    // Abandon Run: built in Start as a copy of the How to Play button, under it; hidden on the title screen. The first
    // click asks again, the second ends the run and goes back to the title menu.
    private Button abandonButton;
    private TextMeshProUGUI abandonText;
    private bool abandonArmed;
    private Vector2 helpHome;
    private const float AbandonOffset = 200f;
    private const string AbandonLabel = "Abandon Run";
    private const string AbandonConfirmLabel = "<color=#E05A4A>Really abandon?</color>";
    public const string TitleSceneName = "Title";

    private const string SeenHelpKey = "seenHowToPlay";

    // The one settings overlay (it lives through scene loads), so other screens can open it (the title menu).
    public static SettingsUI Instance { get; private set; }
    void Awake(){ Instance = this; }
    public void Open(){ SetOpen(true); }

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
        BuildAbandonButton();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) => SetOpen(false);
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
        if(abandonButton != null){
            abandonArmed = false;
            abandonText.text = AbandonLabel;
            bool inRun = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != TitleSceneName;
            abandonButton.gameObject.SetActive(inRun);
            ((RectTransform)helpButton.transform).anchoredPosition = helpHome - new Vector2(inRun ? AbandonOffset : 0f, 0f);
        }
    }

    private void BuildAbandonButton(){
        if(helpButton == null) return;
        GameObject copy = Instantiate(helpButton.gameObject, helpButton.transform.parent);
        copy.name = "AbandonButton";
        RectTransform rect = (RectTransform)copy.transform;
        RectTransform help = (RectTransform)helpButton.transform;
        // Beside How to Play, which moves left while Abandon Run shows (SetOpen).
        helpHome = help.anchoredPosition;
        rect.anchoredPosition = helpHome + new Vector2(AbandonOffset, 0f);
        abandonButton = copy.GetComponent<Button>();
        abandonButton.onClick = new Button.ButtonClickedEvent();
        abandonText = copy.GetComponentInChildren<TextMeshProUGUI>();
        abandonText.text = AbandonLabel;
        abandonButton.onClick.AddListener(OnAbandonClicked);
    }

    private void OnAbandonClicked(){
        if(!abandonArmed){
            abandonArmed = true;
            abandonText.text = AbandonConfirmLabel;
            return;
        }
        SetOpen(false);
        if(RunManager.Instance != null) RunManager.Instance.AbandonRun();
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
