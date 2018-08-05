using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 进度条
    /// 如果ImageType = Filled，进度条的方向与形式可使用Filled中属性调整，SetProgressMode无效果
    /// 如果ImageType != Filled（只支持水平与垂直方向的进度条），SetProgressMode将有效果，控制方向请调整中心点Pivot位置
    /// 例如：left->right=>pivot(0,0.5) right->left=>pivot(1,0.5) top->bottom=>pivot(0.5,1) bottom->top=>pivot(0.5,0)
    /// </summary>
    public class UIProgress : MonoBehaviour
    {
        public event Action<float> OnProgressChange;
        protected Image _image;
        protected float _percent;
        protected float _totalValue;
        [SerializeField]
        private ProgressMode _progressMode = ProgressMode.Horizontal;

        private bool hasInit;
        protected virtual void Awake()
        {
            if (hasInit)
                return;
            hasInit = true;

            _image = gameObject.GetComponent<Image>();
            if (_image.type == Image.Type.Filled)
            {
                _percent = _image.fillAmount;
                _totalValue = 1;
            }
            else
            {
                _percent = -1;
                SetProgressMode(_progressMode);
            }
        }

        public virtual void Init()
        {
            Awake();
        }

        public void SetProgressMode(UIProgress.ProgressMode mode)
        {
            _progressMode = mode;
            if (_progressMode == ProgressMode.Horizontal)
            {
                _totalValue = _image.rectTransform.rect.width;
            }
            else
            {
                _totalValue = _image.rectTransform.rect.height;
            }
        }

        public void UpdateProgress(float percent)
        {
            if (_image == null) return;
            if (percent < 0) percent = 0f;
            else if (percent > 1) percent = 1f;

            if (_percent != percent)
            {
                _percent = percent;
                if (_image.type == Image.Type.Filled)
                {
                    _image.fillAmount = _percent;
                }
                else
                {
                    float changeValue = GetPercent() * _totalValue;
                    if (_progressMode == ProgressMode.Horizontal)
                    {
                        _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, changeValue);
                    }
                    else
                    {
                        _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, changeValue);
                    }
                }
                ChangeProcess(_percent);
            }
        }

        protected virtual float GetPercent()
        {
            return _percent;
        }

        protected void ChangeProcess(float value)
        {
            if (OnProgressChange != null)
            {
                OnProgressChange(value);
            }
        }

        protected bool IsFullProgress()
        {
            return _percent == 1f;
        }

        public enum ProgressMode
        {
            Horizontal,
            Vertical
        }
    }

}