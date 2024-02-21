using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerNetData : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar] public string userName;
    [SyncVar] public bool isReady;
    [SyncVar] public int hitpoints;
    [SyncVar] public int customId;

    
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.Instance._players.Add(this);
        GameManager.Instance.UpdateCameraGroup(this, true);
    }

    public override void OnStartClient()
    {
        //if (!IsOwner) return;
        base.OnStartClient();
        GameManager.Instance._players.Add(this);
        GameManager.Instance.UpdateCameraGroup(this, true);
    }


    public override void OnStopServer()
    {
        base.OnStopServer();
        GameManager.Instance._players.Remove(this);
        GameManager.Instance.UpdateCameraGroup(this, false);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        GameManager.Instance._players.Remove(this);
        GameManager.Instance.UpdateCameraGroup(this, false);
    }

    public void Update()
    {

    }




}
