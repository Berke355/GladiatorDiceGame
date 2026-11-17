using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

// The map's pencil and eraser (GDD "Map"): the player draws their planned route in ink over the parchment. A chunky
// texture (a quarter of the canvas, so the ink is pixel art like the rest) lies over the nodes; it catches the mouse
// only while a tool is in hand, so the nodes stay clickable otherwise. A right-button drag draws without picking the
// pencil. The ink lives for the act (RunManager.mapInk), so it survives battles and shops, and a new act starts clean.
public class MapDrawing : MonoBehaviour
{
    public enum Tool { None, Pencil, Eraser }

    private const int Downscale = 4;
    private const int PencilSize = 2;
    private const int EraserRadius = 7;
    private static readonly Color32 Ink = new Color32(70, 26, 16, 235);
    private static readonly Color32 Clear = new Color32(0, 0, 0, 0);

    private RawImage surface;
    private Texture2D texture;
    private Color32[] pixels;
    private int width, height;
    private bool dirty;
    private Vector2? last;
    private Tool tool;
    private Button pencilButton, eraserButton;
    private System.Func<bool> blocked;

    public Tool Current => tool;

    // Builds the ink layer over the map (right after mapArea, so it covers lines and nodes) and the tool buttons, copied
    // from a button of the scene. blocked: true while a panel (a rest, a reward) covers the map.
    public static MapDrawing Build(RectTransform mapArea, Button buttonStyle, System.Func<bool> blocked){
        RectTransform canvas = (RectTransform)mapArea.parent;
        GameObject layer = new GameObject("MapInk", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        layer.transform.SetParent(canvas, false);
        layer.transform.SetSiblingIndex(mapArea.GetSiblingIndex() + 1);
        RectTransform rect = (RectTransform)layer.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        MapDrawing drawing = layer.AddComponent<MapDrawing>();
        drawing.blocked = blocked;
        drawing.Setup(canvas.rect.size);
        drawing.BuildButtons(canvas, buttonStyle);
        return drawing;
    }

    private void Setup(Vector2 canvasSize){
        width = Mathf.Max(1, Mathf.RoundToInt(canvasSize.x / Downscale));
        height = Mathf.Max(1, Mathf.RoundToInt(canvasSize.y / Downscale));
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false){ filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        RunManager manager = RunManager.Instance;
        bool kept = manager != null && manager.mapInk != null && manager.mapInk.Length == width * height && manager.mapInkAct == manager.run.act;
        pixels = kept ? manager.mapInk : new Color32[width * height];
        if(manager != null){
            manager.mapInk = pixels;
            manager.mapInkAct = manager.run.act;
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        surface = GetComponent<RawImage>();
        surface.texture = texture;
        surface.raycastTarget = false;
    }

    private void BuildButtons(RectTransform canvas, Button style){
        if(style == null) return;
        Image styleImage = style.GetComponent<Image>();
        if(styleImage != null) buttonColor = styleImage.color;
        pencilButton = MakeButton(canvas, style, "Pencil", new Vector2(-1165f, 110f), () => Pick(Tool.Pencil));
        eraserButton = MakeButton(canvas, style, "Eraser", new Vector2(-1165f, 20f), () => Pick(Tool.Eraser));
        MakeButton(canvas, style, "Clear", new Vector2(-1165f, -70f), ClearAll);
        Pick(Tool.None);
    }

    private static Button MakeButton(RectTransform canvas, Button style, string label, Vector2 position, UnityEngine.Events.UnityAction onClick){
        Button button = Instantiate(style, canvas);
        button.name = label + "Button";
        button.gameObject.SetActive(true);
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(onClick);
        RectTransform rect = (RectTransform)button.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(170f, 74f);
        rect.anchoredPosition = position;
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if(text != null){
            text.text = label;
            text.enableAutoSizing = false;
            text.fontSize = 34f;
        }
        return button;
    }

    // Picking the tool in hand puts it down.
    private void Pick(Tool picked){
        tool = tool == picked ? Tool.None : picked;
        surface.raycastTarget = tool != Tool.None;
        Highlight(pencilButton, tool == Tool.Pencil);
        Highlight(eraserButton, tool == Tool.Eraser);
    }

    private Color buttonColor = Color.white;

    private void Highlight(Button button, bool on){
        if(button == null) return;
        Image image = button.GetComponent<Image>();
        if(image != null) image.color = on ? Color.Lerp(buttonColor, new Color(1f, 0.78f, 0.3f, 1f), 0.6f) : buttonColor;
        button.transform.localScale = on ? new Vector3(1.08f, 1.08f, 1f) : Vector3.one;
    }

    private void ClearAll(){
        for(int i = 0; i < pixels.Length; i++) pixels[i] = Clear;
        dirty = true;
    }

    void Update(){
        Mouse mouse = Mouse.current;
        if(mouse == null || texture == null) return;
        bool panel = blocked != null && blocked();
        if(panel && tool != Tool.None) Pick(Tool.None);
        // A stroke starts only on the map itself, never on a button (or the tooltip-free parts of the screen above it).
        if(mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame) stroking = !panel && !OverButton(mouse.position.ReadValue());
        if(!mouse.leftButton.isPressed && !mouse.rightButton.isPressed) stroking = false;
        bool draw = stroking && (tool == Tool.Pencil && mouse.leftButton.isPressed || mouse.rightButton.isPressed);
        bool erase = stroking && tool == Tool.Eraser && mouse.leftButton.isPressed;
        if(draw || erase){
            Vector2 point;
            if(ToTexture(mouse.position.ReadValue(), out point)){
                Stroke(last ?? point, point, erase);
                last = point;
            }
            else last = null;
        }
        else last = null;
        if(dirty){
            texture.SetPixels32(pixels);
            texture.Apply();
            dirty = false;
        }
    }

    private bool stroking;
    private static readonly List<RaycastResult> hits = new List<RaycastResult>();

    private bool OverButton(Vector2 screen){
        if(EventSystem.current == null) return false;
        hits.Clear();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){ position = screen }, hits);
        foreach(RaycastResult hit in hits){
            if(hit.gameObject == gameObject) continue;
            if(hit.gameObject.GetComponentInParent<Selectable>() != null && hit.gameObject.GetComponentInParent<MapNodeUI>() == null) return true;
        }
        return false;
    }

