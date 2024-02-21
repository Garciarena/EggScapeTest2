using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomMovement : NetworkBehaviour
{
    //NetworkData
    private PlayerNetData _playerNetData;

    [SerializeField] private float speed;
    private float velocityY = 0;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float gravityValue = -9.81f;
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
    private TMPro.TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _networkAnim = GetComponent<NetworkAnimator>();
        _textMeshPro = GameObject.Find("txtHitpoints").GetComponent<TextMeshProUGUI>();
        _playerNetData = GetComponent<PlayerNetData>();

        _textMeshPro.text = _playerNetData.hitpoints.ToString();

    }

    [ObserversRpc]
    private void UpdateHitPoints()
    {
        if (!IsOwner) return;
        _textMeshPro.text = _playerNetData.hitpoints.ToString();
    }

    [ServerRpc]
    private void UpdateUI()
    {
        UpdateHitPoints();
    }

    void Update()
    {
        if (!IsOwner) return; //Solo se mueve si es dueño de su client



        Move2();
        HandleAnimation();
        Punch1();
        Punch2();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit != null)
        {
            // Debug.Log($" collision = {hit.gameObject.name}");
        }
    }

    void Move2()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction in world space
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;


        // Rotate character to face movement direction
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            // Move the character
            _characterController.Move(moveDirection * speed * Time.deltaTime);
        }




    }

    void Move()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 Direction = movementInput.normalized;



        bool groundedPlayer = _characterController.isGrounded;
        if (groundedPlayer && velocityY < 0)
        {
            velocityY = 0f;
        }

        transform.Rotate(transform.up, movementInput.x * rotSpeed * Time.deltaTime);
        _characterController.Move(movementInput.z * transform.forward * speed * Time.deltaTime);




        //gravedad
        velocityY += gravityValue * Time.deltaTime;
        _characterController.Move(new Vector3(0, velocityY, 0) * Time.deltaTime);
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
    void HandleRotation()
    {
        //a donde tendria que apuntar el personaje
        Vector3 positionTooLookAt;
        positionTooLookAt.x = movementInput.x;
        positionTooLookAt.y = 0.0f;
        positionTooLookAt.z = movementInput.z;

        //la rotacion actual 
        Quaternion currentRotation = transform.rotation;
        //rotacion a donde tiene que apuntar el player en Quaternion
        Quaternion targetRotation = Quaternion.LookRotation(positionTooLookAt);
        Quaternion.Slerp(currentRotation, targetRotation, rotSpeed * Time.deltaTime);

    }
    void Punch1() //metodo para golpear
    {
        
        if (Input.GetKeyDown(KeyCode.Q) && CanPunch())
        {
            //logica Animacion
            Debug.Log($"{gameObject.name} Punch!");
            _anim.SetBool("isPunching", true);

            ////logica Raycast
            //Ray ray = new Ray(_fist1.position, transform.forward);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, _punchDistance, _punchLayerMask))
            //{
            //    Debug.Log($"{hit.transform.gameObject.name} was hit");
            //}
            OnRaycast();


        }
        else
        {
            _anim.SetBool("isPunching", false);
        }
    }

    void Punch2() //metodo para golpear
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //logica Animacion
            Debug.Log($"{gameObject.name} Punch2!");
            _anim.SetBool("isPunching2", true);

            //logica Raycast
            Ray ray = new Ray(_fist2.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _punchDistance, _punchLayerMask))
            {
                Debug.Log($"{hit.transform.gameObject.name} was hit");

            }


        }
        else
        {
            _anim.SetBool("isPunching2", false);
        }
    }

    [ServerRpc]
    public void OnRaycast()
    {

        Ray ray = new Ray(_fist1.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _punchDistance, _punchLayerMask))
        {
            Debug.Log($"{hit.transform.gameObject.name} was hit");
            hit.transform.gameObject.GetComponent<CustomMovement>().TakeHit();

            //RpcPlayerHit(hit.transform.gameObject.GetComponent<NetworkObject>().LocalConnection);
            // RpcTargetAllClients.InvokeRpc("OnPlayerHit", hit.collider.gameObject.GetComponent<NetworkIdentity>().netId);
        }

        //// Se ejecuta en el servidor
        //// Realizar un Raycast y obtener información sobre la colisión
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
        //{
        //    // Si se produce una colisión, comprobar si el objeto es un jugador
        //    if (hit.collider.gameObject.GetComponent<PlayerNetData>() != null)
        //    {
        //        // Invocar método para calcular la colisión
        //        RpcTargetAllClients.InvokeRpc("OnPlayerCollision", hit.collider.gameObject.GetComponent<NetworkIdentity>().netId);
        //    }
        //}


    }

    private void TakeHit()
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
}
