using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float health;

    private PlayerInput _playerInput;
    public event Action onDeath;
    public event Action<float> onHealthChanged;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void TriggerDeath()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShooter>().enabled = false;
        GetComponent<MouseController>().enabled = false;
        //GetComponent<Rigidbody>().isKinematic = true;
        onDeath?.Invoke();

        _playerInput.Player.Restart.performed += context => SceneManager.LoadScene("office");
    }

    public void UpdateHealth()
    {
        onHealthChanged?.Invoke(health);
    }
}
