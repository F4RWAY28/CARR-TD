using UnityEngine;
using System.Collections.Generic;

public class destructibleManager : MonoBehaviour
{
    public static destructibleManager Instance { get; private set; }

    private static List<Destructible> destructibles = new List<Destructible>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void RegisterDestructible(Destructible d)
    {
        if (!destructibles.Contains(d))
            destructibles.Add(d);
    }

    public static void UnregisterDestructible(Destructible d)
    {
        destructibles.Remove(d);
    }

    public void RespawnAll()
    {
        foreach (Destructible d in destructibles)
        {
            if (d != null)
                d.Respawn();
        }
    }
}
