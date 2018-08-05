using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class AgentAnimation
    {
        private Animator m_cAnimator;
        private AnimationClip[] m_arrAnimClip;

        public void Init(Animator cAnimator)
        {
            m_cAnimator = cAnimator;
            if (m_cAnimator != null)
            {
                if (m_cAnimator.runtimeAnimatorController != null)
                {
                    m_arrAnimClip = m_cAnimator.runtimeAnimatorController.animationClips;
                }
            }
        }

        public void Clear()
        {
            m_cAnimator = null;
            m_arrAnimClip = null;
        }

        public void PlayAnim(string animName)
        {
            if (m_cAnimator != null)
            {
                m_cAnimator.Play(animName);
            }
        }

        public void ResetParam()
        {
            if (null != m_cAnimator)
            {
                var ps = m_cAnimator.parameters;
                foreach (var item in ps)
                {
                    switch (item.type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            SetBool(item.name, item.defaultBool);
                            break;
                        case AnimatorControllerParameterType.Float:
                            SetFloat(item.name, item.defaultFloat);
                            break;
                        case AnimatorControllerParameterType.Int:
                            SetInteger(item.name, item.defaultInt);
                            break;
                        case AnimatorControllerParameterType.Trigger:
                            m_cAnimator.ResetTrigger(item.name);
                            break;
                    }
                }
            }
        }

        public bool HasParam(string paramName)
        {
            if (null != m_cAnimator && m_cAnimator.runtimeAnimatorController != null)
            {
                var ps = m_cAnimator.parameters;
                foreach (var item in ps)
                {
                    if (item.name == paramName) return true;
                }
            }
            return false;
        }

        public void SetFloat(string strName, float fValue)
        {
            if (null != m_cAnimator && HasParam(strName))
            {
                m_cAnimator.SetFloat(strName, fValue);
            }
        }

        public float GetFloat(string strName)
        {
            if (null != m_cAnimator && HasParam(strName))
            {
                return m_cAnimator.GetFloat(strName);
            }
            return 0.0f;
        }

        public void SetBool(string strName, bool bState)
        {
            if (null != m_cAnimator && HasParam(strName))
            {
                m_cAnimator.SetBool(strName, bState);
            }
        }

        public bool GetBool(string strName)
        {
            if (null != m_cAnimator && HasParam(strName))
            {
                return m_cAnimator.GetBool(strName);
            }
            return false;
        }

        public void SetInteger(string strName, int nValue)
        {
            if (null != m_cAnimator && HasParam(strName))
            {
                m_cAnimator.SetInteger(strName, nValue);
            }
        }

        public void SetTrigger(string strName)
        {
            if (null != m_cAnimator && !string.IsNullOrEmpty(strName) && HasParam(strName))
            {
                m_cAnimator.SetTrigger(strName);
            }
        }

        public void ResetTrigger(string strName)
        {
            if (null != m_cAnimator && !string.IsNullOrEmpty(strName) && HasParam(strName))
            {
                m_cAnimator.ResetTrigger(strName);
            }
        }

        public float GetLength(string strName)
        {
            if (m_arrAnimClip != null)
            {
                for (int i = 0; i != m_arrAnimClip.Length; ++i)
                {
                    AnimationClip cClip = m_arrAnimClip[i];
                    if (strName == cClip.name)
                    {
                        return cClip.length;
                    }
                }
            }
            return 0.0f;
        }

        public void SetAnimSpeed(float speed)
        {
            if (null != m_cAnimator)
            {
                m_cAnimator.speed = speed;
            }
        }
    }
}
