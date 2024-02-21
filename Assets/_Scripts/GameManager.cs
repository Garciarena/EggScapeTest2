using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }

    [SyncObject]
    public readonly SyncList<PlayerNetData> _players = new SyncList<PlayerNetData>();
    [SyncVar]
    public bool canStart;

    public CinemachineTargetGroup _targetGroup;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCameraGroup(PlayerNetData playerRefData, bool isAdding) //se llama cada vez que un cliente inicia conexion
    {
        Debug.Log($"UpdateCameraGroup with {playerRefData.name} and is adding :{isAdding}");
        //_targetGroup = FindObjectOfType<CinemachineTargetGroup>();

        //se añade miembro al groupCamera
        if (isAdding)
        {
            _targetGroup.AddMember(playerRefData.GetComponent<Transform>(), 1, 4);
        }
        else
        {
            _targetGroup.RemoveMember(playerRefData.GetComponent<Transform>());
        }

    }
    private void Update()
    {
        if (!IsServer) return;
        canStart = _players.All(_players => _players.isReady);
       // Debug.Log($"Can Start = {canStart}");
        Debug.Log(_players.Count);
    }

}
