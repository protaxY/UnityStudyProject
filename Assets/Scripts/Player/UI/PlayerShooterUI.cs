using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShooterUI : MonoBehaviour
{
    [SerializeField] private PlayerShooter _playerShooter;
    [SerializeField] private TMP_Text _textTMP;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _textTMP.text = _playerShooter._planesCount.ToString();
    }

}
