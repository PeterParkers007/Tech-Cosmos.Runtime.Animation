using System.Collections;
using UnityEngine;
using ZJM_UI_EffectLerpTool.UIAnimationTool.Core;
namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Utilities
{
    public class AnimationLerper
    {
        public static void ValueLerp<T, U>(U obj, T targetValue, float lerpSpeed, bool useUnscaledTime, MonoBehaviour monoBehaviour, AnimationCurve curve = null, System.Action<T> onUpdate = null)
        {
            // 核心修改：不再新建CoroutineManager，改用单例Instance
            AnimationManager manager = AnimationManager.Instance;

            if (obj is GameObject gameObject && targetValue is Vector3 vector3Target)
            {
                Transform transform = gameObject.transform;
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, PosLerpCoroutine(transform, vector3Target, lerpSpeed, useUnscaledTime));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, PosLerpCoroutine(transform, vector3Target, lerpSpeed, useUnscaledTime, curve));
                }
            }
            else if (obj is Vector3 startVec3 && targetValue is Vector3 targetVec3)
            {
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, Vector3LerpCoroutine(startVec3, targetVec3, lerpSpeed, useUnscaledTime, onUpdate as System.Action<Vector3>));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, Vector3LerpCoroutine(startVec3, targetVec3, lerpSpeed, useUnscaledTime, onUpdate as System.Action<Vector3>, curve));
                }
            }
            else if (obj is RectTransform rectTransform && targetValue is Vector2 vector2Target)
            {
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, PosLerpCoroutine(rectTransform, vector2Target, lerpSpeed, useUnscaledTime));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, PosLerpCoroutine(rectTransform, vector2Target, lerpSpeed, useUnscaledTime, curve));
                }
            }
            else if (obj is Transform transF && targetValue is Quaternion quaternionTarget)
            {
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, RotationLerpCoroutine(transF, quaternionTarget, lerpSpeed, useUnscaledTime));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, RotationLerpCoroutine(transF, quaternionTarget, lerpSpeed, useUnscaledTime, curve));
                }
            }
            else if (obj is float floatValue && targetValue is float floatTarget)
            {
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, ValueLerpCoroutine(floatValue, floatTarget, lerpSpeed, useUnscaledTime, onUpdate as System.Action<float>));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, ValueLerpCoroutine(floatValue, floatTarget, lerpSpeed, useUnscaledTime, onUpdate as System.Action<float>, curve));
                }
            }
            else if (obj is Color colorValue && targetValue is Color colorTarget)
            {
                if (curve == null)
                {
                    manager.StartNewCoroutine(monoBehaviour, ColorLerpCoroutine(colorValue, colorTarget, lerpSpeed, useUnscaledTime, onUpdate as System.Action<Color>));
                }
                else
                {
                    manager.StartNewCoroutine(monoBehaviour, ColorLerpCoroutine(colorValue, colorTarget, lerpSpeed, useUnscaledTime, onUpdate as System.Action<Color>, curve));
                }
            }
            else
            {
                Debug.LogError("Unsupported type for ValueLerp");
            }
        }
        public static IEnumerator PosLerpCoroutine(Transform transform, Vector3 targetValue, float lerpSpeed, bool useUnscaledTime, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            Vector3 startValue = transform.position;

            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                transform.position = Vector3.Lerp(startValue, targetValue, t);

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }

            transform.position = targetValue;
        }
        public static IEnumerator Vector3LerpCoroutine(Vector3 startValue, Vector3 targetValue, float lerpSpeed, bool useUnscaledTime, System.Action<Vector3> onUpdate, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                Vector3 currentValue = Vector3.Lerp(startValue, targetValue, t);
                onUpdate?.Invoke(currentValue); // 回调传递Vector3，直接修改缩放

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }
            onUpdate?.Invoke(targetValue); // 确保最终值准确
        }
        public static IEnumerator RotationLerpCoroutine(Transform transform, Quaternion targetValue, float lerpSpeed, bool useUnscaledTime, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            Quaternion startValue = transform.localRotation;

            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                transform.localRotation = Quaternion.Lerp(startValue, targetValue, t);

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }

            transform.localRotation = targetValue;
        }

        public static IEnumerator PosLerpCoroutine(RectTransform rectTransform, Vector2 targetValue, float lerpSpeed, bool useUnscaledTime, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            Vector2 startValue = rectTransform.anchoredPosition;

            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                rectTransform.anchoredPosition = Vector2.Lerp(startValue, targetValue, t);

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }

            rectTransform.anchoredPosition = targetValue;
        }

        public static IEnumerator ColorLerpCoroutine(Color startColor, Color targetColor, float lerpSpeed, bool useUnscaledTime, System.Action<Color> onUpdate, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                Color currentColor = Color.Lerp(startColor, targetColor, t);
                onUpdate?.Invoke(currentColor);

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }
            onUpdate?.Invoke(targetColor);
        }
        public static IEnumerator ValueLerpCoroutine(float startValue, float endValue, float lerpSpeed, bool useUnscaledTime, System.Action<float> onUpdate, AnimationCurve curve = null)
        {
            float elapsedTime = 0;
            while (elapsedTime < 1)
            {
                float t = curve != null ? curve.Evaluate(elapsedTime) : elapsedTime;
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                onUpdate?.Invoke(currentValue);

                elapsedTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * lerpSpeed;
                yield return null;
            }
            onUpdate?.Invoke(endValue);
        }
    }

}