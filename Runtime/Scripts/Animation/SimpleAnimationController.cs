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
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            image = GetComponent<Image>();

            startPos = image.rectTransform.localPosition;
            startScale = image.rectTransform.localScale;
            startColor = image.color;
            startRotation = image.rectTransform.localRotation;
        }

        private void PlayAnimation(AnimationPreset lerpAction, bool isPlayAnimation)
        {
            var rectTransform = image.rectTransform;

            switch (lerpAction.prop)
            {
                case AnimationPreset.LerpProperty.Position:
                    Vector2 targetPos = isPlayAnimation ? lerpAction.targetPos : startPos;
                    AnimationLerper.ValueLerp(rectTransform, targetPos, changeSpeed, isUseUnScaledTime, this,"Position" ,lerpCurve);
                    break;

                case AnimationPreset.LerpProperty.Color:
                    Color targetColor = isPlayAnimation ? lerpAction.targetColor : startColor;
                    AnimationLerper.ValueLerp(image.color, targetColor, changeSpeed, isUseUnScaledTime, this,"Color" ,lerpCurve,
                        (color) => image.color = color);
                    break;

                case AnimationPreset.LerpProperty.Scale:
                    Vector2 targetScale = isPlayAnimation ?
                        new Vector2(startScale.x + lerpAction.targetScale.x, startScale.y + lerpAction.targetScale.y) :
                        startScale;

                    AnimationLerper.ValueLerp(rectTransform.localScale, targetScale, changeSpeed, isUseUnScaledTime, this,"Scale" ,lerpCurve,
                        (scale) => rectTransform.localScale = scale);
                    break;

                case AnimationPreset.LerpProperty.Rotate:
                    Quaternion targetRotation = isPlayAnimation ?
                        GetTargetRotation(lerpAction) :
                        startRotation;

                    AnimationLerper.ValueLerp(rectTransform, targetRotation, changeSpeed, isUseUnScaledTime, this,"Rotate" ,lerpCurve);
                    break;
            }
        }

        private Quaternion GetTargetRotation(AnimationPreset lerpAction)
        {
            return lerpAction.rotateWay switch
            {
                AnimationPreset.RotateWay.x => Quaternion.Euler(lerpAction.targetAngle, 0, 0),
                AnimationPreset.RotateWay.y => Quaternion.Euler(0, lerpAction.targetAngle, 0),
                AnimationPreset.RotateWay.z => Quaternion.Euler(0, 0, lerpAction.targetAngle),
                _ => Quaternion.identity
            };
        }

        public void Play()
        {
            ExecuteAnimations(true);
            PlayAudio(playClip);
        }

        public void Quit()
        {
            ExecuteAnimations(false);
            PlayAudio(quitClip);
        }

        private void ExecuteAnimations(bool isPlay)
        {
            foreach (var preset in lerpWays)
            {
                PlayAnimation(preset, isPlay);
            }
        }

        private void PlayAudio(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}