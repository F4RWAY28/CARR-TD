using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class towerSelectionManager : MonoBehaviour
{
    public static towerSelectionManager Instance;

    [Header("UI Panel")]
    public GameObject upgradePanel;
    public TMP_Text towerNameText;
    public Button[] upgradeButtons;
    public Button sellButton;
    public TMP_Text sellButtonText;

    [Header("Upgrade Tooltip")]
    public TMP_Text tooltipText;

    [Header("Animation Settings")]
    public float panelSlideDuration = 0.3f;
    public float buttonFadeDuration = 0.2f;
    public float buttonFadeDelay = 0.05f;

    [Header("Range Visualization")]
    public int rangeSegments = 50;

    private CanvasGroup panelCanvasGroup;
    private RectTransform panelRect;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;

    public towerInteractable CurrentSelection { get; private set; }
    private bool isPanelLocked = false;
    private Coroutine panelCoroutine;

    private LineRenderer rangeRenderer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (upgradePanel != null)
        {
            panelRect = upgradePanel.GetComponent<RectTransform>();
            panelCanvasGroup = upgradePanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null) panelCanvasGroup = upgradePanel.AddComponent<CanvasGroup>();

            hiddenPosition = panelRect.anchoredPosition + new Vector2(-panelRect.rect.width, 0);
            shownPosition = panelRect.anchoredPosition;

            panelRect.anchoredPosition = hiddenPosition;
            panelCanvasGroup.alpha = 0;
        }

        if (tooltipText != null)
            tooltipText.text = "";
    }

    private void Update()
    {
        HandleHoverSelection();
        HandleClickSelection();
    }

    private void HandleHoverSelection()
    {
        if (isPanelLocked) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            towerInteractable tower = hit.collider.GetComponent<towerInteractable>();
            if (tower != null)
            {
                SelectTowerHover(tower);
            }
            else
            {
                HidePanel();
            }
        }
        else
        {
            HidePanel();
        }
    }

    private void HandleClickSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(clickRay, out RaycastHit clickHit))
        {
            towerInteractable tower = clickHit.collider.GetComponent<towerInteractable>();
            if (tower != null)
            {
                SelectTowerClick(tower);
            }
            else
            {
                isPanelLocked = false;
                HidePanel();
            }
        }
        else
        {
            isPanelLocked = false;
            HidePanel();
        }
    }

    public void SelectTowerHover(towerInteractable tower)
    {
        if (isPanelLocked) return;
        CurrentSelection = tower;
        ShowPanel();
        ShowUI();
        ShowRangeVisualization(tower);
    }

    public void SelectTowerClick(towerInteractable tower)
    {
        CurrentSelection = tower;
        ShowPanel();
        ShowUI();
        isPanelLocked = true;
        ShowRangeVisualization(tower);
    }

    public void DeselectIfHover(towerInteractable tower)
    {
        if (!isPanelLocked && CurrentSelection == tower)
        {
            HidePanel();
        }
    }

    private void ShowPanel()
    {
        if (panelCoroutine != null) StopCoroutine(panelCoroutine);
        panelCoroutine = StartCoroutine(AnimatePanel(true));
    }

    private void HidePanel()
    {
        CurrentSelection = null;
        if (panelCoroutine != null) StopCoroutine(panelCoroutine);
        panelCoroutine = StartCoroutine(AnimatePanel(false));
        HideTooltip();
        ClearRangeVisualization();
    }

    #region Range Visualization
    private void ShowRangeVisualization(towerInteractable tower)
    {
        ClearRangeVisualization();

        rangeRenderer = tower.gameObject.AddComponent<LineRenderer>();
        rangeRenderer.positionCount = rangeSegments + 1;
        rangeRenderer.startWidth = 0.05f;
        rangeRenderer.endWidth = 0.05f;
        rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        rangeRenderer.startColor = Color.cyan;
        rangeRenderer.endColor = Color.cyan;

        tower.OnRangeChanged += () => UpdateRangeCircle(tower);

        UpdateRangeCircle(tower);
    }

    private void UpdateRangeCircle(towerInteractable tower)
    {
        if (rangeRenderer == null || tower == null) return;

        float radius = tower.GetRange();
        float angleStep = 360f / rangeSegments;

        for (int i = 0; i <= rangeSegments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            Vector3 pos = tower.transform.position + new Vector3(Mathf.Cos(angle), 0.01f, Mathf.Sin(angle)) * radius;
            rangeRenderer.SetPosition(i, pos);
        }
    }

    private void ClearRangeVisualization()
    {
        if (rangeRenderer != null)
        {
            Destroy(rangeRenderer);
            rangeRenderer = null;
        }

        if (CurrentSelection != null)
            CurrentSelection.OnRangeChanged = null;
    }
    #endregion

    #region UI Handling
    private void ShowUI()
    {
        if (CurrentSelection == null || upgradePanel == null) return;

        if (towerNameText != null)
            towerNameText.text = CurrentSelection.towerName;

        if (sellButton != null && sellButtonText != null)
        {
            sellButtonText.text = $"Sell: {CurrentSelection.GetSellPrice()}$";
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(SellTower);
        }

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (CurrentSelection.GetUpgrades() != null && i < CurrentSelection.GetUpgrades().Length)
            {
                var upgradeOption = CurrentSelection.GetUpgrades()[i];
                upgradeButtons[i].gameObject.SetActive(true);

                TMP_Text btnText = upgradeButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                    btnText.text = $"{upgradeOption.upgradeName} ($ {upgradeOption.cost})";

                int index = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() =>
                {
                    UpgradeTower(index);
                    HideTooltip();
                });

                EventTrigger trigger = upgradeButtons[i].GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = upgradeButtons[i].gameObject.AddComponent<EventTrigger>();

                trigger.triggers.Clear();

                EventTrigger.Entry enterEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                enterEntry.callback.AddListener((data) => ShowTooltip(CurrentSelection.GetUpgrades()[index].description));
                trigger.triggers.Add(enterEntry);

                EventTrigger.Entry exitEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                exitEntry.callback.AddListener((data) => HideTooltip());
                trigger.triggers.Add(exitEntry);
            }
            else
            {
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpgradeTower(int index)
    {
        if (CurrentSelection == null) return;
        if (index >= CurrentSelection.GetUpgrades().Length) return;

        var upgrade = CurrentSelection.GetUpgrades()[index];
        if (gameManager.Instance.money < upgrade.cost) return;

        gameManager.Instance.TrySpendMoney(upgrade.cost);
        CurrentSelection.ApplyUpgrade(index);

        HideTooltip();
        ShowUI();
    }

    private void SellTower()
    {
        if (CurrentSelection == null) return;

        gameManager.Instance.AddMoney(CurrentSelection.GetSellPrice());
        Destroy(CurrentSelection.gameObject);
        HidePanel();
    }

    private void ShowTooltip(string text)
    {
        if (tooltipText != null)
            tooltipText.text = text;
    }

    private void HideTooltip()
    {
        if (tooltipText != null)
            tooltipText.text = "";
    }
    #endregion

    #region Panel Animation
    private IEnumerator AnimatePanel(bool show)
    {
        float elapsed = 0f;
        Vector2 startPos = panelRect.anchoredPosition;
        Vector2 targetPos = show ? shownPosition : hiddenPosition;
        float startAlpha = panelCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        while (elapsed < panelSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / panelSlideDuration);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        panelRect.anchoredPosition = targetPos;
        panelCanvasGroup.alpha = targetAlpha;
    }
    #endregion
}

