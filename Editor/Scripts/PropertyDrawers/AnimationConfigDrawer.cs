#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Editor.PropertyDrawers
{
    

    [CustomPropertyDrawer(typeof(AnimationConfig))]
    public class AnimationConfigDrawer : PropertyDrawer
    {
        // ��������
        private const float LINE_HEIGHT = 20f;
        private const float SPACING = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 1. �����۵���ͷ������ԭʼ������
            Rect foldoutRect = new Rect(
                position.x,
                position.y,
                position.width,
                LINE_HEIGHT
            );
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                // 2. ����������
                EditorGUI.indentLevel++;
                Rect contentRect = new Rect(
                    position.x,
                    position.y + LINE_HEIGHT + SPACING,
                    position.width,
                    LINE_HEIGHT
                );

                // 3. ����ö��ѡ����
                SerializedProperty lerpTypeProp = property.FindPropertyRelative("LerpTargetValue");
                SerializedProperty lerpTarget = property.FindPropertyRelative("Target");
                EditorGUI.PropertyField(contentRect, lerpTypeProp);
                EditorGUI.PropertyField(new Rect(contentRect.x, contentRect.y + 150, contentRect.width, contentRect.height), lerpTarget);
                contentRect.y += LINE_HEIGHT + SPACING;

                // 4. ����ö�����Ͷ�̬�����ֶ�
                switch ((AnimationConfig.LerpClassEnum)lerpTypeProp.enumValueIndex)
                {
                    case AnimationConfig.LerpClassEnum.Position:
                        DrawPositionFields(ref contentRect, property);
                        break;

                    case AnimationConfig.LerpClassEnum.Color:
                        DrawColorFields(ref contentRect, property);
                        break;

                    case AnimationConfig.LerpClassEnum.Scale:
                        DrawScaleFields(ref contentRect, property);
                        break;
                    case AnimationConfig.LerpClassEnum.Rotate:
                        DrawRotationFields(ref contentRect,property);
                        break;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        // λ���ֶλ���
        private void DrawPositionFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "λ�ù���", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            SerializedProperty enterPos = property.FindPropertyRelative("enterPos");
            SerializedProperty pointDownPos = property.FindPropertyRelative("pressPos");
            EditorGUI.PropertyField(rect, enterPos);
            rect.y += LINE_HEIGHT + SPACING;
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField(rect, pointDownPos);
        }

        // ��ɫ�ֶλ���
        private void DrawColorFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "��ɫ����", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            SerializedProperty enterColor = property.FindPropertyRelative("enterColor");
            SerializedProperty exitColor = property.FindPropertyRelative("exitColor");
            SerializedProperty pointDownColor = property.FindPropertyRelative("pressColor");
            EditorGUI.PropertyField(rect, enterColor);
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField(rect, exitColor);
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField (rect, pointDownColor);
        }

        // �����ֶλ���
        private void DrawScaleFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "���Ź���", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            SerializedProperty enterScale = property.FindPropertyRelative("enterScale");
            SerializedProperty pointDownScale = property.FindPropertyRelative("pressScale");
            EditorGUI.PropertyField(rect, enterScale);
            rect.y += LINE_HEIGHT + SPACING;
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField (rect, pointDownScale);
        }
        // ��ת�ֶλ���
        private void DrawRotationFields(ref Rect rect,SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "��ת����", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;
            SerializedProperty rotateWay = property.FindPropertyRelative("rotateWay");
            SerializedProperty enterAngle = property.FindPropertyRelative("enterAngle");
            SerializedProperty pressAngle = property.FindPropertyRelative("pressAngle");
            EditorGUI.PropertyField(rect, rotateWay);
            rect.y += LINE_HEIGHT + SPACING;
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField(rect, enterAngle);
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField(rect, pressAngle);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1; // �۵���ͷ

            if (property.isExpanded)
            {
                lineCount += 1; // ö��ѡ����
                lineCount += 5; // ���� + �����ֶ�
            }

            return lineCount * (LINE_HEIGHT + SPACING) * 1.3f;
        }
    }


}
#endif