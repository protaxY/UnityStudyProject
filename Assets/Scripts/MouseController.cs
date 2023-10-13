using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{

    [SerializeField] float horizontalCameraSensetivity;
    [SerializeField] float verticalCameraSensetivity;

    private PlayerInput _playerInput;
    private Camera _camera;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.SightMove.performed += context => SightMove(context);
    }
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camera = transform.Find("PlayerCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SightMove(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        
        transform.Rotate(Vector3.up, mouseDelta.x * horizontalCameraSensetivity);


        float sightPitch = _camera.transform.localRotation.eulerAngles.x;
        if (sightPitch > 180)
            sightPitch -= 360;
        sightPitch -= mouseDelta.y * verticalCameraSensetivity;
        sightPitch = Mathf.Clamp(sightPitch, -90, 90);
        _camera.transform.localRotation = Quaternion.Euler(sightPitch, 0f, 0f);
    }
}
