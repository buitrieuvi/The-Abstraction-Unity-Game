using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _cinemachines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cinemachines = GetComponentsInChildren<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInputIsActiveCameras(string cameraName, bool isActive)
    {
        if (cameraName == "") 
        {
            foreach (var cam in _cinemachines)
            {
                cam.GetComponent<CinemachineInputAxisController>().enabled = isActive;
            }
            return;
        }

        foreach (var cam in _cinemachines)
        {
            if (cam.tag == cameraName)
            {
                cam.GetComponent<CinemachineInputAxisController>().enabled = isActive;
                return;
            }
        }
    }
}
