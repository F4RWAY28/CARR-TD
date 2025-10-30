using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class infoManager : MonoBehaviour
{
    public static infoManager Instance;

    [Header("Enemy Info Panel")]
    public GameObject enemyPanel;
    public TMP_Text enemyNameText;
    public TMP_Text enemyLivesText;
    public TMP_Text enemySpeedText;
    public TMP_Text enemyMoneyLossText;
    public TMP_Text enemyRewardText;

    [Header("Destructible Info Panel")]
    public GameObject destructiblePanel;
    public TMP_Text destructibleNameText;
    public TMP_Text destructibleMoneyLossText;

    [Header("Tooltip Behavior")]
    public Vector2 offset = new Vector2(15f, -15f);
    public float fadeDuration = 0.2f;
    public float followSpeed = 12f;

    private CanvasGroup enemyCanvasGroup;
    private CanvasGroup destructibleCanvasGroup;
    private RectTransform enemyRect;
    private RectTransform destructibleRect;
    private Camera mainCam;

    private enemyPathing currentEnemy;
    private destructibleInfo currentDestructible;

    private bool isLocked = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCam = Camera.main;

        if (enemyPanel != null)
        {
            enemyRect = enemyPanel.GetComponent<RectTransform>();
            enemyCanvasGroup = enemyPanel.GetComponent<CanvasGroup>() ?? enemyPanel.AddComponent<CanvasGroup>();
            enemyCanvasGroup.alpha = 0;
            enemyPanel.SetActive(false);
        }

        if (destructiblePanel != null)
        {
            destructibleRect = destructiblePanel.GetComponent<RectTransform>();
            destructibleCanvasGroup = destructiblePanel.GetComponent<CanvasGroup>() ?? destructiblePanel.AddComponent<CanvasGroup>();
            destructibleCanvasGroup.alpha = 0;
            destructiblePanel.SetActive(false);
        }
    }

    private void Update()
    {
        HandleHover();
        HandleClick();
        FollowPanels();
    }

    private void HandleHover()
    {
        if (isLocked) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // ✅ Enemy
            enemyPathing enemy = hit.collider.GetComponent<enemyPathing>();
            if (enemy != null)
            {
                ShowEnemyInfo(enemy);
                return;
            }

            // ✅ Destructible
            destructibleInfo destructible = hit.collider.GetComponent<destructibleInfo>();
            if (destructible != null)
            {
                ShowDestructibleInfo(destructible);
                return;
            }
        }

        HideAll();
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            enemyPathing enemy = hit.collider.GetComponent<enemyPathing>();
            destructibleInfo destructible = hit.collider.GetComponent<destructibleInfo>();

            if (enemy != null)
            {
                isLocked = true;
                ShowEnemyInfo(enemy);
                return;
            }

            if (destructible != null)
            {
                isLocked = true;
                ShowDestructibleInfo(destructible);
                return;
            }
        }

        // Clicked elsewhere
        isLocked = false;
        HideAll();
    }

    private void FollowPanels()
    {
        if (enemyPanel != null && enemyPanel.activeSelf)
            enemyRect.position = Vector2.Lerp(enemyRect.position, Input.mousePosition + (Vector3)offset, Time.deltaTime * followSpeed);

        if (destructiblePanel != null && destructiblePanel.activeSelf)
            destructibleRect.position = Vector2.Lerp(destructibleRect.position, Input.mousePosition + (Vector3)offset, Time.deltaTime * followSpeed);
    }

    #region Enemy Info
    public void ShowEnemyInfo(enemyPathing enemy)
    {
        HideDestructibleInfo();

        if (enemyPanel == null || enemy == null) return;

        currentEnemy = enemy;
        enemyPanel.SetActive(true);

        enemyNameText.text = enemy.name;
        enemyLivesText.text = $"Lives: {enemy.lives}/{enemy.maxLives}";
        enemySpeedText.text = $"Speed: {enemy.moveSpeed:F2}";
        enemyMoneyLossText.text = $"Money Loss: ${enemy.moneyLoss}";
        enemyRewardText.text = $"Reward: ${enemy.moneyReward}";

        enemyRect.position = Input.mousePosition + (Vector3)offset;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePanel(enemyCanvasGroup, true));
    }

    public void HideEnemyInfo()
    {
        if (enemyPanel != null && enemyPanel.activeSelf)
            StartCoroutine(FadePanel(enemyCanvasGroup, false));
        currentEnemy = null;
    }
    #endregion

    #region Destructible Info
    public void ShowDestructibleInfo(destructibleInfo info)
    {
        HideEnemyInfo();

        if (destructiblePanel == null || info == null) return;

        currentDestructible = info;
        destructiblePanel.SetActive(true);

        destructibleNameText.text = info.destructibleName;
        destructibleMoneyLossText.text = $"Money Loss: ${info.moneyLoss}";
        destructibleRect.position = Input.mousePosition + (Vector3)offset;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePanel(destructibleCanvasGroup, true));
    }

    public void HideDestructibleInfo()
    {
        if (destructiblePanel != null && destructiblePanel.activeSelf)
            StartCoroutine(FadePanel(destructibleCanvasGroup, false));
        currentDestructible = null;
    }
    #endregion

    #region Utility
    public void HideAll()
    {
        if (isLocked) return;

        HideEnemyInfo();
        HideDestructibleInfo();
    }

    private IEnumerator FadePanel(CanvasGroup group, bool show)
    {
        if (group == null) yield break;

        float start = group.alpha;
        float end = show ? 1f : 0f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        group.alpha = end;
        if (!show)
            group.gameObject.SetActive(false);
    }
    #endregion
}

