using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AuthPair.Firebase;

namespace AuthPair
{
    /// <summary>
    /// The AuthPair handler
    /// </summary>
    [RequireComponent(typeof(FirebaseAuth))]
    public class AuthPair : MonoBehaviour
    {
        [Tooltip("The text field for displaying the auth instruction")]
        public Text authInstructionText;

        [Tooltip("The text field for displaying the pairing code")]
        public Text pairCodeText;

        [Tooltip("The text field for displaying the user data")]
        public Text userText;

        [Header("Events")]
        public UnityEvent logsIn = new UnityEvent();
        public UnityEvent loginCanceled = new UnityEvent();
        public UnityEvent loggedIn = new UnityEvent();
        public UnityEvent loggedOut = new UnityEvent();

        private static AuthPair _instance;
        private User _user;

        /// <summary>
        /// The current AuthPair Instance
        /// </summary>
        public static AuthPair Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthPair();
                }
                return _instance;
            }
        }               

        /// <summary>
        /// The PairCode length
        /// </summary>
        public int PairCodeLength
        {
            get
            {
                return 6;
            }
        }

        /// <summary>
        /// The current User instance
        /// </summary>
        public User User
        {
            set
            {
                _user = value;
            }
            
            get
            {
                if (_user == null)
                {
                    _user = new User();
                }
                return _user;
            }
        }

        /// <summary>
        /// The Firebase Auth instance
        /// </summary>
        public FirebaseAuth FirebaseAuth { get; set; }

        /// <summary>
        /// The Firebase Realtime Database instance
        /// </summary>
        public FirebaseRTDB FirebaseRTDB { get; set; }

        /// <summary>
        /// The current Pair Code
        /// </summary>
        public string PairCode { get; set; }

        /// <summary>
        /// The current Id Token
        /// </summary>
        public string IdToken { get; set; }        

        /// <summary>
        /// Shows if the user is logging in
        /// </summary>
        public bool IsLoggingIn 
        {
            get
            {
                return IdToken != null && User.isAnonym;
            }
        }

        /// <summary>
        /// Shows if the user is logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return !User.isAnonym;
            }
        }

        void Awake()
        {
            _instance = this;
            FirebaseAuth = gameObject.GetComponent<FirebaseAuth>();
            FirebaseRTDB = gameObject.GetComponent<FirebaseRTDB>();

            if(FirebaseRTDB != null)
            {
                loggedIn.AddListener(FirebaseRTDB.StartSync);
                loggedOut.AddListener(FirebaseRTDB.StopSync);
            }
        }

        void Start()
        {
            // If was logged in -> login on start
            if (PlayerPrefs.GetInt("login", 0) == 1)
            {
                Login();
            }
        }

        private void Update()
        {
            // Show the pairing code
            if (pairCodeText != null)
            {
                pairCodeText.text = IsLoggingIn ? (PairCode ?? "Connecting...") : string.Empty;
            }

            // Show the auth instruction
            if (authInstructionText != null)
            {
                authInstructionText.enabled = IsLoggingIn && PairCode != null;
            }

            // Show the logged in user
            if (userText != null)
            {
                userText.text = !IsLoggingIn ? User.displayName + "\n" + User.email : string.Empty;
            }
        }

        /// <summary>
        /// The Login, Cancel or Logout
        /// </summary>
        public void LoginCancelLogout()
        {
            if (User.isAnonym)
            {
                Login();
            }
            else
            {
                Logout();
            }
        }

        /// <summary>
        /// The Login
        /// </summary>
        public void Login()
        {
            if (!IsLoggingIn)
            {
                if (FirebaseAuth != null)
                {
                    PlayerPrefs.SetInt("login", 1);
                    logsIn?.Invoke();

                    
                    
                    StartCoroutine(FirebaseAuth.LoginAnonymously(this, CancelLogin));

                    
                    
                    
                    
                    
                    // string token = PlayerPrefs.GetString("refreshToken", "");
                    // if (string.IsNullOrEmpty(token))
                    // {
                    //     StartCoroutine(FirebaseAuth.LoginAnonymously(this, CancelLogin));
                    // }
                    // else
                    // {
                    //     StartCoroutine(FirebaseAuth.LoginByRefreshToken(this, token, CancelLogin));
                    // }
                }
                else
                {
                    Debug.LogError("Please add authentication component (e.g. FirebaseAuth) to the AuthPair Script");
                    CancelLogin();
                }
            }
            else
            {
                CancelLogin();
            }
        }

        /// <summary>
        /// Cancel the Login
        /// </summary>
        public void CancelLogin()
        {
            if (IsLoggingIn)
            {
                User = new User();
                IdToken = null;
                PairCode = null;
                PlayerPrefs.SetInt("login", 0);
                loginCanceled?.Invoke();
            }
        }

        /// <summary>
        /// The Logout
        /// </summary>
        public void Logout()
        {
            if (!User.isAnonym || User.id == null)
            {
                PlayerPrefs.SetString("refreshToken", "");
            }

            User = new User();
            IdToken = null;
            PlayerPrefs.SetInt("login", 0);
            loggedOut?.Invoke();
        }
    }
}
