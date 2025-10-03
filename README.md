# ZJM UI Animation Kit 🎨

一个功能强大的UI动画系统，为Unity UI元素提供流畅的插值动画效果。

## ✨ 特性

- 🚀 **四种动画类型**: 位置、颜色、缩放、旋转
- 🎯 **完整事件支持**: 鼠标进入、离开、按下、释放
- 🔊 **音频反馈**: 为每个事件添加音效
- 📱 **多组件支持**: Image、Text、TextMeshPro
- ⚡ **性能优化**: 智能协程管理，避免冲突
- 🎛️ **可视化配置**: 友好的Inspector面板

## 🚀 快速开始

### 安装
1. 通过Package Manager添加Git URL:
https://github.com/yourusername/com.zhangjm.ui-animation-kit.git


### 基础用法
1. 为UI元素添加 `BaseAnimationController` 组件
2. 在Inspector中配置动画效果
3. 运行！享受流畅的交互动画

## 📚 组件说明

### BaseAnimationController
完整的交互动画控制器，支持所有鼠标事件。

### SimpleAnimationController  
简化的动画控制器，适合程序控制的动画。

## 🎮 示例场景

导入Samples查看完整示例：
- 基础按钮动画
- 复杂组合效果  
- 页面过渡动画

## 🔧 系统要求

- Unity 2022.3 或更高版本
- TextMeshPro (可选，用于文本动画)

## 📄 许可证

MIT License - 可自由用于个人和商业项目