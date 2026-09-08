using UnityEngine;
using UnityEngine.UI;

public class InteractionTooltipUI : MonoBehaviour {
    public static InteractionTooltipUI Instance { 
        get; 
        private set; 
    }

    public GameObject tooltipRoot;
    public Text tooltipText;

    void Awake() {
        Instance = this;
        if (tooltipRoot != null) {
            tooltipRoot.SetActive(false);
        }
    }

    public void Show(string message) {
        if (tooltipRoot != null) {
            tooltipRoot.SetActive(true);
        }

        if (tooltipText != null) {
            tooltipText.text = message;
        }
    }

    public void Hide() {
        if (tooltipRoot != null) {
            tooltipRoot.SetActive(false);
        }
    }
}