using UnityEngine;
using TMPro;

// The single tooltip panel of a scene. TooltipTrigger shows it next to the pointer.
public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    public RectTransform panel;
    public TextMeshProUGUI text;
    // Gap between the pointer and the panel, in canvas units.
    public Vector2 offset = new Vector2(24f, -24f);

    private RectTransform canvasRect;

    void Awake(){
        Instance = this;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Hide();
    }

    void OnDestroy(){
        if(Instance == this){
            Instance = null;
        }
    }

    public void Show(string content, Vector2 screenPosition){
        if(string.IsNullOrEmpty(content)){
            Hide();
            return;
        }
        text.text = content;
        panel.gameObject.SetActive(true);
        panel.SetAsLastSibling();
        // Size the panel to the new text before placing it.
        Canvas.ForceUpdateCanvases();

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out local);
        panel.anchoredPosition = Clamp(local + offset);
    }

    public void Hide(){
        panel.gameObject.SetActive(false);
    }

    // Keeps the panel inside the canvas (panel pivot is top-left, canvas pivot is centered).
    private Vector2 Clamp(Vector2 position){
        Vector2 half = canvasRect.rect.size / 2f;
        Vector2 size = panel.rect.size;
        if(position.x + size.x > half.x) position.x = half.x - size.x;
        if(position.x < -half.x) position.x = -half.x;
        if(position.y - size.y < -half.y) position.y = -half.y + size.y;
        if(position.y > half.y) position.y = half.y;
        return position;
    }
}
