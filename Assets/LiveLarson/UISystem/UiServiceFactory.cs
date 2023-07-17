using System.Collections.Generic;

namespace LiveLarson.UISystem
{
    public class UiServiceFactory : IUiServiceFactory
    {
        private readonly Dictionary<int, IUiService> _services = new();

        public IUiService GetUiService(Enums.GameMode gameMode)
        {
            var key = (int)gameMode;
            if (_services.ContainsKey(key))
                return _services[key];
            return null;
        }

        public void RegisterUiService(Enums.GameMode gameMode, IUiService uiService)
        {
            var key = (int)gameMode;
            if (_services.ContainsKey(key))
                _services.Remove(key);
            _services.Add(key, uiService);
        }
    }
}