    private bool ToTexture(Vector2 screen, out Vector2 point){
        point = Vector2.zero;
        RectTransform rect = (RectTransform)transform;
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera view = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        Vector2 local;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, view, out local)) return false;
        Rect r = rect.rect;
        point = new Vector2((local.x - r.xMin) / r.width * width, (local.y - r.yMin) / r.height * height);
        return point.x >= 0f && point.y >= 0f && point.x < width && point.y < height;
    }

    // A line of stamps from the last point, so a fast stroke has no gaps.
    private void Stroke(Vector2 from, Vector2 to, bool erase){
        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to)));
        for(int i = 0; i <= steps; i++){
            Vector2 p = Vector2.Lerp(from, to, (float)i / steps);
            if(erase) Stamp((int)p.x, (int)p.y, EraserRadius, Clear, true);
            else Stamp((int)p.x, (int)p.y, PencilSize / 2, Ink, false);
        }
        dirty = true;
    }

    private void Stamp(int cx, int cy, int radius, Color32 color, bool round){
        for(int y = cy - radius; y <= cy + radius; y++){
            if(y < 0 || y >= height) continue;
            for(int x = cx - radius; x <= cx + radius; x++){
                if(x < 0 || x >= width) continue;
                if(round && (x - cx) * (x - cx) + (y - cy) * (y - cy) > radius * radius) continue;
                pixels[y * width + x] = color;
            }
        }
    }

    void OnDestroy(){
        if(texture != null) Destroy(texture);
    }
}
