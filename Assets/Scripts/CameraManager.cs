using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class CameraManager : Singleton<CameraManager>
{
   public CinemachineVirtualCamera Cam;

    private void Start()
    {
        Cam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void LerpFOV(float LerpTime, float FOV)
    {
        
    }
}
