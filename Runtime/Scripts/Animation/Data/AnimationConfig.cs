using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data
{

    [Serializable]
    public class AnimationConfig
    {
        public TargetClassEnum Target;
        public LerpClassEnum LerpTargetValue;
        public RotateXYZ rotateWay;
        public enum LerpClassEnum
        {
            Position,
            Color,
            Scale,
            Rotate
        }
        public enum RotateXYZ
        {
            x,y,z
        }
        public enum TargetClassEnum
        {
            Image,
            Text
        }

        public Vector2 enterPos;
        public Vector2 pressPos;
        public Color enterColor;
        public Color exitColor;
        public Color pressColor;
        public Vector2 enterScale;
        public Vector2 pressScale;
        public float enterAngle;
        public float pressAngle;
    }
}