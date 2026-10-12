using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one die face slot in the Market scene. MarketUI fills it.
public class DieSlotUI : MonoBehaviour
{
    // Clicked to choose which face a bought offer replaces.
    public Button slotButton;
    public TextMeshProUGUI faceText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;
}
