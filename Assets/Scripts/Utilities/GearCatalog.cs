using System.Collections.Generic;
using UnityEngine;

// What a gear does on the board [G1]. Clicker is the hand-turned starter gear (id 1); it fits none of the other three.
public enum GearType
{
    Unit,
    Booster,
    StatBuff,
    Clicker,
}

[System.Serializable]
public class GearDefinition
{
    public int id;
    public string displayName;
    public GearType type;
    [Tooltip("Board gear prefab spawned for this id.")]
    public GameObject prefab;
    public Sprite icon;
    public int startingCopies;
    public int maxCopies;
    public bool Sellable;
    public int cost;
    public int sellValue;
    public float weight;
}



public class GearCatalog : MonoBehaviour
{
    public static GearCatalog instance { get; private set; }

    public List<GearDefinition> definitions = new List<GearDefinition>();

    private Dictionary<int, GearDefinition> lookup;

    private void Awake()
    {
        instance = this;
        Initialize();
    }

    // Separated from Awake so tests can build the lookup without going
    // through Unity's scene-load lifecycle.
    public void Initialize()
    {
        lookup = new Dictionary<int, GearDefinition>();
        foreach (var def in definitions)
        {
            lookup[def.id] = def;
        }
    }

    public GearDefinition GetDefinition(int id)
    {
        return lookup != null && lookup.TryGetValue(id, out var def) ? def : null;
    }

    public GameObject GetPrefab(int id)
    {
        GearDefinition def = GetDefinition(id);
        if (def == null) return null;
        return def.prefab;
    }

    public IReadOnlyList<GearDefinition> GetAll()
    {
        return definitions;
    }
}
