using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public CharacterController controller;

    // General parameters
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public float mouseSensitivity = 2.5f;
    public float lookXLimit = 85f;

    // Wall running variables
    public bool enableWallRun = true;
    public LayerMask wallMask;
    public float wallCheckDistance = 0.7f;
    public float wallRunSpeed = 7f;
    public float wallRunMaxDuration = 2.5f;
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 6f;
    public float wallRunReentryDelay = 0.3f;
    public float wallRunTiltAngle = 15f;
    public float tiltSpeed = 8f;
    [Range(1f, 180f)]
    public float maxLookAwayAngle = 180f;

    // Physics Parameters
    private Vector3 velocity;
    private float verticalLookRotation;
    private float currentTilt;
    private float wallLookYaw;
    private float cameraYawOffset;
    private Vector3 lockedWallRight;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallRunning;
    private bool wallOnRight;
    private Vector3 wallNormal;
    private float wallRunTimer;
    private float wallRunCooldown;
    private Collider currentWallCollider;
    private Collider lastJumpWallCollider;

    void Start() {
        if (controller == null) {
            controller = GetComponent<CharacterController>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f) {
            velocity.y = -2f;
        }

        HandleWallRun();
        HandleLook();
        HandleMovement();
        HandleJump();

        if (isWallRunning) {
            velocity.y = 0f;
        } else {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        if (wallRunCooldown > 0f) {
            wallRunCooldown -= Time.deltaTime;
        }
    }

    void HandleLook() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (isTouchingWall) {
            wallLookYaw += mouseX;
        }

        if (isWallRunning) {
            cameraYawOffset += mouseX;

            if (Mathf.Abs(wallLookYaw) > maxLookAwayAngle) {
                DetachFromWall();
                wallRunCooldown = wallRunReentryDelay;
            }
        } else {
            transform.Rotate(Vector3.up * mouseX);
        }

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -lookXLimit, lookXLimit);

        float targetTilt = 0f;
        if (isWallRunning) {
            targetTilt = wallOnRight ? wallRunTiltAngle : -wallRunTiltAngle;
        }

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, cameraYawOffset, currentTilt);
    }

    void HandleMovement() {
        if (isWallRunning) {
            Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
            if (Vector3.Dot(wallForward, transform.forward) < 0f) {
                wallForward = -wallForward;
            }

            velocity.x = wallForward.x * wallRunSpeed;
            velocity.z = wallForward.z * wallRunSpeed;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = sprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized;
        velocity.x = moveDir.x * speed;
        velocity.z = moveDir.z * speed;
    }

    void HandleJump() {
        if (Input.GetButtonDown("Jump")) {
            if (isWallRunning) {
                lastJumpWallCollider = currentWallCollider;

                velocity = wallNormal * wallJumpSideForce + Vector3.up * wallJumpUpForce;
                StopWallRun();
                wallRunCooldown = wallRunReentryDelay;
            } else if (isGrounded) {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    void HandleWallRun() {
        if (!enableWallRun || isGrounded) {
            EndWallProximity();
            return;
        }

        if (!isTouchingWall) {
            lockedWallRight = transform.right;
        }

        bool hitRight = Physics.Raycast(transform.position, lockedWallRight, out RaycastHit rightHit, wallCheckDistance, wallMask);
        bool hitLeft = Physics.Raycast(transform.position, -lockedWallRight, out RaycastHit leftHit, wallCheckDistance, wallMask);

        if (!hitRight && !hitLeft) {
            EndWallProximity();
            return;
        }

        isTouchingWall = true;
        wallNormal = hitRight ? rightHit.normal : leftHit.normal;
        wallOnRight = hitRight;
        currentWallCollider = hitRight ? rightHit.collider : leftHit.collider;

        float v = Input.GetAxisRaw("Vertical");
        bool withinLookRange = Mathf.Abs(wallLookYaw) <= maxLookAwayAngle;

        bool blockedByJumpCooldown = wallRunCooldown > 0f && currentWallCollider == lastJumpWallCollider;

        if (!blockedByJumpCooldown && v > 0.1f && withinLookRange) {
            if (!isWallRunning) {
                isWallRunning = true;
                wallRunTimer = 0f;
                cameraYawOffset = 0f;
            }

            wallRunTimer += Time.deltaTime;
            if (wallRunTimer >= wallRunMaxDuration) {
                DetachFromWall();
            }
        } else if (isWallRunning) {
            DetachFromWall();
        }
    }

    void EndWallProximity() {
        isTouchingWall = false;
        wallLookYaw = 0f;
        if (isWallRunning) {
            DetachFromWall();
        }
    }

    void DetachFromWall() {
        if (isWallRunning) {
            velocity += wallNormal * 2f;
        }

        StopWallRun();
    }

    void StopWallRun() {
        if (isWallRunning) {
            transform.Rotate(Vector3.up * cameraYawOffset);
            cameraYawOffset = 0f;
        }

        isWallRunning = false;
        wallRunTimer = 0f;
    }

    public void Teleport(Vector3 position) {
        if (controller != null) {
            controller.enabled = false;
        }

        transform.position = position;
        velocity = Vector3.zero;

        if (controller != null) {
            controller.enabled = true;
        }
    }
}