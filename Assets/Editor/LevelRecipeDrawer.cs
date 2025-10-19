using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LevelRecipe))]
public class LevelRecipeDrawer : PropertyDrawer
{
    const int FixedOreCount = 4;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded) return line + EditorGUIUtility.standardVerticalSpacing;
        // fold + levelValue + lockedByDefault + ore lines + padding
        int oreLines = FixedOreCount;
        return (1 + 1 + 1 + oreLines) * (line + EditorGUIUtility.standardVerticalSpacing) + 4;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float lineH = EditorGUIUtility.singleLineHeight;
        float pad = EditorGUIUtility.standardVerticalSpacing;
        Rect r = new Rect(position.x, position.y, position.width, lineH);

        property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label, true);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        r.y += lineH + pad;

        // levelValue
        var levelValueProp = property.FindPropertyRelative("levelValue");
        EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, lineH), levelValueProp, new GUIContent("Level Value"));
        r.y += lineH + pad;

        // lockedByDefault (if present)
        var lockedProp = property.FindPropertyRelative("lockedByDefault");
        if (lockedProp != null)
        {
            EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, lineH), lockedProp, new GUIContent("Locked By Default"));
            r.y += lineH + pad;
        }

        // cost array -> force size and populate oreType indices
        var costArray = property.FindPropertyRelative("cost");
        if (costArray != null)
        {
            if (costArray.arraySize != FixedOreCount)
                costArray.arraySize = FixedOreCount;

            string[] oreNames = System.Enum.GetNames(typeof(OreType));
            for (int i = 0; i < FixedOreCount; i++)
            {
                var elem = costArray.GetArrayElementAtIndex(i);
                var oreTypeProp = elem.FindPropertyRelative("oreType");
                var amountProp = elem.FindPropertyRelative("amount");

                // 강제: oreType 순서 고정
                if (oreTypeProp != null)
                {
                    if (oreTypeProp.enumValueIndex != i)
                        oreTypeProp.enumValueIndex = Mathf.Clamp(i, 0, oreNames.Length - 1);
                }

                string labelText = (i < oreNames.Length) ? oreNames[i] : $"Ore{i}";
                Rect labelRect = new Rect(r.x, r.y, position.width * 0.6f, lineH);
                Rect fieldRect = new Rect(r.x + position.width * 0.6f + 6, r.y, position.width * 0.4f - 6, lineH);

                EditorGUI.LabelField(labelRect, labelText);
                if (amountProp != null)
                    amountProp.intValue = EditorGUI.IntField(fieldRect, amountProp.intValue);

                r.y += lineH + pad;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
}