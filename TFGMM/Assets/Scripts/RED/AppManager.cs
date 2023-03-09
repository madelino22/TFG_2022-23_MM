using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    private HeadlessServerManager _headlessServerManager = null;

    [SerializeField]
    private string _map = "Login";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!_headlessServerManager.IsServer)
        {
            SceneManager.LoadScene(_map);
        }
    }
}