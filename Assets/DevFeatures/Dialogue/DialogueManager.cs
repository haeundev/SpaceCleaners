using System;
using DataTables;
using Febucci.UI;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevFeatures.Dialogue
{
    [Serializable]
    public class SpriteBySpeakerType : SerializableDictionary<SpeakerType, Sprite> {}
    [Serializable]
    public class AudioBySpeakerType : SerializableDictionary<SpeakerType, string> {}
    
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;
        [SerializeField] private Button buttonOptionA;
        [SerializeField] private Button buttonOptionB;
        [SerializeField] private TextMeshProUGUI tmpOptionA;
        [SerializeField] private TextMeshProUGUI tmpOptionB;
        [SerializeField] private TextMeshProUGUI tmpSpeakerName;
        [SerializeField] private TextMeshProUGUI tmpLine;
        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private Button buttonNext;
        private DialogueInfo _currentDialogue;
        public bool dialogueFinished;
        [SerializeField] private SpriteBySpeakerType speakerSpriteDict;
        [SerializeField] private AudioBySpeakerType speakerAudioDict;
        [SerializeField] private Image speakerImage;
        
        private void Awake()
        {
            Instance = this;
            
            TaskManager.Instance.OnDialogueTaskInit += OnDialogueTaskInit;
      
            //typewriter.onTypewriterStart.AddListener(OnLineStart);
            typewriter.onTextShowed.AddListener(OnLineEnd);
            typewriter.onTextForceStopped.AddListener(OnLineEnd);
            buttonNext.onClick.AddListener(OnNextButtonPressed);
            buttonOptionA.onClick.AddListener(OnPressOptionA);
            buttonOptionB.onClick.AddListener(OnPressOptionB);
            
            TaskManager.Instance.dialogueUI = gameObject;
        }

        private void DisableOptionButtons()
        {
            buttonOptionA.gameObject.SetActive(false);
            buttonOptionB.gameObject.SetActive(false);
        }
        
        [Sirenix.OdinInspector.Button]
        private void OnPressOptionA()
        {
            if (_currentDialogue == default || _currentDialogue.HasChoice == false)
                return;
            
            PlayLine(_currentDialogue.NextForA);
            DisableOptionButtons();
        }
        
        [Sirenix.OdinInspector.Button]
        private void OnPressOptionB()
        {
            if (_currentDialogue == default || _currentDialogue.HasChoice == false)
                return;
            
            PlayLine(_currentDialogue.NextForB);
            DisableOptionButtons();
        }

        [Sirenix.OdinInspector.Button]
        private void OnNextButtonPressed()
        {
            if (_currentDialogue == default || _currentDialogue.HasChoice
                || TaskManager.Instance.taskCompleteUI.activeSelf 
                || TaskManager.Instance.levelUpUI.activeSelf 
                )
                return;
            
            var next = _currentDialogue?.Next ?? 0;
            if (next != default && next != 0)
            {
                // has next line
                PlayLine(next);
            }
            else if (_currentDialogue?.Next is 0 or null)
            {
                OnDialogueFinished();
            }
        }

        private void OnDialogueFinished()
        {
            dialogueFinished = true;
        }
        
        private void OnLineStart()
        {
            
        }
    
        private void OnLineEnd()
        {
            if (_currentDialogue == default)
            {
                Debug.Log("Current dialogue is null.");
                return;
            }
            buttonOptionA.gameObject.SetActive(_currentDialogue.HasChoice);
            buttonOptionB.gameObject.SetActive(_currentDialogue.HasChoice);
            
            // if (_currentDialogue?.Next == default)
            // {
            //     TaskManager.Instance.CompleteCurrentTask();
            // }
        }
        
        private void OnDialogueTaskInit(TaskInfo taskInfo)
        {
            dialogueFinished = false;
            PlayLine(taskInfo.ValueInt); // dialogue ID.
        }
        
        private void PlayLine(int dialogueID)
        {
            Debug.Log($"PlayLine {dialogueID}");
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }
            _currentDialogue = DataTableManager.DialogueInfos.Find(dialogueID);
            if (_currentDialogue == default)
            {
                Debug.LogError($"Dialogue ID {dialogueID} not found.");
                return;
            }
            tmpLine.SetText(_currentDialogue.Line);
            tmpSpeakerName.SetText(_currentDialogue.DisplayName);
            
            typewriter.GetComponent<TypewriterSoundPlayer>().SetSFX(speakerAudioDict[_currentDialogue.SpeakerType]);
            speakerImage.sprite = speakerSpriteDict[_currentDialogue.SpeakerType];
            speakerImage.SetNativeSize();
            
            // toggle options
            buttonNext.gameObject.SetActive(!_currentDialogue.HasChoice);
          
            tmpOptionA.SetText(_currentDialogue.ResponseA);
            tmpOptionB.SetText(_currentDialogue.ResponseB);
            
            tmpLine.gameObject.GetComponent<TypewriterByCharacter>().StartShowingText(true);
            OnLineStart();
        }
    }
}