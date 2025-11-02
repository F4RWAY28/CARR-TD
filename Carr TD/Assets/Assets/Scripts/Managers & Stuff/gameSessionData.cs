using UnityEngine;

public class gameSessionData : MonoBehaviour
{
    public static gameSessionData Instance;

    [HideInInspector] public int wavesReached = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
