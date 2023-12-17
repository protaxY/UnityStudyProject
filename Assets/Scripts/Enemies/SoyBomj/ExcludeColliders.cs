using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcludeColliders : MonoBehaviour
{
    [SerializeField] private List<Collider> collidersToIgnore;
    
    void Start()
    {
        foreach (Collider collider in collidersToIgnore)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collider);
        }
    }
}
