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

            // ��ȡ�������л�����
            SerializedProperty prop = property.FindPropertyRelative("prop");
            SerializedProperty rotateWay = property.FindPropertyRelative("rotateWay");
            SerializedProperty targetPos = property.FindPropertyRelative("targetPos");
            SerializedProperty targetColor = property.FindPropertyRelative("targetColor");
            SerializedProperty targetScale = property.FindPropertyRelative("targetScale");
            SerializedProperty targetAngle = property.FindPropertyRelative("targetAngle");

            // ����λ��
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // �����۵���ͷ�ͱ�ǩ
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // ������ʾ��Щ����
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, prop);

                // ����ѡ�������������ʾ��Ӧ���ֶ�
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
                        // ����ʾ��ת��ʽ
                        EditorGUI.PropertyField(rect, rotateWay);
                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        // ����ʾ�Ƕ�
                        EditorGUI.PropertyField(rect, targetAngle);
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1; // ��ʼ�۵���

            if (property.isExpanded)
            {
                lineCount += 3; // prop, changeSpeed, curve, �͵�ǰѡ�е�����

                // �������ת���ԣ���Ҫ����һ����ʾ��ת��ʽ
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