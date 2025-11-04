using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    private float elapsedTime;
    private bool timerRunning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (timerRunning)
            elapsedTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
        // Save last time and update personal best if needed
        PlayerPrefs.SetFloat("LastRunTime", elapsedTime);

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
