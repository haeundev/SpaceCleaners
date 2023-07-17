using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GogoGaga.UHM
{
    public enum FOCUS_TYPE
    {
        Gameobject,
        UI
    }

    public class FOCUS_PROPERTIES
    {
        public float showTime;
        public float size;
        public FOCUS_TYPE mode;

        public FOCUS_PROPERTIES()
        {
            mode = FOCUS_TYPE.Gameobject;
            showTime = -1;
            size = 150;
        }

        /// <param name="showTime">After that time effect will be disabled. Use -1 if you don't want to use this property</param>
        /// <param name="size">Size in screen space for the circle. Limit is 100-250</param>
        public FOCUS_PROPERTIES(float showTime,float size)
        {
            this.showTime = showTime;
            this.size = size;  
            mode = FOCUS_TYPE.Gameobject;
        }

        public FOCUS_PROPERTIES(FOCUS_TYPE mode)
        {
            showTime = -1;
            size = 150;
            this.mode = mode;
        }

        /// <param name="showTime">After that time effect will be disabled. Use -1 if you don't want to use this property</param>
        /// <param name="size">Size in screen space for the circle. Limit is 100-250</param>
        public FOCUS_PROPERTIES(float showTime, float size, FOCUS_TYPE mode)
        {
            this.showTime = showTime;
            this.size = size;
            this.mode = mode;
        }
    }


    public class FocusOnObjectManager : MonoBehaviour
    {

        [Range(0.3f,3f)]public float ShowHideSpeed = 1;
        public RectTransform FocusPoint;
        RectTransform rectTransform;
        Transform _target;
        Vector3 _targetPos;

        Camera cam;
        CanvasGroup canvasGroup;
        bool showing = false;
        FOCUS_TYPE type; 

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            rectTransform.position = Vector3.zero;
            cam = UltimateHudManager.Instance.mainCam;
        }

        public void Create(Transform target, FOCUS_PROPERTIES properties)
        {
            _target = target;
            SetFocusSize(properties.size);
            type = properties.mode;
            Create(properties.showTime);
        }

        public void Create(Vector3 targetPos, FOCUS_PROPERTIES properties)
        {
            _targetPos = targetPos;
            SetFocusSize(properties.size);
            type = properties.mode;
            Create(properties.showTime);
        }

        void Create(float showTime )
        {
            if(showCorotuine!=null)
                StopCoroutine(showCorotuine);
            showCorotuine = StartCoroutine(Show(showTime));
        }
        

        void SetFocusSize(float size)
        {
            size = Mathf.Clamp(size,100, 250);
            FocusPoint.sizeDelta = new Vector2(size, size);
        }

        Coroutine showCorotuine;
        IEnumerator Show(float showTime = -1)
        {

            if (showTime > 0)
            {
                float timer = 0f;
                while (timer < showTime)
                {
                    Calculate();
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                }
                ShowHide(false);
            }
            else
            {
                while (true)
                {
                    Calculate();
                    yield return new WaitForEndOfFrame();
                }

            }
        }

        void Calculate()
        {
            if (cam == null)
            {
                cam = UltimateHudManager.Instance.mainCam;

                if (cam == null)
                {
                    Debug.LogError("No main camera assigned in Ultimate Hud Manager");
                    return;
                }
            }

            if (_target != null)
                _targetPos = _target.position;

            if (type == FOCUS_TYPE.Gameobject)
            {
                Vector3 dir = _targetPos - cam.transform.position;

                float dis = Vector3.Distance(cam.transform.position, _targetPos);

                float scaleFactor = UltimateHudManager.RemapValue(dis, 1, 10, 1.7f, 0.9f);

                float checkDot = Vector3.Dot(cam.transform.forward, dir.normalized);

                //Debug.Log(checkDot);

                transform.localScale = Vector3.one * Mathf.Clamp(scaleFactor, 0.9f, 1.7f);

                Vector3 pos = cam.WorldToScreenPoint(_targetPos);

                if (checkDot < 0)
                {
                    if (showing)
                        ShowHide(false);
                }
                else
                {
                    if (CheckPos(pos))
                        rectTransform.position = pos;
                }
            }
            else
            {
                if (CheckPos(_targetPos))
                    rectTransform.position = _targetPos;
            }
        }

        public void Stop()
        {
            _target = null;

            if (showCorotuine != null)
                StopCoroutine(showCorotuine);

            if (showing)
                ShowHide(false,true);
        }

        bool CheckPos(Vector3 pos)
        {
            bool check = true;
            if (pos.x > Screen.width || pos.x < 0 || pos.y > Screen.height || pos.y < 0)
            {
                check = false;
            }

            if(!check && showing)
            {
                ShowHide(false);
            }
            else if(check && !showing)
            {
                ShowHide(true);
            }

            return check;
        }
        
        void ShowHide(bool show,bool disableOnHide = false)
        {
            showing = show;

            if (showHideCorotuine != null)
                StopCoroutine(showHideCorotuine);

            showHideCorotuine = StartCoroutine(ShowHideAnim(disableOnHide));
        }

        Coroutine showHideCorotuine;
        IEnumerator ShowHideAnim(bool disableOnHide )
        {
            if (showing)
            {
                while(canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += Time.deltaTime * ShowHideSpeed;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (canvasGroup.alpha > 0)
                {
                    canvasGroup.alpha -= Time.deltaTime * ShowHideSpeed;
                    yield return new WaitForEndOfFrame();
                }
                if(disableOnHide)
                    gameObject.SetActive(false);
            }
        }
    }
}