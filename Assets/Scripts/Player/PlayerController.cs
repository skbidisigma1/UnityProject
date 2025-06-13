using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float WalkSpeed     = 5f;
    public float JumpSpeed     = 5f;
    public float LookSpeed     = 3f;

    CharacterController cc;
    Transform camT;
    float     pitch, vertVel;

    void Awake()
    {
        cc   = GetComponent<CharacterController>();
        camT = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        HandleMouseLock();
        Look();
        Move();
    }

    // ---------- movement ----------
    void Move()
    {
        Vector3 move = (transform.right * Input.GetAxisRaw("Horizontal") +
                        transform.forward * Input.GetAxisRaw("Vertical")).normalized;

        Vector3 vel = move * WalkSpeed;
        if (cc.isGrounded)
        {
            if (vertVel < 0) vertVel = -2f;
            if (Input.GetButtonDown("Jump")) vertVel = JumpSpeed;
        }
        vertVel += Physics.gravity.y * Time.deltaTime;
        vel.y = vertVel;

        cc.Move(vel * Time.deltaTime);
    }

    // ---------- looking ----------
    void Look()
    {
        float yaw   = Input.GetAxis("Mouse X") * LookSpeed;
        float pitchD= -Input.GetAxis("Mouse Y") * LookSpeed;
        transform.Rotate(Vector3.up * yaw);

        pitch = Mathf.Clamp(pitch + pitchD, -90f, 90f);
        camT.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    // toggle cursor
    void HandleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = !locked;
        }
    }
}