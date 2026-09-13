using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public CanvasGroup settingsPanelGroup;
    public KeyCode toggleKey = KeyCode.Escape;
    public PlayerMovement playerMovement;

    private bool isOpen;

    void Start()
    {
        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                playerMovement = player.GetComponent<PlayerMovement>();
            }
        }

        SetOpen(false);
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            SetOpen(!isOpen);
        }
    }

    public void SetOpen(bool open) {
        isOpen = open;

        if (settingsPanelGroup != null) {
            settingsPanelGroup.alpha = open ? 1f : 0f;
            settingsPanelGroup.interactable = open;
            settingsPanelGroup.blocksRaycasts = open;
        }

        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        if (playerMovement != null) {
            playerMovement.enabled = !open;
        }
    }
}