#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace Input
{
    [CustomEditor(typeof(Commander))]
    public class CommanderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var commander = (Commander)target;
            if (!commander)
                return;

            base.OnInspectorGUI();

            if (GUILayout.Button("Send"))
                commander.Send();
        }
    }
}
#endif