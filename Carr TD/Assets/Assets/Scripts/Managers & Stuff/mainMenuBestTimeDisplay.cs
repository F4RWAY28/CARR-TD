using UnityEngine;
using TMPro;

public class mainMenuBestTimeDisplay : MonoBehaviour
{
    public TMP_Text bestTimeText;

    private void Start()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        if (bestTime <= 0f)
            bestTimeText.text = "Personal Best: --:--.--";
        else
            bestTimeText.text = "Personal Best: " + TimerManager.FormatTime(bestTime);
    }

    public void ResetBestTime()
    {
        PlayerPrefs.DeleteKey("BestTime");
        bestTimeText.text = "Personal Best: --:--.--";
    }
}
