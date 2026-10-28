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

    public void SetSpeedCamera(float x, float y) 
    {
        foreach (var cam in _cinemachines)
        {
            var input = cam.GetComponent<CinemachineInputAxisController>();

            foreach (var controller in input.Controllers)
            {
                
                if (controller.Name.Contains("Look X"))
                {
                    controller.Input.Gain = x; 
                }
                else if (controller.Name.Contains("Look Y"))
                {
                    controller.Input.Gain = -y;
                }
            }
        }
    }
}
