using UnityEngine;
using UnityEngine.UI;

// The title menu, the game's first screen: New Run (to the run start), Settings (the settings overlay) and Exit to
// Desktop (hidden in the Editor and on the web, where quitting means nothing). The logo banner sways gently.
public class TitleUI : MonoBehaviour
{
    public Button newRunButton;
    public Button settingsButton;
    public Button exitButton;
    // The logo banner hanging over the die; it sways a little around its top.
    public RectTransform logo;
    public float swayDegrees = 1.2f;
    public float swaySpeed = 0.9f;

    void Start(){
        AudioManager.PlayMusic(MusicTrack.RunStart);
        newRunButton.onClick.AddListener(() => RunManager.Instance.RestartRun());
        settingsButton.onClick.AddListener(() => { if(SettingsUI.Instance != null) SettingsUI.Instance.Open(); });
        exitButton.onClick.AddListener(Application.Quit);
        exitButton.gameObject.SetActive(!Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer);
    }

    void Update(){
        if(logo != null) logo.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.unscaledTime * swaySpeed) * swayDegrees);
    }
}
