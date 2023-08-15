using System;
using System.Collections;
using DataTables;
using Febucci.UI;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using LiveLarson.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private GameObject visuals;
        [SerializeField] private Button buttonOptionA;
        [SerializeField] private Button buttonOptionB;
        [SerializeField] private TextMeshProUGUI tmpOptionA;
        [SerializeField] private TextMeshProUGUI tmpOptionB;
        [SerializeField] private TextMeshProUGUI tmpSpeakerName;
        [SerializeField] private TextMeshProUGUI tmpLine;
        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private Button buttonNext;
        private DialogueInfo _currentDialogue;
        public bool playedDialogueInScene = false;
        public bool dialogueFinished = false;
        [SerializeField] private SpriteBySpeakerType speakerSpriteDict;
        [SerializeField] private AudioBySpeakerType speakerAudioDict;
        [SerializeField] private Image speakerImage;
        
        private void Awake()
        {
            Instance = this;
            
            TaskManager.Instance.OnInitTask += OnInitTask;
            //typewriter.onTypewriterStart.AddListener(OnLineStart);
            typewriter.onTextShowed.AddListener(OnLineEnd);
            typewriter.onTextForceStopped.AddListener(OnLineEnd);
            buttonNext.onClick.AddListener(OnNextButtonPressed);
            buttonOptionA.onClick.AddListener(OnPressOptionA);
            buttonOptionB.onClick.AddListener(OnPressOptionB);
            
            TaskManager.Instance.dialogueUI = gameObject;
            
            visuals.SetActive(false);
        }

        private void OnInitTask(TaskInfo taskInfo)
        {
            dialogueFinished = false;
            if (taskInfo.TaskType == TaskType.Dialogue)
            {
                PlayLine(taskInfo.ValueInt); // dialogue ID.
            }
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
                || (TaskManager.Instance.taskCompleteUI != default && TaskManager.Instance.taskCompleteUI.activeSelf )
                || (TaskManager.Instance.levelUpUI != default && TaskManager.Instance.levelUpUI.activeSelf )
               )
                return;
            
            var next = _currentDialogue?.Next ?? 0;
            if (next != default && next != 0)
            {
                // has next line
                PlayLine(next);
            }
            else if (_currentDialogue?.IsEnd == true)
            {
                TriggerDialogueFinish();
            }
            else if (_currentDialogue?.Next is 0 or null)
            {
                Debug.LogError($"why next Dialogue is null?");
            }
        }

        private void TriggerDialogueFinish()
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
        
        private void PlayLine(int dialogueID)
        {
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);
            StartCoroutine(CoPlayLine(dialogueID));
            playedDialogueInScene = true;
        }

        private IEnumerator CoPlayLine(int dialogueID)
        {
            yield return YieldInstructionCache.WaitUntil(() =>
            {
                var hasOtherUIOn = (TaskManager.Instance.taskCompleteUI != default &&
                                    TaskManager.Instance.taskCompleteUI.activeSelf)
                                   || (TaskManager.Instance.levelUpUI != default &&
                                       TaskManager.Instance.levelUpUI.activeSelf);
                return hasOtherUIOn == false;
            });
            
            visuals.SetActive(true);
            Debug.Log($"PlayLine {dialogueID}");
            _currentDialogue = DataTableManager.DialogueInfos.Find(dialogueID);
            if (_currentDialogue == default)
            {
                Debug.LogError($"Dialogue ID {dialogueID} not found.");
                yield break;
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