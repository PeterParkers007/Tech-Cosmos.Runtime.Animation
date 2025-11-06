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

        // 配置字段
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

        // 组件引用
        private Image image;
        private TextMeshProUGUI textMeshText;
        private Text textT;
        private AudioSource audioSource;
        private HorizontalLayoutGroup HLG;
        private VerticalLayoutGroup VLG;
        private GridLayoutGroup GLP;

        // 初始状态
        private float buttonStartAngle;
        private float textStartAngle;
        private Vector2 buttonStartPos;
        private Vector3 buttonStartSize;
        private Vector3 textStartSize;
        private Vector2 textStartPos;

        void Start()
        {
            InitializeComponents();
            InitializeStartStates();
            SetupEventTriggers();
        }

        private void InitializeComponents()
        {
            HLG = transform.parent?.GetComponent<HorizontalLayoutGroup>();
            VLG = transform.parent?.GetComponent<VerticalLayoutGroup>();
            GLP = transform.parent?.GetComponent<GridLayoutGroup>();

            image = GetComponent<Image>();
            textT = GetComponentInChildren<Text>();
            textMeshText = GetComponentInChildren<TextMeshProUGUI>();
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        }

        private void InitializeStartStates()
        {
            buttonStartPos = image.rectTransform.localPosition;
            buttonStartSize = image.rectTransform.localScale;
            buttonStartAngle = 0;

            // 获取文本初始状态
            var textComponent = GetTextComponent();
            if (textComponent != null)
            {
                textStartSize = textComponent.transform.localScale;
                textStartPos = textComponent.transform.localPosition;
            }
            else
            {
                textStartSize = Vector2.one;
                textStartPos = Vector2.zero;
            }
        }

        private void SetupEventTriggers()
        {
            AddEvent(EventTriggerType.PointerEnter, (data) => MouseEnterEvent());
            AddEvent(EventTriggerType.PointerExit, (data) => MouseExitEvent());
            AddEvent(EventTriggerType.PointerDown, (data) => MousePointDownEvent());
            AddEvent(EventTriggerType.PointerUp, (data) => MousePointUpEvent());
        }

        #region 事件处理
        public void MousePointDownEvent()
        {
            PlayAudio(PressAudioClip);
            ExecuteAnimationEvent(AnimationEventType.PointerDown);
        }

        public void MousePointUpEvent()
        {
            PlayAudio(ReleaseAudioClip);
            ExecuteAnimationEvent(AnimationEventType.PointerUp);
        }

        public void MouseEnterEvent()
        {
            PlayAudio(EnterAudioClip);
            ExecuteAnimationEvent(AnimationEventType.PointerEnter);
        }

        public void MouseExitEvent()
        {
            PlayAudio(ExitAudioClip);
            ExecuteAnimationEvent(AnimationEventType.PointerExit);
        }

        private void ExecuteAnimationEvent(AnimationEventType eventType)
        {
            foreach (var config in lerpWays)
            {
                switch (eventType)
                {
                    case AnimationEventType.PointerDown:
                        HandleAnimation(config, AnimationEventType.PointerDown);
                        break;
                    case AnimationEventType.PointerUp:
                        HandleAnimation(config, AnimationEventType.PointerUp);
                        break;
                    case AnimationEventType.PointerEnter:
                        HandleAnimation(config, AnimationEventType.PointerEnter);
                        break;
                    case AnimationEventType.PointerExit:
                        HandleAnimation(config, AnimationEventType.PointerExit);
                        break;
                }
            }
        }
        #endregion

        #region 核心动画逻辑
        private void HandleAnimation(AnimationConfig config, AnimationEventType eventType)
        {
            switch (config.Target)
            {
                case AnimationConfig.TargetClassEnum.Image:
                    HandleImageAnimation(config, eventType);
                    break;
                case AnimationConfig.TargetClassEnum.Text:
                    HandleTextAnimation(config, eventType);
                    break;
            }
        }

        private void HandleImageAnimation(AnimationConfig config, AnimationEventType eventType)
        {
            switch (config.LerpTargetValue)
            {
                case AnimationConfig.LerpClassEnum.Position:
                    HandleImagePosition(config, eventType);
                    break;
                case AnimationConfig.LerpClassEnum.Color:
                    HandleImageColor(config, eventType);
                    break;
                case AnimationConfig.LerpClassEnum.Scale:
                    HandleImageScale(config, eventType);
                    break;
                case AnimationConfig.LerpClassEnum.Rotate:
                    HandleImageRotation(config, eventType);
                    break;
            }
        }

        private void HandleTextAnimation(AnimationConfig config, AnimationEventType eventType)
        {
            var textComponent = GetTextComponent();
            if (textComponent == null) return;

            switch (config.LerpTargetValue)
            {
                case AnimationConfig.LerpClassEnum.Position:
                    HandleTextPosition(config, eventType, textComponent);
                    break;
                case AnimationConfig.LerpClassEnum.Color:
                    HandleTextColor(config, eventType, textComponent);
                    break;
                case AnimationConfig.LerpClassEnum.Scale:
                    HandleTextScale(config, eventType, textComponent);
                    break;
                case AnimationConfig.LerpClassEnum.Rotate:
                    HandleTextRotation(config, eventType, textComponent);
                    break;
            }
        }
        #endregion

        #region 具体动画实现
        private void HandleImagePosition(AnimationConfig config, AnimationEventType eventType)
        {
            if (!CheckParentLayout()) return;

            Vector2 targetPos = eventType switch
            {
                AnimationEventType.PointerDown => buttonStartPos + config.pressPos,
                AnimationEventType.PointerUp => buttonStartPos + config.enterPos,
                AnimationEventType.PointerEnter => buttonStartPos + config.enterPos,
                AnimationEventType.PointerExit => buttonStartPos,
                _ => buttonStartPos
            };

            AnimationLerper.ValueLerp(image.rectTransform, targetPos, changeSpeed, isUseUnScaledTime, this, "ImagePositionCoroutine",animationCurve);
        }

        private void HandleImageColor(AnimationConfig config, AnimationEventType eventType)
        {
            Color targetColor = eventType switch
            {
                AnimationEventType.PointerDown => config.pressColor,
                AnimationEventType.PointerUp => config.enterColor,
                AnimationEventType.PointerEnter => config.enterColor,
                AnimationEventType.PointerExit => config.exitColor,
                _ => image.color
            };

            AnimationLerper.ValueLerp(image.color, targetColor, changeSpeed, isUseUnScaledTime, this, "ImageColorCoroutine" , animationCurve,
                (color) => image.color = color);
        }

        private void HandleImageScale(AnimationConfig config, AnimationEventType eventType)
        {
            Vector2 targetScale = eventType switch
            {
                AnimationEventType.PointerDown => buttonStartSize + config.pressScale,
                AnimationEventType.PointerUp => buttonStartSize + config.enterScale,
                AnimationEventType.PointerEnter => buttonStartSize + config.enterScale,
                AnimationEventType.PointerExit => buttonStartSize,
                _ => buttonStartSize
            };

            LerpScale(image.rectTransform, targetScale);
        }

        private void HandleImageRotation(AnimationConfig config, AnimationEventType eventType)
        {
            float targetAngle = eventType switch
            {
                AnimationEventType.PointerDown => config.pressAngle,
                AnimationEventType.PointerUp => config.enterAngle,
                AnimationEventType.PointerEnter => config.enterAngle,
                AnimationEventType.PointerExit => buttonStartAngle,
                _ => buttonStartAngle
            };

            LerpRotation(image.rectTransform, config.rotateWay, targetAngle);
        }

        private void HandleTextPosition(AnimationConfig config, AnimationEventType eventType, Component textComponent)
        {
            Vector2 targetPos = eventType switch
            {
                AnimationEventType.PointerDown => config.pressPos,
                AnimationEventType.PointerUp => config.enterPos,
                AnimationEventType.PointerEnter => config.enterPos,
                AnimationEventType.PointerExit => textStartPos,
                _ => textStartPos
            };

            AnimationLerper.ValueLerp(GetRectTransform(textComponent), targetPos, changeSpeed, isUseUnScaledTime, this,"TextPositionCoroutine" ,animationCurve);
        }

        private void HandleTextColor(AnimationConfig config, AnimationEventType eventType, Component textComponent)
        {
            Color targetColor = eventType switch
            {
                AnimationEventType.PointerDown => config.pressColor,
                AnimationEventType.PointerUp => config.enterColor,
                AnimationEventType.PointerEnter => config.enterColor,
                AnimationEventType.PointerExit => config.exitColor,
                _ => GetColor(textComponent)
            };

            AnimationLerper.ValueLerp(GetColor(textComponent), targetColor, changeSpeed, isUseUnScaledTime, this, "TextColorCoroutine" ,animationCurve,
                (color) => SetColor(textComponent, color));
        }

        private void HandleTextScale(AnimationConfig config, AnimationEventType eventType, Component textComponent)
        {
            Vector2 targetScale = eventType switch
            {
                AnimationEventType.PointerDown => textStartSize + config.pressScale,
                AnimationEventType.PointerUp => textStartSize + config.enterScale,
                AnimationEventType.PointerEnter => textStartSize + config.enterScale,
                AnimationEventType.PointerExit => textStartSize,
                _ => textStartSize
            };

            LerpScale(GetRectTransform(textComponent), targetScale);
        }

        private void HandleTextRotation(AnimationConfig config, AnimationEventType eventType, Component textComponent)
        {
            float targetAngle = eventType switch
            {
                AnimationEventType.PointerDown => config.pressAngle,
                AnimationEventType.PointerUp => config.enterAngle,
                AnimationEventType.PointerEnter => config.enterAngle,
                AnimationEventType.PointerExit => textStartAngle,
                _ => textStartAngle
            };

            LerpRotation(GetRectTransform(textComponent), config.rotateWay, targetAngle);
        }
        #endregion

        #region 工具方法
        private void LerpScale(RectTransform rectTransform, Vector3 targetScale)
        {
            AnimationLerper.ValueLerp(rectTransform.localScale, targetScale, changeSpeed, isUseUnScaledTime, this,
            $"LerpScale_{rectTransform.GetInstanceID()}", animationCurve,
            (scale) => {
                rectTransform.localScale = scale;
            });
        }

        private void LerpRotation(RectTransform rectTransform, AnimationConfig.RotateXYZ rotateWay, float angle)
        {
            Quaternion targetRotation = rotateWay switch
            {
                AnimationConfig.RotateXYZ.x => Quaternion.Euler(angle, 0, 0),
                AnimationConfig.RotateXYZ.y => Quaternion.Euler(0, angle, 0),
                AnimationConfig.RotateXYZ.z => Quaternion.Euler(0, 0, angle),
                _ => Quaternion.identity
            };

            AnimationLerper.ValueLerp(rectTransform, targetRotation, changeSpeed, isUseUnScaledTime, this,"LerpRotationCoroutine" ,animationCurve);
        }

        private Component GetTextComponent()
        {
            return textMeshText != null ? (Component)textMeshText : textT;
        }

        private RectTransform GetRectTransform(Component component)
        {
            return component.transform as RectTransform;
        }

        private Color GetColor(Component textComponent)
        {
            if (textComponent is TextMeshProUGUI tmp) return tmp.color;
            if (textComponent is Text text) return text.color;
            return Color.white;
        }

        private void SetColor(Component textComponent, Color color)
        {
            if (textComponent is TextMeshProUGUI tmp) tmp.color = color;
            else if (textComponent is Text text) text.color = color;
        }

        private void PlayAudio(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        private bool CheckParentLayout()
        {
            if (HLG != null || VLG != null || GLP != null)
            {
                Debug.LogError($"按钮 {gameObject.name} 的Position动画与父物体Layout冲突", this);
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
        #endregion
    }

    public enum AnimationEventType
    {
        PointerDown,
        PointerUp,
        PointerEnter,
        PointerExit
    }
}