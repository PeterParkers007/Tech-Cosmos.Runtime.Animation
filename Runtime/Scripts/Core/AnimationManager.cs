using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZJM_UI_EffectLerpTool.UIAnimationTool.Core
{
    public class AnimationManager : Singleton<AnimationManager>
    {
        // 关键：按“协程持有者（MonoBehaviour）”存储对应的当前协程
        // 避免不同对象的协程互相干扰（比如A对象的插值不会影响B对象的插值）
        private Dictionary<MonoBehaviour, Coroutine> _objCoroutineMap = new Dictionary<MonoBehaviour, Coroutine>();

        // 外部调用：给指定MonoBehaviour启动新协程，自动停止该对象的旧协程
        public void StartNewCoroutine(MonoBehaviour owner, IEnumerator coroutine)
        {
            // 若该对象已有运行中的协程，先停止
            if (_objCoroutineMap.ContainsKey(owner) && _objCoroutineMap[owner] != null)
            {
                StopCoroutine(_objCoroutineMap[owner]);
            }

            // 启动新协程并更新映射表
            Coroutine newCoroutine = StartCoroutine(coroutine);
            _objCoroutineMap[owner] = newCoroutine;
        }

        // 可选：手动停止指定对象的协程（比如场景切换前清理）
        public void StopOwnerCoroutine(MonoBehaviour owner)
        {
            if (_objCoroutineMap.ContainsKey(owner) && _objCoroutineMap[owner] != null)
            {
                StopCoroutine(_objCoroutineMap[owner]);
                _objCoroutineMap[owner] = null;
            }
        }
    }
}