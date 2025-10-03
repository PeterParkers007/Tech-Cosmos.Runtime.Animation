#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Editor.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(AnimationPreset))]
    public class AnimationPresetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取所有序列化属性
            SerializedProperty prop = property.FindPropertyRelative("prop");
            SerializedProperty rotateWay = property.FindPropertyRelative("rotateWay");
            SerializedProperty targetPos = property.FindPropertyRelative("targetPos");
            SerializedProperty targetColor = property.FindPropertyRelative("targetColor");
            SerializedProperty targetScale = property.FindPropertyRelative("targetScale");
            SerializedProperty targetAngle = property.FindPropertyRelative("targetAngle");

            // 计算位置
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // 绘制折叠箭头和标签
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // 总是显示这些属性
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, prop);

                // 根据选择的属性类型显示相应的字段
                AnimationPreset.LerpProperty selectedProp = (AnimationPreset.LerpProperty)prop.enumValueIndex;

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                switch (selectedProp)
                {
                    case AnimationPreset.LerpProperty.Position:
                        EditorGUI.PropertyField(rect, targetPos);
                        break;

                    case AnimationPreset.LerpProperty.Color:
                        EditorGUI.PropertyField(rect, targetColor);
                        break;

                    case AnimationPreset.LerpProperty.Scale:
                        EditorGUI.PropertyField(rect, targetScale);
                        break;

                    case AnimationPreset.LerpProperty.Rotate:
                        // 先显示旋转方式
                        EditorGUI.PropertyField(rect, rotateWay);
                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        // 再显示角度
                        EditorGUI.PropertyField(rect, targetAngle);
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1; // 初始折叠行

            if (property.isExpanded)
            {
                lineCount += 3; // prop, changeSpeed, curve, 和当前选中的属性

                // 如果是旋转属性，需要额外一行显示旋转方式
                SerializedProperty prop = property.FindPropertyRelative("prop");
                if (prop.enumValueIndex == (int)AnimationPreset.LerpProperty.Rotate)
                {
                    lineCount += 1;
                }
            }

            return lineCount * EditorGUIUtility.singleLineHeight +
                   (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif