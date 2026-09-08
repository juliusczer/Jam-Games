using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathTrigger : MonoBehaviour {
    public Transform respawnPoint;

    void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        if (respawnPoint == null) {
            return;
        }

        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();

        if (movement != null) {
            movement.Teleport(respawnPoint.position);
        } else {
            other.transform.position = respawnPoint.position;
        }
    }
}