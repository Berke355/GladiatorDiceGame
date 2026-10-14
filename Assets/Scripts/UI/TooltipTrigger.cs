using UnityEngine;
using UnityEngine.EventSystems;

// Shows its text in the scene's TooltipUI while the pointer is over this object.
// The object needs a Graphic with Raycast Target on (e.g. a background Image) to receive pointer events.
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string text;

    public void OnPointerEnter(PointerEventData eventData){
        if(TooltipUI.Instance != null){
            TooltipUI.Instance.Show(text, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(TooltipUI.Instance != null){
            TooltipUI.Instance.Hide();
        }
    }

    void OnDisable(){
        if(TooltipUI.Instance != null){
            TooltipUI.Instance.Hide();
        }
    }
}
