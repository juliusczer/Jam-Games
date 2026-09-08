using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WinTrigger : MonoBehaviour
{
    public GameObject lightToActivate;
    public string winMessage = "You Found The Exit";

    private bool hasTriggered;

    void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;

        if (lightToActivate != null)
            lightToActivate.SetActive(true);

        //InteractionTooltipUI.Instance?.Show(winMessage);
    }
}