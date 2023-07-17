using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.Enums;
using LiveLarson.Util;
using UnityEngine;
using Type = System.Type;

namespace LiveLarson.UISystem
{
    public abstract class UIController
    {
        private Action<UIController> _completeWindowSetting;
        private bool _waitClose;
        public UIWindow Window;
        public int ID;
        public bool IsOpenedWindow;

        protected UIController()
        {
            SetChildrenControllers();
        }

        public abstract Type UIWindowType { get; }
        public virtual UIType UIType => UIType.None;
        public Transform WindowTransform => Window.transform;
        public WindowOption WindowOption { get; private set; } = WindowOption.None;

        public bool LoadComplete { get; private set; }

        public int SortingOrder
        {
            get
            {
                if (Window == null)
                    return int.MaxValue;
                return Window.SortingOrder;
            }
        }

        public int SubWindowCount => MChildrenBodies.Count;

        public virtual void SetWindowOption(int id, Action<UIController> completeWindowSetting, WindowOption option)
        {
            ID = id;
            _completeWindowSetting = completeWindowSetting;
            WindowOption = option;
        }


        protected void CreateWindow<T>() where T : UIWindow
        {
            UIWindowManager.OpenWindow<T>(this, InitWindow, ID, SubWindowCount, WindowOption);
        }

        public virtual void InitWindow(UIWindow window)
        {
            Window = window;
            Window.isRenewalUI = true;
            Window.SetSortingOrder();
            UIWindowManager.GetSubWindows(() =>
            {
                if (!_waitClose) Awake();

                LoadComplete = true;
            }, this);
        }

        protected abstract void Awake();

        private void CheckedDisableUI()
        {
            var inputBlock = Window.GetComponent<UIInputBlock>();
            if (inputBlock != null) inputBlock.OnOpenAndCheckedDisable();
        }

        public virtual void Hide()
        {
            if (Window == null) return;
            Window.gameObject.SetActive(false);
            OnHide();
        }

        public virtual void Show()
        {
            Window.gameObject.SetActive(true);
            CheckedDisableUI();
            OnShow();
        }

        public virtual void TemporaryHide()
        {
            Window.HideLowSortingOrder();
        }

        public virtual void ReturnPrevOpenState()
        {
            Window.PrevVisibleState();
        }

        public virtual void Close()
        {
            UIWindowService.Close(this);
        }

        public void CloseAction()
        {
            _waitClose = true;
            if (LoadComplete)
            {
                foreach (var body in MChildrenBodies) body.CloseAction();
                OnDestroyAfterClose();
                Window.Close();
                OnClose();
            }
            else
            {
                CoroutineManager.ExecuteCoroutine(DelayClose());
            }
        }

        public void OnDestroyAfterClose()
        {
            UIWindowManager.CloseUIWindowBody(this);
        }

        private IEnumerator DelayClose()
        {
            yield return new WaitUntil(() => LoadComplete);
            CloseAction();
            Window.Close();
            OnClose();
        }

        #region Tree

        protected List<UIController> MChildrenBodies = new();
        public List<UIController> ChildrenBodies => MChildrenBodies;

        protected virtual void SetChildrenControllers()
        {
        }

        protected void AddChildBody(UIController body)
        {
            MChildrenBodies.Add(body);
        }

        #endregion

        #region Window Event

        public virtual void OnOpen()
        {
            if (LoadComplete)
                foreach (var body in MChildrenBodies)
                    body.OnOpen();
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnShow()
        {
            if (LoadComplete)
                foreach (var body in MChildrenBodies)
                    if (body.Window.IsOpen)
                        body.OnShow();

            IsOpenedWindow = true;
        }

        public virtual void OnHide()
        {
            if (LoadComplete)
                foreach (var body in MChildrenBodies)
                    if (body.Window.IsOpen)
                        body.OnHide();

            IsOpenedWindow = false;
        }

        public virtual void OnDestroy()
        {
            if (LoadComplete)
                foreach (var body in MChildrenBodies)
                    body.OnDestroy();
            else
                CoroutineManager.ExecuteCoroutine(DelayOnDestroy());
        }

        private IEnumerator DelayOnDestroy()
        {
            yield return new WaitUntil(() => LoadComplete);
            foreach (var body in MChildrenBodies) body.OnDestroy();
        }

        #endregion
    }
}