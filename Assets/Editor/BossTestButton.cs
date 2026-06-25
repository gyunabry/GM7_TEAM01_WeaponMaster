using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BossController))]
public class BossTestButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BossController controller = (BossController)target;

        if (GUILayout.Button("Take Damage (No Crit)"))
        {
            controller.TakeDamage(50, false);
        }
        if (GUILayout.Button("Take Damage (Crit)"))
        {
            controller.TakeDamage(100, true);
        }
    }
}
