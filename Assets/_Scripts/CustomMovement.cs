using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public struct MoveData : IReplicateData
{
    public Vector3 Direction;

    /* Everything below this is required for
    * the interface. You do not need to implement
    * Dispose, it is there if you want to clean up anything
    * that may allocate when this structure is discarded. */
    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public struct ReconcileData : IReconcileData
{
    public Vector3 Position;

    /* Everything below this is required for
    
    the interface. You do not need to implement
    Dispose, it is there if you want to clean up anything
    that may allocate when this structure is discarded. */
    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public class CustomMovement : NetworkBehaviour
{
    //NetworkData
    public Vector3 newDirection;

    private PlayerNetData _playerNetData;


    [SerializeField] private float speed;
    private float velocityY = 0;
    [SerializeField] private Animator _anim;
    [SerializeField] private NetworkAnimator _networkAnim;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private Transform _fist1; //RayCastEmiters
    [SerializeField] private Transform _fist2;
    [SerializeField] private float _punchDistance;

    [SerializeField] private LayerMask _punchLayerMask;

    //calculosRotacion
    [SerializeField] private Vector3 movementInput;

    [SerializeField] private float cooldownPunch = 0.5f; //medio segundo de cooldown para golpear
    private float lastPunch;

    //UI
    [SerializeField]
    private TMPro.TextMeshProUGUI _hitpointsUI;
    [SerializeField]
    private TMPro.TextMeshPro _nameUI;

    private void Awake()
    {
        _networkAnim = GetComponent<NetworkAnimator>();
        _hitpointsUI = GameObject.Find("txtHitpoints").GetComponent<TextMeshProUGUI>();
        _playerNetData = GetComponent<PlayerNetData>();

        _hitpointsUI.text = _playerNetData.hitpoints.ToString();

        _nameUI.text = LobbyPlayerData.Instance._playerName;

    }

    [ObserversRpc]
    private void UpdateHitPoints()
    {
        if (!IsOwner) return;
        _hitpointsUI.text = _playerNetData.hitpoints.ToString();
        //_hitpointsUI.GetComponent<AnimateHitPoints>().AnimHitPoints();


    }

    [ObserversRpc]
    public void UpdateNameUI()
    {
        Debug.Log("UpdateNameUI");
        if (!IsOwner) return;
        Debug.Log("UpdateNameUI is owner");
        _nameUI.text = _playerNetData.userName;

    }


    void Update()
    {
        if (!IsOwner) return; //Solo se mueve si es dueño de su client
        MovePlayer();
        HandleAnimation();
        Attack();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit != null)
        {
            // Debug.Log($" collision = {hit.gameObject.name}");
        }
    }

    void MovePlayer()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


           newDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;


       



    }

    
    void HandleAnimation()
    {
        // move anim
        if (movementInput != Vector3.zero)
        {
            // Debug.Log($"is moving: {true}");
            _anim.SetBool("isMoving", true);
        }
        else
        {
            //   Debug.Log($"is moving: {false}");
            _anim.SetBool("isMoving", false);
        }
    }

    void Attack() //metodo para golpear
    {
        
        if (Input.GetKeyDown(KeyCode.Q) && CanPunch())
        {
            //logica Animacion
            _anim.SetBool("isPunching2", true); 
            OnRaycast();


        }
        else
        {
            _anim.SetBool("isPunching2", false);
        }
    }


    [ServerRpc]
    public void OnRaycast() //se llama al golpear
    {

        Ray ray = new Ray(_fist2.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _punchDistance, _punchLayerMask))
        {
            Debug.Log($"{hit.transform.gameObject.name} was hit");
            hit.transform.gameObject.GetComponent<CustomMovement>().TakeHit();

           
        }

    }

   
    private void TakeHit() //metodo para sustraer vida del personaje
    {
        Debug.Log(gameObject.name + "was Hit! in TakeHit");
        Debug.Log(GetComponent<PlayerNetData>().hitpoints);
        _playerNetData.hitpoints = _playerNetData.hitpoints - 1;
        UpdateHitPoints();
    } 

    private bool CanPunch() 
    {
        if(Time.realtimeSinceStartup > lastPunch + cooldownPunch) 
        {
            lastPunch= Time.realtimeSinceStartup;
            return true;
        }
        else
        {
            return false;
        }
    }

    [TargetRpc]
    public void RpcPlayerHit(NetworkConnection connection)
    {
        // Se ejecuta en todos los clientes cuando el Raycast del servidor colisiona con un jugador
        // Recibir la ID del jugador y mostrar un mensaje
        Debug.Log("Hit en el jugador " + connection.ClientId);
    }

    //NetworkTickMethods

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        base.TimeManager.OnTick += TimeManager_OnTick;
        //suscribimos el tick cuando el objeto entra en la red
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if (base.TimeManager != null)
            base.TimeManager.OnTick -= TimeManager_OnTick;
        //desuscribimos el tick cuando el objeto entra en la red
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            BuildActions(out MoveData md);
            Move(md, false);
        }

        //if (base.IsServer)
        //{
        //    Move(default, true);
        //    ReconcileData rd = new ReconcileData()
        //    {
        //        Position = transform.position
        //    };
        //    Reconcile(rd, true);
        //}
    }

    private void BuildActions(out MoveData moveData)
    {
        moveData = default;
        moveData.Direction = newDirection;
        //Unset queued values.
        newDirection = Vector3.zero;
    }

    //replicate // Reconcile

    [Replicate] //se replica en otros clientes
    private void Move(MoveData moveData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {

       // Calculate movement direction in world space

        // Rotate character to face movement direction
        if (moveData.Direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveData.Direction);
            // Move the character
            _characterController.Move(moveData.Direction * speed ); //time.deltatime
        }
    }

    [Reconcile] //reconcilia discrepancias entre clientes y servidores
    private void Reconcile(ReconcileData recData, bool asServer, Channel channel = Channel.Unreliable)
    {
        //Reset the client to the received position. It's okay to do this
        //even if there is no de-synchronization.
        transform.position = recData.Position;
    }

}
