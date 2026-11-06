using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Shows a keyword's definition in the scene's TooltipUI while the pointer is over that keyword in this text.
// The keywords come from GameText.Colorize, which wraps them in TMP links. Works on Screen Space - Overlay canvases.
[RequireComponent(typeof(TextMeshProUGUI))]
public class KeywordLinkTooltip : MonoBehaviour
{
    private TextMeshProUGUI text;
    // The link currently shown; -1 when none.
    private int shownLink = -1;

    void Awake(){
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update(){
        if(TooltipUI.Instance == null || Mouse.current == null) return;
        Vector2 pointer = Mouse.current.position.ReadValue();
        int link = TMP_TextUtilities.FindIntersectingLink(text, pointer, null);
        if(link == shownLink) return;

        if(link < 0){
            TooltipUI.Instance.Hide();
            shownLink = -1;
            return;
        }
        shownLink = link;
        TooltipUI.Instance.Show(GameText.KeywordTooltip(text.textInfo.linkInfo[link].GetLinkID()), pointer);
    }

    void OnDisable(){
        if(shownLink >= 0 && TooltipUI.Instance != null) TooltipUI.Instance.Hide();
        shownLink = -1;
    }
}
