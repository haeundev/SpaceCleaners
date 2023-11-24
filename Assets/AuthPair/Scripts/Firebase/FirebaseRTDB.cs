
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AuthPair.Firebase
{
    /// <summary>
    /// A class which syncs GameObjects and Texts with Firebase Realtime Database
    /// </summary>
    [RequireComponent(typeof(AuthPair))]
    public class FirebaseRTDB : MonoBehaviour
    {
        [Tooltip("GameObjects that should be synced with Firebase Realtime Database")]
        public SyncedObject[] syncedObjects;

        [Tooltip("Texts that should be synced with Firebase Realtime Database")]
        public SyncedText[] syncedTexts;

        [Tooltip("Update frequency in seconds")]
        public float updateFrequency = 1f;
                
        [Header("Events")]
        public UnityEvent dataLoaded = new UnityEvent();

        private AuthPair _auth;
        private ObjectData[] _initialObjects;
        private string[] _initialTexts;

        void Awake()
        {
            _auth = gameObject.GetComponent<AuthPair>() ?? new AuthPair();
        }

        void Start()
        {
            _initialObjects = new ObjectData[syncedObjects.Length];
            _initialTexts = new string[syncedTexts.Length];

            for (var i=0; i< syncedObjects.Length; i++)
            {
                _initialObjects[i] = GetObjectData(syncedObjects[i].gameObject);
            }

            for (var i = 0; i < _initialTexts.Length; i++)
            {
                _initialTexts[i] = syncedTexts[i].text != null ? syncedTexts[i].text.text : string.Empty;
            }
        }

        public void StartSync()
        {
            GetData();
            InvokeRepeating(nameof(SetData), updateFrequency, updateFrequency);
        }

        public void StopSync()
        {
            CancelInvoke();
        }
        
        public void ResetData()
        {
            for (var i = 0; i < syncedObjects.Length; i++)
            {
                SetObjectData(syncedObjects[i].gameObject, _initialObjects[i]);
            }

            for (var i = 0; i < syncedTexts.Length; i++)
            {
                syncedTexts[i].text.text = _initialTexts[i];
            }
        }

        private void GetData()
        {
            if (_auth.IsLoggedIn)
            {
                foreach (var obj in syncedObjects)
                {
                    StartCoroutine(GetObject(obj));
                }

                foreach (var obj in syncedTexts)
                {
                    StartCoroutine(GetText(obj));
                }

                StartCoroutine(DataLoaded());
            }
        }
        
        private void SetData()
        {
            if (_auth.IsLoggedIn)
            {
                foreach (var obj in syncedObjects)
                {
                    StartCoroutine(PutObject(obj));
                }

                foreach (var obj in syncedTexts)
                {
                    StartCoroutine(PutText(obj));
                }
            }
        }

        IEnumerator DataLoaded()
        {
            yield return new WaitForSeconds(1);
            dataLoaded?.Invoke();
        }

        IEnumerator GetObject(SyncedObject syncedObject)
        {
            if (!string.IsNullOrEmpty(syncedObject.path) && _auth.FirebaseAuth != null && _auth.FirebaseAuth.databaseURL != null)
            {
                string dbUri = _auth.FirebaseAuth.databaseURL + ReplaceVariables(syncedObject.path.StartsWith("/") ? syncedObject.path :"/" + syncedObject.path);
                using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{dbUri}.json?auth=" + _auth.IdToken))
                {
                    yield return www.SendWebRequest();

                    if (www.error != null)
                    {
                        Debug.Log(www.error);                        
                    }
                    else 
                    {
                        try
                        {
                            ObjectData objectData = JsonUtility.FromJson<ObjectData>(www.downloadHandler.text);
                            SetObjectData(syncedObject.gameObject, objectData);                           
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        IEnumerator GetText(SyncedText syncedText)
        {
            if (!string.IsNullOrEmpty(syncedText.path) && _auth.FirebaseAuth != null && _auth.FirebaseAuth.databaseURL != null)
            {
                string dbUri = _auth.FirebaseAuth.databaseURL + ReplaceVariables(syncedText.path.StartsWith("/") ? syncedText.path : "/" + syncedText.path);
                using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{dbUri}.json?auth=" + _auth.IdToken))
                {
                    yield return www.SendWebRequest();

                    if (www.error != null)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        if(www.downloadHandler.text != "null")
                        {
                            syncedText.text.text = www.downloadHandler.text;
                        }
                    }
                }
            }
        }

        IEnumerator PutObject(SyncedObject syncedObject)
        {
            if (!string.IsNullOrEmpty(syncedObject.path) && syncedObject.gameObject != null && _auth.FirebaseAuth != null && _auth.FirebaseAuth.databaseURL != null)
            {
                string dbUri = _auth.FirebaseAuth.databaseURL + ReplaceVariables(syncedObject.path.StartsWith("/") ? syncedObject.path : "/" + syncedObject.path);
                ObjectData objectData = GetObjectData(syncedObject.gameObject);
                using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{dbUri}.json?auth=" + _auth.IdToken, "PATCH", JsonUtility.ToJson(objectData)))
                {
                    yield return www.SendWebRequest();

                    if (www.error != null)
                    {
                        Debug.LogError(www.error);
                    }
                }
            }
        }

        IEnumerator PutText(SyncedText syncedText)
        {
            if (!string.IsNullOrEmpty(syncedText.path) && syncedText.text != null && _auth.FirebaseAuth != null && _auth.FirebaseAuth.databaseURL != null)
            {
                string dbUri = _auth.FirebaseAuth.databaseURL + ReplaceVariables(syncedText.path.StartsWith("/") ? syncedText.path : "/" + syncedText.path);
                using (UnityWebRequest www = FirebaseHelper.GetWebRequest($"{dbUri}.json?auth=" + _auth.IdToken, "PUT", syncedText.text.text))
                {
                    yield return www.SendWebRequest();

                    if (www.error != null)
                    {
                        Debug.LogError(www.error);
                    }
                }
            }
        }

        private GameObject SetObjectData(GameObject gameObject, ObjectData data)
        {
            if(gameObject != null && data != null)
            {
                gameObject.transform.localPosition = GetVector3(data.position);
                gameObject.transform.localEulerAngles = GetVector3(data.rotation);
                gameObject.transform.localScale = GetVector3(data.scale, 1);

                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    rb.velocity = GetVector3(data.velocity);
                    rb.angularVelocity = GetVector3(data.angularVelocity);
                }

                gameObject.SetActive(data.isActive);
            }
            return gameObject;
        }

        private ObjectData GetObjectData(GameObject gameObject)
        {
            if (gameObject != null)
            {
                ObjectData objectData = new ObjectData(){
                    position = GetArray(gameObject.transform.localPosition),
                    rotation = GetArray(gameObject.transform.localEulerAngles),
                    scale = GetArray(gameObject.transform.localScale),
                    isActive = gameObject.activeSelf
                };

                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    objectData.velocity = GetArray(rb.velocity);
                    objectData.angularVelocity = GetArray(rb.angularVelocity);
                }

                return objectData;
            }

            return null;
        }

        private string ReplaceVariables(string text)
        {
            return text.Replace("[USER_ID]", _auth.User.id);
        }

        private Vector3 GetVector3(float[] data, float def = 0)
        {
            return new Vector3(data.Length > 0 ? data[0] : def, data.Length > 1 ? data[1] : def, data.Length > 2 ? data[2] : def);
        }

        private float[] GetArray(Vector3 vector)
        {
            return new float[] { vector.x, vector.y, vector.z };
        }
    }

    /// <summary>
    /// GameObject that will be synced with Firebase
    /// </summary>
    [Serializable]
    public class SyncedObject
    {
        public string path;
        public GameObject gameObject;
    }

    /// <summary>
    /// Text that will be synced with Firebase
    /// </summary>
    [Serializable]
    public class SyncedText
    {
        public string path;
        public Text text;
    }

    /// <summary>
    /// The object data that will be written to Firebase
    /// </summary>
    [Serializable]
    public class ObjectData
    {
        public float[] position = new float[0];
        public float[] rotation = new float[0];
        public float[] scale = new float[0];
        public float[] velocity = new float[0];
        public float[] angularVelocity = new float[0];
        public bool isActive = true;
    }
}