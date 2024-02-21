using FishNet;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerMenu : MonoBehaviour
{

    [SerializeField] private Button hostBtn, clientBtn;
    // Start is called before the first frame update
    void Start()
    {

        hostBtn.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        });
        clientBtn.onClick.AddListener(() => InstanceFinder.ClientManager.StartConnection());
    }


}
