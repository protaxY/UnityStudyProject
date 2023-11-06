using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PaperCollectable : MonoBehaviour
{
    private Transform _player;
    [SerializeField] public int paperCount = 1;
    [SerializeField] private float _outlineDrawDistance;

    private float squareDrawDistqance;
    private Outline _outline;

    void Start()
    {
        _player = GameObject.Find("Player").transform;
        squareDrawDistqance = Mathf.Pow(_outlineDrawDistance, 2);
        _outline = GetComponent<Outline>();
    }

    void Update()
    {
        if ((_player.transform.position - transform.position).sqrMagnitude < squareDrawDistqance){
            _outline.enabled = true;
        }
        else
        {
            _outline.enabled = false;
        }
    }
}
