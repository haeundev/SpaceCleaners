using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace AuthPair.Firebase
{
    /// <summary>
    /// The Firebase Auth handler
    /// </summary>
    public class FirebaseAuth : MonoBehaviour
    {
        [Header("Firebase Config")]
        [Tooltip("The Realtime Database URL of your Firebase project")]
        public string databaseURL = "https://authpair-default-rtdb.europe-west1.firebasedatabase.app"; // TODO: Replace with your own database URL (Details on: https://appscore.gitbook.io/authpair/how-to/set-up-your-own-auth-url)

        [Tooltip("The API Key of your Firebase project")]
        public string apiKey = "AIzaSyCMukbmVyMI1XfZ3pTpp5iN7MXT3XGuQwk"; // TODO: Replace with your own API key (Details on: https://appscore.gitbook.io/authpair/how-to/set-up-your-own-auth-url)                

        /// <summary>
        /// The Anonymous Login (to retrieve the pairing code)
        /// </summary>
        public IEnumerator LoginAnonymously(AuthPair auth, Action onError)
        {
            using (UnityWebRequest www = FirebaseHelper.GetWebRequest("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey, UnityWebRequest.kHttpVerbPOST, "{\"returnSecureToken\":true}"))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError("===============================================");
                    Debug.LogError($"url : {www.url}");
                    Debug.LogError($"method : {www.method}");
                    Debug.LogError($"uri : {www.uri}");
                    Debug.LogError($"responseCode : {www.responseCode}");
                    Debug.LogError(www.error);
                    onError?.Invoke();
                }
                else
                {
                    Debug.LogError($"url : {www.url}");
                    Debug.LogError($"method : {www.method}");
                    Debug.LogError($"uri : {www.uri}");
                    Debug.LogError($"responseCode : {www.responseCode}");
                    
                    
                    
                    UserData data = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                    auth.User = new User { id = data.localId };
                    auth.IdToken = data.idToken;
                    auth.PairCode = auth.User.id.ToLower().Substring(0, auth.PairCodeLength);
                    PlayerPrefs.SetString("refreshToken", data.refreshToken);
                    StartCoroutine(GetTokenAndLogin(auth));
                }
            }
        }

        /// <summary>
        /// The Login by Refresh Token
        /// </summary>
        public IEnumerator LoginByRefreshToken(AuthPair auth, string token, Action onError)
        {
            using (UnityWebRequest www = FirebaseHelper.GetWebRequest("https://securetoken.googleapis.com/v1/token?key=" + apiKey, UnityWebRequest.kHttpVerbPOST, "{\"grant_type\":\"refresh_token\",\"refresh_token\":\"" + token + "\"}"))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"url : {www.url}");
                    Debug.LogError($"method : {www.method}");
                    Debug.LogError($"uri : {www.uri}");
                    Debug.LogError($"responseCode : {www.responseCode}");
                    
                    
                    Debug.Log(www.error);
                    PlayerPrefs.SetString("refreshToken", "");
                    onError?.Invoke();
                }
                else
                {
                    TokenData data = JsonUtility.FromJson<TokenData>(www.downloadHandler.text);

                    if (!string.IsNullOrEmpty(auth.PairCode))
                    {
                        // If there is a Pair Code (= used for storing the Refresh Token) -> delete the Pair Code with its Refresh Token
                        using (UnityWebRequest www2 = FirebaseHelper.GetWebRequest($"{databaseURL}/tokens/{auth.PairCode}.json?auth=" + auth.IdToken, UnityWebRequest.kHttpVerbDELETE))
                        {
                            Debug.LogError($"url : {www2.url}");
                            Debug.LogError($"method : {www2.method}");
                            Debug.LogError($"uri : {www2.uri}");
                            Debug.LogError($"responseCode : {www2.responseCode}");
                            
                            
                            yield return www2.SendWebRequest();

                            if (www2.result == UnityWebRequest.Result.ConnectionError)
                            {
                                Debug.Log(www2.error);
                                yield break;
                            }
                        }
                    }

                    PlayerPrefs.SetString("refreshToken", data.refresh_token);
                    StartCoroutine(LoginByIdToken(auth, data.id_token));
                }
            }
        }

        /// <summary>
        /// Gets the token by pairing code and logs in
        /// </summary>
        IEnumerator GetTokenAndLogin(AuthPair auth)
        {
            bool tokenReceived = false;

            if (auth.PairCode != null)
            {
                while (!tokenReceived && auth.IsLoggingIn)
                {
                    yield return new WaitForSeconds(3);
                    using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{databaseURL}/tokens/{auth.PairCode}/token.json?auth=" + auth.IdToken))
                    {
                        yield return www.SendWebRequest();

                        if (www.isDone)
                        {
                            var token = www.downloadHandler.text.Trim('"');
                            tokenReceived = token.Length > 50;
                            if (tokenReceived)
                            {
                                StartCoroutine(LoginByRefreshToken(auth, token, auth.CancelLogin));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The Login by Id Token
        /// </summary>
        IEnumerator LoginByIdToken(AuthPair auth, string token)
        {
            var previousIdToken = token != auth.IdToken ? auth.IdToken : null;
            auth.IdToken = token;

            using (UnityWebRequest www = FirebaseHelper.GetWebRequest("https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + apiKey, UnityWebRequest.kHttpVerbPOST, "{\"idToken\":\"" + token + "\"}"))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    auth.User = new User(www.downloadHandler.text);

                    if (!auth.User.isAnonym)
                    {
                        auth.PairCode = string.Empty;
                        auth.User.lastLoginAt = System.DateTime.UtcNow;
                        auth.loggedIn?.Invoke();

                        StartCoroutine(SetUserData(auth));

                        if (!string.IsNullOrEmpty(previousIdToken))
                        {
                            // If there is an previous Id Token (= anonymous account used for pairing) -> delete the account
                            using (UnityWebRequest www3 = FirebaseHelper.GetWebRequest("https://identitytoolkit.googleapis.com/v1/accounts:delete?key=" + apiKey, UnityWebRequest.kHttpVerbPOST, "{\"idToken\":\"" + previousIdToken + "\"}"))
                            {
                                yield return www3.SendWebRequest();

                                if (www3.result == UnityWebRequest.Result.ConnectionError)
                                {
                                    Debug.Log(www3.error);
                                }
                            }                                
                        }
                    }
                    else if(auth.User != null && auth.User.id != null)
                    {
                        auth.PairCode = auth.User.id.ToLower().Substring(0, auth.PairCodeLength);
                        StartCoroutine(GetTokenAndLogin(auth));
                    }
                    else
                    {
                        auth.Logout();
                    }
                }
            }
        }

        /// <summary>
        /// Writes the user data to Firebase
        /// </summary>
        IEnumerator SetUserData(AuthPair auth)
        {
            using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{databaseURL}/users/{auth.User.id}.json?auth=" + auth.IdToken, "PATCH", JsonUtility.ToJson(auth.User)))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
            }
        }
    }
}
