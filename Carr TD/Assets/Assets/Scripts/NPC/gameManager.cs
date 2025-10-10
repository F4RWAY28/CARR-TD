using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("Player Stats")]
    public int money = 1000;

    [Header("UI")]
    public TMP_Text moneyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public void LoseMoney(int amount)
    {
        money -= amount;
        if (money < 0) money = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyText.text = "Money: " + money;
    }
}
