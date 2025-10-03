using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data
{

    [Serializable]
    public class AnimationPreset
    {
        public enum LerpProperty
        {
            Position,
            Color,
            Scale,
            Rotate,
        }
        public enum RotateWay
        {
            x, y, z
        }
        public LerpProperty prop;
        public RotateWay rotateWay;

        public Vector2 targetPos;
        public Color targetColor;
        public Vector2 targetScale;
        public float targetAngle;
    }
}