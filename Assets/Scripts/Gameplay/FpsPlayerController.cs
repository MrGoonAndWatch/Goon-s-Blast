using Photon.Pun;
using UnityEngine;

public class FpsPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _cameraContainer;

    [SerializeField] private float _horizontalMouseSensitivity, _verticalMouseSensitivity, _sprintSpeed, _walkSpeed, _jumpForce, _smoothTime;

    private float _verticalLookRotation;
    private bool _grounded;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;

    private Rigidbody _rigidbody;
    private PhotonView _photonView;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rigidbody);
        }
    }
    
    private void Update()
    {
        if (!_photonView.IsMine)
            return;

        HandleLook();
        HandleMove();
        HandleJump();
    }

    private void HandleLook()
    {
        // TODO: Use modern control input processing to allow Joystick reading as well here!
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _horizontalMouseSensitivity);
        _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * _verticalMouseSensitivity;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);
        _cameraContainer.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
    }

    private void HandleMove()
    {
        // TODO: Use newer input processing for these keys (Horizontal, Vertical, LeftShift)
        var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        var moveSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _walkSpeed;
        _moveAmount = Vector3.SmoothDamp(_moveAmount, moveDir * moveSpeed, ref _smoothMoveVelocity, _smoothTime);
    }

    private void HandleJump()
    {
        // TODO: Use newer input processing here.
        if (!_grounded || !Input.GetKeyDown(KeyCode.Space))
            return;
        
        _rigidbody.AddForce(transform.up * _jumpForce);
    }

    public void SetGrounded(bool grounded)
    {
        _grounded = grounded;
    }

    // Put in fixed update so move speed isn't affected by FPS.
    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
            return;
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }
}
