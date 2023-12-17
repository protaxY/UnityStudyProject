using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerShooterUI : MonoBehaviour
{
    [SerializeField] private PlayerShooter _playerShooter;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private TMP_Text _planesCount;
    [SerializeField] private TMP_Text _healthCount;
    [SerializeField] private TMP_Text _gameOver;
    [SerializeField] private TMP_Text _PressRTorestart;
    [SerializeField] private TMP_Text _PressEscToQuit;

    // Start is called before the first frame update
    void Start()
    {
        _playerShooter.onPlanesCountChanged += UpdatePlanesCount;
        _playerHealth.onDeath += ShowDeathScreen;
        _playerHealth.onHealthChanged += UpdateHealthCount;

        _healthCount.text = Mathf.Floor(_playerHealth.health).ToString();
        _planesCount.text = _playerShooter._planesCount.ToString();
    }

    private void OnDestroy()
    {
        _playerShooter.onPlanesCountChanged -= UpdatePlanesCount;
        _playerHealth.onDeath -= ShowDeathScreen;
        _playerHealth.onHealthChanged -= UpdateHealthCount;
    }

    private void UpdatePlanesCount(int value)
    {
        _planesCount.text = value.ToString();
    }

    private void ShowDeathScreen()
    {
        _gameOver.enabled = true;
        _PressRTorestart.enabled = true;
        _PressEscToQuit.enabled = true;
    }

    private void UpdateHealthCount(float value)
    {
        _healthCount.text = Mathf.Floor(value).ToString();
    }
}
