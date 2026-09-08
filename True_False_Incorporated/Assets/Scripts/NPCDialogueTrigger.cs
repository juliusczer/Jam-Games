using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCDialogueTrigger : MonoBehaviour {

    [TextArea(2, 4)]
    public string dialogueText;
    public AudioClip voiceClip;

    public bool repeatable = true;

    private AudioSource audioSource;
    private bool hasShownOnce;

    void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        if (!repeatable && hasShownOnce) {
            return;
        }

        hasShownOnce = true;

        InteractionTooltipUI.Instance?.Show(dialogueText);

        if (voiceClip != null && audioSource != null) {
            audioSource.PlayOneShot(voiceClip);
        }
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        InteractionTooltipUI.Instance?.Hide();
    }
}