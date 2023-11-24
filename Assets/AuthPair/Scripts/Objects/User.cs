using System;
using UnityEngine;

namespace AuthPair
{
    /// <summary>
    /// The AuthPair User
    /// </summary>
    [Serializable]
    public class User : ISerializationCallbackReceiver
    {
        public string email, displayName;
        public DateTime lastLoginAt;
        public DateTime createdAt;

        [NonSerialized]
        public string id, photoUrl;

        [NonSerialized]
        public bool emailVerified, isAnonym = true;

        public User() { }

        public User (string jsonString)
        {
            UserData userData = null;
            try
            {
                var response = JsonUtility.FromJson<LookupResponse>(jsonString);
                userData = response.users.Length > 0 ? response.users[0] : null;
            }
            catch (Exception)
            {
                try
                {
                    userData = JsonUtility.FromJson<UserData>(jsonString);
                }
                catch (Exception) { }
            }

            if (userData != null)
            {
                id = userData.localId;
                email = userData.email;
                emailVerified = userData.emailVerified;
                displayName = userData.displayName;
                photoUrl = userData.photoUrl;
                lastLoginAt = userData.lastLoginAt != null ? (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(userData.lastLoginAt)) : DateTime.UtcNow;
                createdAt = userData.lastLoginAt != null ? (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(userData.createdAt)) : DateTime.UtcNow;
                isAnonym = userData.providerUserInfo == null || userData.providerUserInfo.Length == 0;
            }
        }

        // serialized backing field that will actually receive the json string
        [SerializeField] 
        private string lastLogin, created;

        public void OnBeforeSerialize()
        {
            created = createdAt.ToString("O");
            lastLogin = lastLoginAt.ToString("O");
        }

        public void OnAfterDeserialize()
        {
            DateTime.TryParse(created, out createdAt);
            DateTime.TryParse(lastLogin, out lastLoginAt);
        }
    }

    /// <summary>
    /// The user data retrieved from https://identitytoolkit.googleapis.com/v1/accounts:lookup
    /// </summary>
    [Serializable]
    public class UserData
    {
        public string localId, email, displayName, photoUrl, lastLoginAt, createdAt, idToken, refreshToken;
        public bool emailVerified;
        public ProviderUserInfo[] providerUserInfo;
    }

    /// <summary>
    /// The token data retrieved from https://securetoken.googleapis.com/v1/token
    /// </summary>
    [Serializable]
    public class TokenData
    {
        public string refresh_token, id_token;
    }

    /// <summary>
    /// The provider user info retrieved from user data retrieved from https://identitytoolkit.googleapis.com/v1/accounts:lookup
    /// </summary>
    [Serializable]
    public class ProviderUserInfo
    {
        public string providerId, displayName, photoUrl, federatedId, email, rawId, screenName, phoneNumber;        
    }

    /// <summary>
    /// The lookup response retrieved from https://identitytoolkit.googleapis.com/v1/accounts:lookup
    /// </summary>
    [Serializable]
    public class LookupResponse
    {
        public string kind;
        public UserData[] users;
    }

    /// <summary>
    /// The update response retrieved from https://identitytoolkit.googleapis.com/v1/accounts:update
    /// </summary>
    [Serializable]
    public class UpdateResponse
    {
        public string localId;
        public string idToken;
    }
}
