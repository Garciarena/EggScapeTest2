using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    private float velocityY = 0;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Animator _anim;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private Transform _fist1; //RayCastEmiters
    [SerializeField] private Transform _fist2;
    [SerializeField] private float _punchDistance;

    [SerializeField] private LayerMask _punchLayerMask;

    //calculosRotacion
    [SerializeField] private Vector3 movementInput;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //logica Animacion
             Debug.Log($"{gameObject.name} Punch!");
            _anim.SetBool("isPunching", true);

            //logica Raycast
            Ray ray = new Ray(_fist1.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _punchDistance, _punchLayerMask))
            {
                Debug.Log($"{hit.transform.gameObject.name} was hit");
            }


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

}
