using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using Cinemachine;

public class CameraController : NetworkBehaviour
{

    [SerializeField] private CinemachineTargetGroup _targetGroup;
    public override void OnStartClient()
    {
        //Debug.Log($"CameraController OnStartClient before check owner");

        base.OnStartClient();
        //if (base.IsOwner)
        //{
            Debug.Log($"CameraController OnStartClient is Owner");
            _targetGroup = FindObjectOfType<CinemachineTargetGroup>();
            Camera c = Camera.main;
            CinemachineVirtualCamera vc = GetComponent<CinemachineVirtualCamera>();
            vc.Follow = _targetGroup.transform;
            vc.LookAt = _targetGroup.transform;

        //}
      
    }

    public override void OnStartServer()
    {
        Debug.Log($"CameraController OnStartServer");
        base.OnStartServer();

        _targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        Camera c = Camera.main;
        CinemachineVirtualCamera vc = GetComponent<CinemachineVirtualCamera>();
        vc.Follow = _targetGroup.transform;
        vc.LookAt = _targetGroup.transform;

    }
}