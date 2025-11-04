using UnityEngine;

public class timer : MonoBehaviour
{
    public static timer Instance;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isRunning)
            elapsedTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
        PlayerPrefs.SetFloat("LastRunTime", elapsedTime);

        // Save personal best if better
        if (!PlayerPrefs.HasKey("BestTime") || elapsedTime < PlayerPrefs.GetFloat("BestTime"))
            PlayerPrefs.SetFloat("BestTime", elapsedTime);

        PlayerPrefs.Save();
    }

    public float GetElapsedTime() => elapsedTime;

    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds / 10:00}";
    }
}
