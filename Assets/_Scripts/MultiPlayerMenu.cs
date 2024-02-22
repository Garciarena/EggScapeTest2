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

    //CanvasGroups
    [SerializeField]  CanvasGroup _instructionsCG;
    [SerializeField]  CanvasGroup _textInputCG;
    private bool submited=false;

    //transform
    [SerializeField] Transform sword;

    void Start()
    {

        hostBtn.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        });
        clientBtn.onClick.AddListener(() => InstanceFinder.ClientManager.StartConnection());

        inputField.onSubmit.AddListener(OnSubmit);
        hostBtn.interactable = false;
        clientBtn.interactable = false;

        LeanTween.rotateY(sword.gameObject, -360, 2).setLoopClamp();
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
        hostBtn.interactable = true;
        clientBtn.interactable = true;

        //Cuando hacemos Submit escondemos el text input
        HideTextInput();
        submited = true;
    }


    public void HideInstructions()
    {
        LeanTween.alphaCanvas(_instructionsCG, 0, .3f);
    }

    public void ShowInstructions()
    {
        if(!submited)
        LeanTween.alphaCanvas(_instructionsCG, 1, .3f);
    }

    public void HideTextInput()
    {
        HideInstructions();
        LeanTween.moveY(inputField.gameObject, -100, .5f).setEaseInOutBack();
        LeanTween.alphaCanvas(_instructionsCG, 0, .5f);
        
    }

}
