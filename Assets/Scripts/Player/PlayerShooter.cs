using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] public int _planesCount;
    [SerializeField] private float _throwTimer;
    [SerializeField] private float _throwFrowardImpulse;
    [SerializeField] private GameObject _planePrefab;
    [SerializeField] private Vector3 _handOffset;
    [SerializeField] private float _collectDistance;
    [SerializeField] private LayerMask _whatIsPaper;

    private Camera _camera;
    private PlayerInput _playerInput;

    private bool _isSpawned = false;
    private bool _isThrowing = false;
    private GameObject _currentPlane;

    public event Action<int> onPlanesCountChanged;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        if (_planesCount > 0)
        {
            SpawnPlane();
            _isSpawned = true;
        }
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

    private void SpawnPlane()
    {
        _currentPlane = Instantiate(_planePrefab, _camera.transform.TransformPoint(_handOffset), _camera.transform.rotation, _camera.transform);
        _currentPlane.transform.GetComponentInChildren<Collider>().enabled = false;
        //int whatIsInHandIndex = Mathf.RoundToInt(Mathf.Log(_whatIsInHand.value, 2));
        //_currentPlane.layer = whatIsInHandIndex;
    }
    private IEnumerator ThrowPlaneAsync()
    {
        if (!_isThrowing && _isSpawned)
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

            //_isSpawned = false;
            _isThrowing = false;
            _isSpawned = false;
            _planesCount--;
            onPlanesCountChanged?.Invoke(_planesCount);

            if (_planesCount > 0)
            {
                SpawnPlane();
                _isSpawned = true;
            }
            
        }
        
    }

    private void Collect()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward * _collectDistance, out hit))
        {
            if (1 << hit.collider.transform.gameObject.layer == _whatIsPaper.value)
            {
                _planesCount += hit.collider.transform.GetComponent<PaperCollectable>().paperCount;
                onPlanesCountChanged?.Invoke(_planesCount);
                Destroy(hit.collider.gameObject);

                if (_planesCount > 0 && !_isSpawned)
                {
                    SpawnPlane();
                    _isSpawned = true;
                }
            }
        }
    }

}
