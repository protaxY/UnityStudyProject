using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private int _planesCount;
    [SerializeField] private float _throwTimer;
    [SerializeField] private float _throwFrowardImpulse;
    [SerializeField] private GameObject _planePrefab;
    [SerializeField] private Vector3 _handOffset;
    [SerializeField] private float _collectDistance;
    [SerializeField] private LayerMask _whatIsPaper;
    //[SerializeField] private LayerMask _whatIsInHand;
    //[SerializeField] private LayerMask _whatIsWeapon;

    private Camera _camera;
    private PlayerInput _playerInput;

    private bool _isSpawned = false;
    private bool _isThrowing = false;
    private GameObject _currentPlane;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.ThrowPlane.performed += context => StartCoroutine(ThrowPlaneAsync());
        _playerInput.Player.Collect.performed += context => Collect();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSpawned)
        {
            SpawnPlane();
            _isSpawned = true;
        }
    }

    private void SpawnPlane()
    {
        _currentPlane = Instantiate(_planePrefab, _camera.transform.TransformPoint(_handOffset), _camera.transform.rotation, _camera.transform);
        _currentPlane.transform.GetComponentInChildren<Collider>().enabled = false;
        //int whatIsInHandIndex = Mathf.RoundToInt(Mathf.Log(_whatIsInHand.value, 2));
        //_currentPlane.layer = whatIsInHandIndex;
    }
    private IEnumerator ThrowPlaneAsync()
    {
        if (!_isThrowing)
        {
            Rigidbody planeRb = _currentPlane.transform.GetComponent<Rigidbody>();

            //int whatIsWeaponIndex = Mathf.RoundToInt(Mathf.Log(_whatIsWeapon.value, 2));
            //_currentPlane.layer = whatIsWeaponIndex;
            _currentPlane.transform.GetComponentInChildren<Collider>().enabled = true;
            _currentPlane.transform.SetParent(null, true);
            planeRb.isKinematic = false;
            planeRb.AddRelativeForce(new Vector3(0f, 0f, _throwFrowardImpulse), ForceMode.Acceleration);

            _isThrowing = true;

            yield return new WaitForSeconds(_throwTimer);

            _isSpawned = false;
            _isThrowing = false;
        }
        
    }

    void Collect()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward * _collectDistance, out hit))
        {
            if (1 << hit.collider.transform.gameObject.layer == _whatIsPaper.value)
            {
                _planesCount += hit.collider.transform.GetComponent<PaperCollectable>().paperCount;
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
