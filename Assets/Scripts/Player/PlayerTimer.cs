using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _bestTimeText;
    [SerializeField] private TMP_Text _bomjesLeft;
    [SerializeField] private TMP_Text _pressRTorestart;
    [SerializeField] private TMP_Text _level—leared;
    [SerializeField] private TMP_Text _newRecord;

    [SerializeField] private List<SoyBomjAi> _enemies;
    
    private Stopwatch _timer;
    private static TimeSpan _bestTime;

    private int _enemiesLeftCount;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.QuitToMenu.performed += context => QuitToMenu();
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
        foreach (SoyBomjAi enemy in _enemies)
        {
            enemy.onDeath += UpadeCount;
        }

        _enemiesLeftCount = _enemies.Count;

        _bestTime = TimeSpan.FromSeconds(PlayerPrefs.GetFloat("bestTime", 0f));
        
        if (_bestTime != new TimeSpan())
        {
            string bestTime = String.Format("{0:00}:{1:00}:{2:00}",
                     _bestTime.Minutes, _bestTime.Seconds, _bestTime.Milliseconds / 10);
            _bestTimeText.text = String.Concat("Best time: ", bestTime);
        }

        _bomjesLeft.text = String.Concat("Bomjes left: ", _enemiesLeftCount.ToString());

        _timer = new Stopwatch();
        _timer.Start();
    }

    private void OnDestroy()
    {
        foreach (SoyBomjAi enemy in _enemies)
        {
            enemy.onDeath -= UpadeCount;
        }
    }

    private void Update()
    {
        // Format and display the TimeSpan value.
        TimeSpan currentTime = _timer.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
            currentTime.Minutes, currentTime.Seconds, currentTime.Milliseconds / 10);
        _time.text = String.Concat("Time: ", elapsedTime);
    }

    private void UpadeCount()
    {
        --_enemiesLeftCount;

        _bomjesLeft.text = String.Concat("Bomjes left: ", _enemiesLeftCount.ToString());

        if (_enemiesLeftCount == 0)
        {
            TimeSpan time = _timer.Elapsed;
            _timer.Stop();
            if (_bestTime == new TimeSpan() || time < _bestTime)
            {
                _bestTime = time;
                PlayerPrefs.SetFloat("bestTime", _bestTime.Seconds);
                string bestTime = String.Format("{0:00}:{1:00}:{2:00}",
                     _bestTime.Minutes, _bestTime.Seconds, _bestTime.Milliseconds / 10);
                _bestTimeText.text = String.Concat("Best time: ", bestTime);

                _newRecord.enabled = true;
            }

            _level—leared.enabled = true;
            _pressRTorestart.enabled = true;

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerShooter>().enabled = false;
            GetComponent<MouseController>().enabled = false;

            _playerInput.Player.Restart.performed += context => SceneManager.LoadScene("office");
        }
    }

    private void QuitToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
