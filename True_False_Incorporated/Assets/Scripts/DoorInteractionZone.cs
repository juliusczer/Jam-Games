using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorInteractionZone : MonoBehaviour {
    public SlidingDoor door;

    void Reset() {
        GetComponent<Collider>().isTrigger = true;
        if (door == null) {
            door = GetComponentInParent<SlidingDoor>();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player") || door == null) {
            return;
        }
        door.OnPlayerEnterRange();
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player") || door == null) {
            return;
        }
        door.OnPlayerExitRange();
    }
}