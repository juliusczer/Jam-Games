using UnityEngine;

public class FaceCamera : MonoBehaviour {
    
    public Transform target;
    public bool lockYRotation = true;

    void LateUpdate() {
        if (target == null) {
            if (Camera.main != null) {
                target = Camera.main.transform;
            } else {
                return;
            }
        }

        Vector3 directionAwayFromTarget = transform.position - target.position;

        if (lockYRotation) {
            directionAwayFromTarget.y = 0f;
        }

        if (directionAwayFromTarget.sqrMagnitude < 0.0001f) {
            return;
        }

        transform.rotation = Quaternion.LookRotation(directionAwayFromTarget);
    }
}