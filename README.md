# EggScape Examen

## Instrucciones
Usa *WASD* para moverte y *Q* para Atacar. si tu ataque impacta en el cuerpo de tu enemigo conseguiras hacer 1 de daño.

![Captura](images/Captura.Jpg)

Como se puede ver en la imagen, todavia no corregi el SynVar de el nombre de los personajes
Por otro lado, hay que tener en cuenta de que los personajes si se pueden detectar, pero el raycast tiene que colisionar con el prefab correcto.

# Escenas 
##### Lobby
Pantalla de Inicio donde se elige Host o Client y el nombre del jugador
##### MyCustomPlayground
Aqui se cargan los clients usando un prefab de jugador 
cada jugador empieza con 10 de vida

# Prefab Jugador
Componentes de Fishnet: NetworkObject, NetWork Animator, Time Manager
Mis componentes: PlayerNetData, Custom Movement


# Otros Scripts
### CustomMovement
Este script gestiona el movimiento, la animación y el combate de un personaje de red en un juego con FishNet. Se encarga de la sincronización de la posición, la animación y el daño entre clientes y el servidor.

##### Su Estructura de datos
MoveData: Almacena la dirección del movimiento para la replicación en red.
ReconcileData: Almacena la posición del personaje para la reconciliación entre clientes y el servidor.

##### Variables 


**newDirection**: Almacena la dirección del movimiento del jugador.
**_playerNetData**: Referencia al componente PlayerNetData del personaje.
**speed**: Velocidad de movimiento del personaje.
**velocityY**: Velocidad vertical del personaje.
**_anim**: Componente Animator del personaje.
**_networkAnim**: Componente NetworkAnimator del personaje.
**_characterController**: Componente CharacterController del personaje.
**_fist1, _fist2**: Transforms de los puños del personaje.
**_punchDistance**: Distancia máxima del golpe.
**_punchLayerMask**: Máscara de capas para detectar colisiones con el golpe.
**movementInput**: Vector3 que almacena la entrada de movimiento del jugador.
**cooldownPunch**: Tiempo de espera entre golpes.
**lastPunch**: Tiempo del último golpe realizado.
**_hitpointsUI**: Referencia al TextMeshProUGUI que muestra los puntos de vida.
**_nameUI**: Referencia al TextMeshPro que muestra el nombre del jugador.

##### Métodos

**Awake()**: Inicializa variables y componentes.
**UpdateHitPoints() (ObserversRpc)**: Actualiza el texto de los puntos de vida en todos los clientes.
**UpdateNameUI (ObserversRpc)**: Actualiza el texto del nombre del jugador en todos los clientes.
**Update()**: Ejecutado en el cliente propietario, gestiona el movimiento, la animación y el ataque.
**OnControllerColliderHit()**: Detecta colisiones del CharacterController.
**MovePlayer()**: Calcula y aplica el movimiento del personaje.
**HandleAnimation()**: Activa o desactiva la animación de movimiento según la entrada.
**Attack()**: Gestiona el ataque del personaje.
**OnRaycast (ServerRpc)**: Se llama en el servidor al golpear, realiza un Raycast para detectar enemigos.
**TakeHit()**: Reduce los puntos de vida del personaje al recibir un golpe.
**CanPunch()**: Comprueba si el personaje puede realizar un nuevo golpe.
**RpcPlayerHit (TargetRpc)**: Se ejecuta en todos los clientes cuando el Raycast del servidor colisiona con un jugador.
**OnStartNetwork()**: Suscribe el evento OnTick al iniciar la red.
**OnStopNetwork()**: Desuscribe el evento OnTick al detener la red.
**TimeManager_OnTick()**: Se llama en el cliente propietario cada tick de red, construye y envía los datos de movimiento.
**BuildActions()**: Construye los datos de movimiento (MoveData) a partir de la dirección de movimiento.
**Move (Replicate)**: Mueve el personaje en base a los datos de movimiento recibidos.
**Reconcile (Reconcile)**: Corrige la posición del personaje en caso de discrepancias entre clientes y el servidor.

*Los métodos ObserversRpc se ejecutan en todos los clientes, mientras que TargetRpc se ejecuta solo en el cliente objetivo.
El script gestiona el movimiento del personaje en el cliente propietario y lo replica a otros clientes mediante Replicate.
La reconciliación de la posición se realiza mediante Reconcile para corregir posibles desincronizaciones.*

### Game Manager 
Es NetworkObject
Almacena datos de los jugadores en una lista sincronizada (_players).
Actualiza un grupo de objetivos Cinemachine con los jugadores conectados.
##### Clases y Variables

_players (SyncList<PlayerNetData>): Sincroniza datos de los jugadores a través de la red.
_targetGroup (CinemachineTargetGroup): Gestiona los objetivos de la cámara para los jugadores.
##### Métodos
UpdateCameraGroup(PlayerNetData playerRefData, bool isAdding):
Gestiona los miembros del grupo de objetivos de Cinemachine en función de la conexión/desconexión del jugador.




