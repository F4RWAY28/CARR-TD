using System.Collections.Generic;
using UnityEngine;

public class destructibleManager : MonoBehaviour
{
    public static destructibleManager Instance;

    private List<Destructible> destructibles = new List<Destructible>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Registers a destructible object to the manager.
    /// </summary>
    public static void RegisterDestructible(Destructible d)
    {
        if (Instance != null && !Instance.destructibles.Contains(d))
            Instance.destructibles.Add(d);
    }

    /// <summary>
    /// Unregisters a destructible object from the manager.
    /// </summary>
    public static void UnregisterDestructible(Destructible d)
    {
        if (Instance != null && Instance.destructibles.Contains(d))
            Instance.destructibles.Remove(d);
    }

    /// <summary>
    /// Respawns all destructibles currently registered.
    /// </summary>
    public void RespawnAll()
    {
        foreach (var d in destructibles)
        {
            if (d != null)
                d.Respawn();
        }
    }

    /// <summary>
    /// Optional: deactivate all destructibles at once.
    /// </summary>
    public void DeactivateAll()
    {
        foreach (var d in destructibles)
        {
            if (d != null)
                d.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Optional: reactivate all destructibles without resetting positions.
    /// </summary>
    public void ActivateAll()
    {
        foreach (var d in destructibles)
        {
            if (d != null)
                d.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Returns the list of currently registered destructibles.
    /// </summary>
    public List<Destructible> GetAllDestructibles() => destructibles;
}
