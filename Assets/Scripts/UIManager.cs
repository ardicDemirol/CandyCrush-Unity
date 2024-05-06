using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject LoginPanel;
    public GameObject RegisterPanel;
 
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null) Destroy(this);
    }
    public void LoginScreen()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    public void RegisterScreen()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

   
}
