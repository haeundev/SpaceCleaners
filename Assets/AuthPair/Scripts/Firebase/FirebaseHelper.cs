using System.Text;
using UnityEngine.Networking;

namespace AuthPair.Firebase
{
    /// <summary>
    /// The Firebase Core handler
    /// </summary>
    public class FirebaseHelper
    {
        /// <summary>
        /// Creates a UnityWebRequest for Firebase (returns the UnityWebRequest)
        /// </summary>
        public static UnityWebRequest GetWebRequest(string url, string method = UnityWebRequest.kHttpVerbGET, string body = null)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);
            if (body != null)
            {
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body.ToCharArray())) { contentType = "application/json" };
            }
            www.downloadHandler = new DownloadHandlerBuffer();
            return www;
        }
    }
}