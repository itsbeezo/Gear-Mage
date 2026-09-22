using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearDefinition
{
    public int id;
    public string displayName;
    public Sprite icon;
    public int startingCopies;
    public int maxCopies;
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

    public IReadOnlyList<GearDefinition> GetAll()
    {
        return definitions;
    }
}
