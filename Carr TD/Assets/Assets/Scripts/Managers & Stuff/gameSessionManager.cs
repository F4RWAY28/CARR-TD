using UnityEngine;

public class gameSessionManager : MonoBehaviour
{
    public static gameSessionManager Instance;

    [HideInInspector] public int wavesReached = 0;
    [HideInInspector] public bool playerWon = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
