using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookAtPlayer : MonoBehaviour
{

    GameObject player = null;

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    public void lockCameraToPlayer()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;

        GetComponent<CinemachineStateDrivenCamera>().Follow = player.transform;
    }
    
}
