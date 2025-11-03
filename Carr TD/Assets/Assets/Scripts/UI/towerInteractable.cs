using UnityEngine;

[RequireComponent(typeof(tower))]
public class towerInteractable : MonoBehaviour
{
    [Header("Tower Info")]
    public string towerName = "Tower";
    public int baseSellPrice = 50;

    [Header("Tower Stats")]
    public float fireRate = 1f;
    public int vanDamage = 1;
    public GameObject vanPrefab;

    [Header("Upgrades")]
    public StatUpgradeOption[] upgrades;

    [Header("Audio")]
    public AudioClip upgradeSound; // Sound played when upgrading
    public float upgradeSoundVolume = 1f;
    public AudioClip sellSound;    // Sound played when selling
    public float sellSoundVolume = 1f;

    [HideInInspector] public int upgradeLevel = 0;

    private tower towerScript;
    private AudioSource audioSource;

    // Range change event for selection manager
    public System.Action OnRangeChanged;

    private void Start()
    {
        towerScript = GetComponent<tower>();
        SyncTowerStats();

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Make sound 3D
            audioSource.maxDistance = 15f;
        }
    }

    private void OnMouseEnter()
    {
        if (towerSelectionManager.Instance != null)
            towerSelectionManager.Instance.SelectTowerHover(this);
    }

    private void OnMouseExit()
    {
        if (towerSelectionManager.Instance != null)
            towerSelectionManager.Instance.DeselectIfHover(this);
    }

    private void OnMouseDown()
    {
        if (towerSelectionManager.Instance != null)
            towerSelectionManager.Instance.SelectTowerClick(this);
    }

    public void ApplyUpgrade(int index)
    {
        if (upgrades == null || index < 0 || index >= upgrades.Length)
        {
            Debug.LogWarning($"Invalid upgrade index on tower {towerName}!");
            return;
        }

        var upgrade = upgrades[index];

        fireRate += upgrade.fireRateIncrease;
        vanDamage += upgrade.damageIncrease;

        if (upgrade.newVanPrefab != null)
            vanPrefab = upgrade.newVanPrefab;

        if (!string.IsNullOrEmpty(upgrade.towerName))
            towerName = upgrade.towerName;

        if (upgrade.sellPrice >= 0)
            baseSellPrice = upgrade.sellPrice;

        upgrades = upgrade.nextUpgrades;
        upgradeLevel++;
        SyncTowerStats();

        // Notify range change
        OnRangeChanged?.Invoke();

        // Play upgrade sound
        if (upgradeSound != null && audioSource != null)
            audioSource.PlayOneShot(upgradeSound, upgradeSoundVolume);

        Debug.Log($"{towerName} upgraded with '{upgrade.upgradeName}' (Level {upgradeLevel})");
    }

    public void SellTower()
    {
        // Notify range change or other systems if needed
        OnRangeChanged?.Invoke();

        // Play sell sound
        if (sellSound != null && audioSource != null)
            audioSource.PlayOneShot(sellSound, sellSoundVolume);

        // Example: give player money here
        gameManager.Instance.AddMoney(baseSellPrice);

        // Optionally, destroy tower
        Destroy(gameObject);

        Debug.Log($"{towerName} sold for {baseSellPrice}");
    }

    private void SyncTowerStats()
    {
        if (towerScript != null)
        {
            towerScript.fireRate = fireRate;
            towerScript.vanDamage = vanDamage;
            towerScript.vanPrefab = vanPrefab;
        }
    }

    public StatUpgradeOption[] GetUpgrades() => upgrades;
    public int GetSellPrice() => baseSellPrice;

    public float GetRange() => towerScript != null ? towerScript.range : 0f;
}

[System.Serializable]
public class StatUpgradeOption
{
    [Header("Upgrade Info")]
    public string upgradeName = "Upgrade";
    public int cost = 50;

    [Header("Resulting Tower Info (optional)")]
    public string towerName;
    public int sellPrice = -1;

    [Header("Stat changes")]
    public float fireRateIncrease = 0f;
    public int damageIncrease = 0;

    [Header("Projectile Prefab (optional)")]
    public GameObject newVanPrefab;

    [Header("Next Upgrade Options")]
    public StatUpgradeOption[] nextUpgrades;

    [Header("Optional Tooltip Description")]
    [TextArea]
    public string description;
}