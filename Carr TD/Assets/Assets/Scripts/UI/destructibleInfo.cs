using UnityEngine;

public class destructibleInfo : MonoBehaviour
{
    [Header("Destructible Info")]
    public string destructibleName = "Destructible";
    public int moneyLoss = 5;

    private bool isMouseOver = false;

    private void OnMouseEnter()
    {
        if (infoManager.Instance != null)
        {
            isMouseOver = true;
            infoManager.Instance.ShowDestructibleInfo(this);
        }
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        if (infoManager.Instance != null)
            infoManager.Instance.HideDestructibleInfo();
    }

    private void OnDisable()
    {
        if (infoManager.Instance != null)
            infoManager.Instance.HideDestructibleInfo();
    }
}
