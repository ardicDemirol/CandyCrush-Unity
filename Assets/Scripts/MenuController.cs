using TMPro;
using UnityEngine;

internal class MenuController : MonoBehaviour
{

    public static MenuController Instance;

    public TextMeshProUGUI[] MaxScores;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

}
