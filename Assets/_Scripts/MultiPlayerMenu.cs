using FishNet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerMenu : MonoBehaviour
{

    [SerializeField] private Button hostBtn, clientBtn;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string _playerName;
    // Start is called before the first frame update
    void Start()
    {

        hostBtn.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        });
        clientBtn.onClick.AddListener(() => InstanceFinder.ClientManager.StartConnection());

        inputField.onSubmit.AddListener(OnSubmit);
        

    }

    public void OnSubmit(string arg)
    {
        // Obtener el texto del TMP_InputField
        //_playerName = inputField.text;

        // Actualizar el texto del nombre del jugador
        Debug.Log("OnSubmit:" + inputField.text);
        // Guardar el nombre del jugador (código específico del juego)

        LobbyPlayerData.Instance._playerName = inputField.text;
        // ...
    }

   

    
}
