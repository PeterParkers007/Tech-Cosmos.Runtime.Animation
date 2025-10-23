using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Core
{
    public class AnimationManager : Singleton<AnimationManager>
    {
        // 双key字典：(协程持有者, 动画标识) → 对应的协程
        // 标识建议用字符串（比如"Scale"、"ColorFade"），直观且易区分
        private Dictionary<(MonoBehaviour, string), Coroutine> _coroutineMap = new Dictionary<(MonoBehaviour, string), Coroutine>();

        #region 重载1：兼容原有逻辑（同一脚本单协程，不用传标识）
        public void StartNewCoroutine(MonoBehaviour owner, IEnumerator coroutine)
        {
            // 用固定空字符串作为默认标识，兼容旧代码
            StartNewCoroutine(owner, "", coroutine);
        }
        #endregion

        #region 重载2：支持同一脚本多协程（必须传标识，区分不同动画）
        /// <param name="owner">协程所属脚本（比如你的 AttackTargetMarkerEffect）</param>
        /// <param name="animTag">动画标识（同一脚本内唯一，比如"ScaleAnim"、"ColorAnim"）</param>
        /// <param name="coroutine">要执行的插值协程</param>
        public void StartNewCoroutine(MonoBehaviour owner, string animTag, IEnumerator coroutine)
        {
            // 构建双key（确保同一脚本不同标识的协程独立）
            var key = (owner, animTag);

            // 停止该标识对应的旧协程（避免同一动画重复执行）
            if (_coroutineMap.ContainsKey(key) && _coroutineMap[key] != null)
            {
                StopCoroutine(_coroutineMap[key]);
            }

            // 启动新协程并更新字典
            Coroutine newCoroutine = StartCoroutine(coroutine);
            _coroutineMap[key] = newCoroutine;
        }
        #endregion

        #region 精准停止协程（按 脚本+标识）
        /// <summary>
        /// 停止指定脚本中某个标识的协程（比如只停止颜色动画，不影响缩放动画）
        /// </summary>
        public void StopCoroutine(MonoBehaviour owner, string animTag)
        {
            var key = (owner, animTag);
            if (_coroutineMap.ContainsKey(key) && _coroutineMap[key] != null)
            {
                StopCoroutine(_coroutineMap[key]);
                _coroutineMap[key] = null; // 清空引用，避免内存残留
            }
        }
        #endregion

        #region 停止脚本内所有协程（批量清理）
        /// <summary>
        /// 停止指定脚本的所有插值协程（比如脚本禁用时调用）
        /// </summary>
        public void StopAllCoroutinesByOwner(MonoBehaviour owner)
        {
            // 遍历字典，找到该owner的所有协程并停止
            var keysToRemove = new List<(MonoBehaviour, string)>();
            foreach (var (key, coroutine) in _coroutineMap)
            {
                if (key.Item1 == owner && coroutine != null)
                {
                    StopCoroutine(coroutine);
                    keysToRemove.Add(key);
                }
            }
            // 移除已停止的协程记录
            foreach (var key in keysToRemove)
            {
                _coroutineMap.Remove(key);
            }
        }
        #endregion
    }
}