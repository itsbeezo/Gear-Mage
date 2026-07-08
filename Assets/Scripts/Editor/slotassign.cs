using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class slotassign
{
    private const float spacing = 1f;


    [MenuItem("Tools/Grid/Assign Row and Column")]
    private static void AssignSlots()
    {
        GridSlot[] slots = Object.FindObjectsByType<GridSlot>(FindObjectsSortMode.None);

        if (slots.Length == 0)
        {
            Debug.Log("Warning: No GridSlot components found in the scene.");
            return;
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;

        foreach (GridSlot slot in slots) 
        {
            Vector3 pos = slot.transform.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.y < minY) minY = pos.y;
        }

        foreach (GridSlot slot in slots)
        {
            Vector3 pos = slot.transform.position;
            int column = Mathf.RoundToInt((pos.x - minX) / spacing);
            int row = Mathf.RoundToInt((pos.y - minY) / spacing);

            slot.AssignCordinates(row, column);
            EditorUtility.SetDirty(slot); // Mark the object as dirty to ensure changes are saved
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"Assigned row/column to {slots.Length} grid slots");
    }
}
