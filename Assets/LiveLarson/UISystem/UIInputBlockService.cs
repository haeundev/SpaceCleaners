using UnityEngine;

namespace LiveLarson.UISystem
{
    public static class InputBlockService
    {
        public delegate void InputDelegate();

        private static event InputDelegate EnableEvent;
        private static event InputDelegate DisableEvent;

        private static bool _mIsBlock;
        public static bool IsBlock => _mIsBlock;
    

        public static void RegisterEnableEvent(InputDelegate enableDelegate)
        {
            EnableEvent += enableDelegate;
        }
    
        public static void RemoveEnableEvent(InputDelegate enableDelegate)
        {
            EnableEvent -= enableDelegate;
        }
    
        public static void RegisterDisableEvent(InputDelegate disableDelegate)
        {
            DisableEvent += disableDelegate;
        }
   
        public static void RemoveDisableEvent(InputDelegate disableDelegate)
        {
            DisableEvent -= disableDelegate;
        }
    
        public static void Release()
        {
            if (!_mIsBlock)
            {
                Debug.Log("Input already unblocked!");
                return;
            }
            _mIsBlock = false;
            EnableEvent?.Invoke();
            Debug.Log("Input unblocked!");
        }
    
        public static void Block()
        {
            if (_mIsBlock)
            {
                Debug.Log("Input already blocked!");
                return;
            }
        
            DisableEvent?.Invoke();
            _mIsBlock = true;
            Debug.Log("Input blocked!");
        }
    }
}