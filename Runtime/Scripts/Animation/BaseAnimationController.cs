using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Utilities;
[assembly: AssemblyTitle("Peter's UI Animation Lerp Tool")]
[assembly: AssemblyDescription("高级UI动画系统 - 按钮支持TextMeshPro和原生Text组件")]
[assembly: AssemblyCompany("张锦铭")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("Copyright ©  2025 张锦铭")]

namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Animation
{
    
    public class BaseAnimationController : MonoBehaviour
    {
        public List<AnimationConfig> lerpWays = new List<AnimationConfig>();
        private float buttonStartAngle;
        private float textStartAngle;
        private Vector2 buttonStartPos;
        private Vector2 buttonStartSize;
        private Vector2 textStartSize;
        private Image image;
        private TextMeshProUGUI textMeshText;
        private Text textT;
        private AudioSource audioSource;
        private Vector2 _currentPos;
        private HorizontalLayoutGroup HLG;
        private VerticalLayoutGroup VLG;
        private GridLayoutGroup GLP;
        [Header("是否受时间缩放影响")]
        public bool isUseUnScaledTime;
        [Header("速度")]
        public float changeSpeed;
        [Header("鼠标按下音效")]
        public AudioClip PressAudioClip;
        [Header("鼠标松开音效")]
        public AudioClip ReleaseAudioClip;
        [Header("鼠标悬停音效")]
        public AudioClip EnterAudioClip;
        [Header("鼠标移出音效")]
        public AudioClip ExitAudioClip;
        public AnimationCurve animationCurve;
        void Start()
        {
            HLG = transform.parent?.GetComponent<HorizontalLayoutGroup>();
            VLG = transform.parent?.GetComponent<VerticalLayoutGroup>();
            GLP = transform.parent?.GetComponent<GridLayoutGroup>();
            image = GetComponent<Image>();
            buttonStartPos = image.rectTransform.localPosition;
            buttonStartSize = image.rectTransform.localScale;
            buttonStartAngle = 0;
            textStartAngle = 0;
            textT = GetComponentInChildren<Text>();
            textMeshText = GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshText != null)
                textStartSize = textMeshText.rectTransform.localScale;
            else if (textT != null)
                textStartSize = textT.rectTransform.localScale;
            else
                textStartSize = Vector2.one;
            AddEvent(EventTriggerType.PointerEnter, (data) =>
            {
                MouseEnterEvent();
            });
            AddEvent(EventTriggerType.PointerExit, (data) =>
            {
                MouseExitEvent();
            });
            AddEvent(EventTriggerType.PointerDown, (data) =>
            {
                MousePointDownEvent();
            });
            AddEvent(EventTriggerType.PointerUp, (data) =>
            {
                MousePointUpEvent();
            });
        }
        private bool CheckParentLayout()
        {
            if (HLG != null || VLG != null || GLP != null)
            {
                return false;
            }

            return true;
        }
        private void AddEvent(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry()
            {
                eventID = type,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }
        public void PointDownEvent(AnimationConfig lerpAction, Text text = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform,buttonStartPos + lerpAction.pressPos,changeSpeed, isUseUnScaledTime, this,animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.pressColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x, buttonStartSize.x + lerpAction.pressScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y, buttonStartSize.y + lerpAction.pressScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.pressAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.pressAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.pressAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(text != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(text.rectTransform, lerpAction.pressPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(text.color, lerpAction.pressColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    text.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.x, textStartSize.x + lerpAction.pressScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.x = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.y, textStartSize.y + lerpAction.pressScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.y = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(lerpAction.pressAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, lerpAction.pressAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, 0, lerpAction.pressAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }

                                break;
                        }
                    }
                    break;
            }
        }
        public void PointDownEvent(AnimationConfig lerpAction,TextMeshProUGUI text = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform,buttonStartPos + lerpAction.pressPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.pressColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x,buttonStartSize.x + lerpAction.pressScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y,buttonStartSize.y + lerpAction.pressScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.pressAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.pressAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.pressAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(text != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(text.rectTransform, lerpAction.pressPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(text.color, lerpAction.pressColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    text.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.x, textStartSize.x + lerpAction.pressScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.x = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.y, textStartSize.y + lerpAction.pressScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.y = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(lerpAction.pressAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, lerpAction.pressAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, 0, lerpAction.pressAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }

                                break;
                        }
                    }
                    break;
            }
        }
        public void PointDownEvent(AnimationConfig lerpAction)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform, buttonStartPos + lerpAction.pressPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.pressColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x, buttonStartSize.x + lerpAction.pressScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y, buttonStartSize.y + lerpAction.pressScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.pressAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.pressAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.pressAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        public void PointUpEvent(AnimationConfig lerpAction,Text text = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform,buttonStartPos + lerpAction.enterPos,changeSpeed, isUseUnScaledTime, this,animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x, buttonStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y, buttonStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(text != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(text.rectTransform, buttonStartPos + lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(text.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    text.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.x, textStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.x = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.y, textStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.y = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void PointUpEvent(AnimationConfig lerpAction, TextMeshProUGUI text = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform, buttonStartPos + lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x, buttonStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y, buttonStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(text != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(text.rectTransform, buttonStartPos + lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(text.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    text.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.x, textStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.x = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(text.rectTransform.localScale.y, textStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = text.rectTransform.localScale;
                                    localScale.y = value;
                                    text.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(text.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void PointUpEvent(AnimationConfig lerpAction)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    switch (lerpAction.LerpTargetValue)
                    {
                        case AnimationConfig.LerpClassEnum.Position:
                            AnimationLerper.ValueLerp(image.rectTransform, buttonStartPos + lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.LerpClassEnum.Color:
                            AnimationLerper.ValueLerp(image.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                            {
                                image.color = color;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Scale:
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.x, buttonStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.x = value;
                                image.rectTransform.localScale = localScale;
                            });
                            AnimationLerper.ValueLerp(image.rectTransform.localScale.y, buttonStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                            {
                                Vector2 localScale = image.rectTransform.localScale;
                                localScale.y = value;
                                image.rectTransform.localScale = localScale;
                            });
                            break;
                        case AnimationConfig.LerpClassEnum.Rotate:
                            switch (lerpAction.rotateWay)
                            {
                                case AnimationConfig.RotateXYZ.x:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.y:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                                case AnimationConfig.RotateXYZ.z:
                                    AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        public void EnterEvent(AnimationConfig lerpAction, Text legacyText = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction, true);
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(legacyText != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(legacyText.rectTransform, lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(legacyText.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    legacyText.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(legacyText.rectTransform.localScale.x, textStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (scaleValue) =>
                                {
                                    Vector2 localScale = legacyText.rectTransform.localScale;
                                    localScale.x = scaleValue;
                                    legacyText.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(legacyText.rectTransform.localScale.y, textStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (scaleValue) =>
                                {
                                    Vector2 localScale = legacyText.rectTransform.localScale;
                                    localScale.y = scaleValue;
                                    legacyText.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void EnterEvent(AnimationConfig lerpAction, TextMeshProUGUI meshText = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction, true);
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(meshText != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(meshText.rectTransform, lerpAction.enterPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(meshText.color, lerpAction.enterColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    meshText.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(meshText.rectTransform.localScale.x, textStartSize.x + lerpAction.enterScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (scaleValue) =>
                                {
                                    Vector2 localScale = meshText.rectTransform.localScale;
                                    localScale.x = scaleValue;
                                    meshText.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(meshText.rectTransform.localScale.y, textStartSize.y + lerpAction.enterScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (scaleValue) =>
                                {
                                    Vector2 localScale = meshText.rectTransform.localScale;
                                    localScale.y = scaleValue;
                                    meshText.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(lerpAction.enterAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(0, lerpAction.enterAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(0, 0, lerpAction.enterAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;

                        }
                    }
                    break;
            }
        }
        public void EnterEvent(AnimationConfig lerpAction)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction,true);
                    break;
            }
        }
        public void ExitEvent(AnimationConfig lerpAction,Text legacyText = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction, false);

                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(legacyText != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(legacyText.rectTransform, new Vector2(0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(legacyText.color, lerpAction.exitColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    legacyText.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(legacyText.rectTransform.localScale.x, textStartSize.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = legacyText.rectTransform.localScale;
                                    localScale.x = value;
                                    legacyText.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(legacyText.rectTransform.localScale.y, textStartSize.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = legacyText.rectTransform.localScale;
                                    localScale.y = value;
                                    legacyText.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(textStartAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(0, textStartAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(legacyText.rectTransform, Quaternion.Euler(0, 0, textStartAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void ExitEvent(AnimationConfig lerpAction, TextMeshProUGUI meshText = null)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction,false);
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    if(meshText != null)
                    {
                        switch (lerpAction.LerpTargetValue)
                        {
                            case AnimationConfig.LerpClassEnum.Position:
                                AnimationLerper.ValueLerp(meshText.rectTransform, new Vector2(0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                break;
                            case AnimationConfig.LerpClassEnum.Color:
                                AnimationLerper.ValueLerp(meshText.color, lerpAction.exitColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                                {
                                    meshText.color = color;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Scale:
                                AnimationLerper.ValueLerp(meshText.rectTransform.localScale.x, textStartSize.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = meshText.rectTransform.localScale;
                                    localScale.x = value;
                                    meshText.rectTransform.localScale = localScale;
                                });
                                AnimationLerper.ValueLerp(meshText.rectTransform.localScale.y, textStartSize.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                                {
                                    Vector2 localScale = meshText.rectTransform.localScale;
                                    localScale.y = value;
                                    meshText.rectTransform.localScale = localScale;
                                });
                                break;
                            case AnimationConfig.LerpClassEnum.Rotate:
                                switch (lerpAction.rotateWay)
                                {
                                    case AnimationConfig.RotateXYZ.x:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(textStartAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.y:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(0, textStartAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                    case AnimationConfig.RotateXYZ.z:
                                        AnimationLerper.ValueLerp(meshText.rectTransform, Quaternion.Euler(0, 0, textStartAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void ExitEvent(AnimationConfig lerpAction)
        {
            switch (lerpAction.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleButtonAnimation(lerpAction,false);
                    break;
            }
        }
        public void MousePointDownEvent()
        {
            if(PressAudioClip != null)
            {
                audioSource.clip = PressAudioClip;
                audioSource.Play();
            }
            for(int i = 0; i < lerpWays.Count; i++)
            {
                if(textMeshText != null)
                {
                    PointDownEvent(lerpWays[i],textMeshText);
                }
                else if(textT != null)
                {
                    PointDownEvent(lerpWays[i], textT);
                }
                else
                {
                    PointDownEvent(lerpWays[i]);
                }
            }
        }
        public void MousePointUpEvent()
        {
            if(ReleaseAudioClip != null)
            {
                audioSource.clip = ReleaseAudioClip;
                audioSource.Play(); 
            }
            for(int i = 0;i < lerpWays.Count; i++)
            {
                if(textMeshText != null)
                {
                    PointUpEvent(lerpWays[i],textMeshText);
                }
                else if (textT != null)
                {
                    PointUpEvent(lerpWays[i], textT);
                }
                else
                {
                    PointUpEvent(lerpWays[i]);
                }
            }
        }
        public void MouseEnterEvent()
        {
            if(EnterAudioClip != null)
            {
                audioSource.clip = EnterAudioClip;
                audioSource.Play();
            }
            for (int i = 0; i < lerpWays.Count; i++)
            {
                if(textMeshText != null)
                {
                    EnterEvent(lerpWays[i],textMeshText);
                }
                else if (textT != null)
                {
                    EnterEvent(lerpWays[i],textT);
                }
                else
                {
                    EnterEvent(lerpWays[i]);
                }
                
            }
        }
        public void MouseExitEvent()
        {
            if(ExitAudioClip != null)
            {
                audioSource.clip = ExitAudioClip;
                audioSource.Play();
            }
            for (int i = 0; i < lerpWays.Count; i++)
            {
                if(textMeshText != null)
                {
                    ExitEvent(lerpWays[i],textMeshText);
                }
                else if(textT != null)
                {
                    ExitEvent(lerpWays[i],textT);
                }
                else
                {
                    ExitEvent(lerpWays[i]);
                }
            }
        }
        private void HandleButtonAnimation(AnimationConfig lerpAction, bool isEnter)
        {
            switch (lerpAction.LerpTargetValue)
            {
                case AnimationConfig.LerpClassEnum.Position:
                    if (CheckParentLayout())
                    {
                        Vector2 targetPos = isEnter ?
                            buttonStartPos + lerpAction.enterPos :
                            buttonStartPos;
                        AnimationLerper.ValueLerp(image.rectTransform, targetPos, changeSpeed, isUseUnScaledTime, this, animationCurve);
                    }
                    else
                    {
                        Debug.LogError($"按钮 {gameObject.name} 的Position动画与父物体Layout冲突", this);
                    }
                    break;

                case AnimationConfig.LerpClassEnum.Color:
                    Color targetColor = isEnter ? lerpAction.enterColor : lerpAction.exitColor;
                    AnimationLerper.ValueLerp(image.color, targetColor, changeSpeed, isUseUnScaledTime, this, animationCurve, (color) =>
                    {
                        image.color = color;
                    });
                    break;

                case AnimationConfig.LerpClassEnum.Scale:
                    Vector2 targetScale = isEnter ?
                        buttonStartSize + lerpAction.enterScale :
                        buttonStartSize;
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.x, targetScale.x, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.x = value;
                        image.rectTransform.localScale = localScale;
                    });
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.y, targetScale.y, changeSpeed, isUseUnScaledTime, this, animationCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.y = value;
                        image.rectTransform.localScale = localScale;
                    });
                    break;
                case AnimationConfig.LerpClassEnum.Rotate:
                    float targetAngle = isEnter ? lerpAction.enterAngle : buttonStartAngle;
                    switch (lerpAction.rotateWay)
                    {
                        case AnimationConfig.RotateXYZ.x:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(targetAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.RotateXYZ.y:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, targetAngle, 0), changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                        case AnimationConfig.RotateXYZ.z:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, targetAngle), changeSpeed, isUseUnScaledTime, this, animationCurve);
                            break;
                    }
                    
                    break;
            }
        }
    }

}