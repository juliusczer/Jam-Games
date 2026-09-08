using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour {

    // General Parameters
    public float dropDistance = 3f;
    public float moveSpeed = 2f;
    public string openPrompt = "Press E to Open";
    public string closePrompt = "Press E to Close";
    public KeyCode interactKey = KeyCode.E;
    public string InteractionPrompt => isOpen ? closePrompt : openPrompt;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen;
    private bool isMoving;
    private bool playerInRange;

    void Start() {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.down * dropDistance;
    }

    void Update() {
        if (playerInRange && Input.GetKeyDown(interactKey)) {
            Interact(null);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        OnPlayerEnterRange();
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        OnPlayerExitRange();
    }

    public void OnPlayerEnterRange() {
        playerInRange = true;
        InteractionTooltipUI.Instance?.Show(InteractionPrompt);
    }

    public void OnPlayerExitRange() {
        playerInRange = false;
        InteractionTooltipUI.Instance?.Hide();
    }

    public void Interact(GameObject interactor) {
        if (isMoving) {
            return;
        }
        StartCoroutine(ToggleDoor());
    }

    IEnumerator ToggleDoor() {
        isMoving = true;
        Vector3 targetPos = isOpen ? closedPosition : openPosition;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isOpen = !isOpen;
        isMoving = false;

        if (playerInRange) {
            InteractionTooltipUI.Instance?.Show(InteractionPrompt);
        }
    }
}