using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Animation.Data;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Utilities;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Animation
{
    public class SimpleAnimationController : MonoBehaviour
    {
        public List<AnimationPreset> lerpWays = new List<AnimationPreset>();
        public float changeSpeed;
        public AnimationCurve lerpCurve;
        public bool isUseUnScaledTime;
        public AudioClip playClip;
        public AudioClip quitClip;
        private Image image;
        private AudioSource audioSource;
        private Vector2 startPos;
        private Vector2 startScale;
        private Color startColor;
        private Quaternion startRotation;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                gameObject.AddComponent<AudioSource>();
            }
            image = GetComponent<Image>();
            startPos = image.rectTransform.localPosition;
            startScale = image.rectTransform.localScale;
            startColor = image.color;
            startRotation = image.rectTransform.localRotation;
        }
        private void PlayTargetAnimation(AnimationPreset lerpAction)
        {
            switch (lerpAction.prop)
            {
                case AnimationPreset.LerpProperty.Position:
                    AnimationLerper.ValueLerp(image.rectTransform, lerpAction.targetPos, changeSpeed, isUseUnScaledTime, this, lerpCurve);
                    break;
                case AnimationPreset.LerpProperty.Color:
                    AnimationLerper.ValueLerp(image.color, lerpAction.targetColor, changeSpeed, isUseUnScaledTime, this, lerpCurve, (color) =>
                    {
                        image.color = color;
                    });
                    break;
                case AnimationPreset.LerpProperty.Scale:
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.x, startScale.x + lerpAction.targetScale.x, changeSpeed, isUseUnScaledTime, this, lerpCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.x = value;
                        image.rectTransform.localScale = localScale;
                    });
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.y, startScale.y + lerpAction.targetScale.y, changeSpeed, isUseUnScaledTime, this, lerpCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.y = value;
                        image.rectTransform.localScale = localScale;
                    });
                    break;
                case AnimationPreset.LerpProperty.Rotate:
                    switch (lerpAction.rotateWay)
                    {
                        case AnimationPreset.RotateWay.z:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, lerpAction.targetAngle), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                        case AnimationPreset.RotateWay.x:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(lerpAction.targetAngle, 0, 0), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                        case AnimationPreset.RotateWay.y:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, lerpAction.targetAngle, 0), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                    }
                    break;
            }
        }
        private void PlayQuitAnimation(AnimationPreset lerpAction)
        {
            switch (lerpAction.prop)
            {
                case AnimationPreset.LerpProperty.Position:
                    AnimationLerper.ValueLerp(image.rectTransform, startPos, changeSpeed, isUseUnScaledTime, this, lerpCurve);
                    break;
                case AnimationPreset.LerpProperty.Color:
                    AnimationLerper.ValueLerp(image.color, startColor, changeSpeed, isUseUnScaledTime, this, lerpCurve, (color) =>
                    {
                        image.color = color;
                    });
                    break;
                case AnimationPreset.LerpProperty.Scale:
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.x, startScale.x, changeSpeed, isUseUnScaledTime, this, lerpCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.x = value;
                        image.rectTransform.localScale = localScale;
                    });
                    AnimationLerper.ValueLerp(image.rectTransform.localScale.y, startScale.y, changeSpeed, isUseUnScaledTime, this, lerpCurve, (value) =>
                    {
                        Vector2 localScale = image.rectTransform.localScale;
                        localScale.y = value;
                        image.rectTransform.localScale = localScale;
                    });
                    break;
                case AnimationPreset.LerpProperty.Rotate:
                    switch (lerpAction.rotateWay)
                    {
                        case AnimationPreset.RotateWay.x:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(startRotation.x, 0, 0), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                        case AnimationPreset.RotateWay.y:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, startRotation.y, 0), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                        case AnimationPreset.RotateWay.z:
                            AnimationLerper.ValueLerp(image.rectTransform, Quaternion.Euler(0, 0, startRotation.z), changeSpeed, isUseUnScaledTime, this, lerpCurve);
                            break;
                    }
                    break;
            }
        }
        public void Play()
        {
            for (int i = 0; i < lerpWays.Count; i++)
            {
                PlayTargetAnimation(lerpWays[i]);
            }
            if (playClip != null)
            {
                audioSource.clip = playClip;
                audioSource.Play();
            }
        }
        public void Quit()
        {
            for (int i = 0; i < lerpWays.Count; i++)
            {
                PlayQuitAnimation(lerpWays[i]);
            }
            if (quitClip != null)
            {
                audioSource.clip = quitClip;
                audioSource.Play();
            }

        }
    }
}

