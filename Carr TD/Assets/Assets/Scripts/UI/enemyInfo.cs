using UnityEngine;

[RequireComponent(typeof(enemyPathing))]
public class enemyInfo : MonoBehaviour
{
    private enemyPathing pathing;
    private bool isMouseOver = false;

    private void Awake()
    {
        pathing = GetComponent<enemyPathing>();
    }

    private void OnMouseEnter()
    {
        if (infoManager.Instance != null && pathing != null)
        {
            isMouseOver = true;
            infoManager.Instance.ShowEnemyInfo(pathing);
        }
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        if (infoManager.Instance != null)
            infoManager.Instance.HideEnemyInfo();
    }

    private void OnDisable()
    {
        if (infoManager.Instance != null)
            infoManager.Instance.HideEnemyInfo();
    }

    private void Update()
    {
        // Close tooltip if enemy gets destroyed mid-hover
        if (isMouseOver && (pathing == null || pathing.Equals(null)))
        {
            isMouseOver = false;
            if (infoManager.Instance != null)
                infoManager.Instance.HideEnemyInfo();
        }
    }
}
