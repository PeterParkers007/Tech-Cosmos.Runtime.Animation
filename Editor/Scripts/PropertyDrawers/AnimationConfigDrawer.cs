#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Editor.PropertyDrawers
{
    

    [CustomPropertyDrawer(typeof(AnimationConfig))]
    public class AnimationConfigDrawer : PropertyDrawer
    {
        // 常量定义
        private const float LINE_HEIGHT = 20f;
        private const float SPACING = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 1. 绘制折叠箭头（保留原始缩进）
            Rect foldoutRect = new Rect(
                position.x,
                position.y,
                position.width,
                LINE_HEIGHT
            );
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                // 2. 缩进子内容
                EditorGUI.indentLevel++;
                Rect contentRect = new Rect(
                    position.x,
                    position.y + LINE_HEIGHT + SPACING,
                    position.width,
                    LINE_HEIGHT
                );

                // 3. 绘制枚举选择器
                SerializedProperty lerpTypeProp = property.FindPropertyRelative("LerpTargetValue");
                SerializedProperty lerpTarget = property.FindPropertyRelative("Target");
                EditorGUI.PropertyField(contentRect, lerpTypeProp);
                EditorGUI.PropertyField(new Rect(contentRect.x, contentRect.y + 150, contentRect.width, contentRect.height), lerpTarget);
                contentRect.y += LINE_HEIGHT + SPACING;

                // 4. 根据枚举类型动态绘制字段
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

        // 位置字段绘制
        private void DrawPositionFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "位置过渡", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            SerializedProperty enterPos = property.FindPropertyRelative("enterPos");
            SerializedProperty pointDownPos = property.FindPropertyRelative("pressPos");
            EditorGUI.PropertyField(rect, enterPos);
            rect.y += LINE_HEIGHT + SPACING;
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField(rect, pointDownPos);
        }

        // 颜色字段绘制
        private void DrawColorFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "颜色过渡", EditorStyles.boldLabel);
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

        // 缩放字段绘制
        private void DrawScaleFields(ref Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "缩放过渡", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            SerializedProperty enterScale = property.FindPropertyRelative("enterScale");
            SerializedProperty pointDownScale = property.FindPropertyRelative("pressScale");
            EditorGUI.PropertyField(rect, enterScale);
            rect.y += LINE_HEIGHT + SPACING;
            rect.y += LINE_HEIGHT + SPACING;
            EditorGUI.PropertyField (rect, pointDownScale);
        }
        // 旋转字段绘制
        private void DrawRotationFields(ref Rect rect,SerializedProperty property)
        {
            EditorGUI.LabelField(rect, "旋转过渡", EditorStyles.boldLabel);
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
            int lineCount = 1; // 折叠箭头

            if (property.isExpanded)
            {
                lineCount += 1; // 枚举选择器
                lineCount += 5; // 标题 + 两个字段
            }

            return lineCount * (LINE_HEIGHT + SPACING) * 1.3f;
        }
    }


}
#endif