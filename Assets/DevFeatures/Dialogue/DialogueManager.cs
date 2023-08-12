using DataTables;
using Febucci.UI;
using LiveLarson.DataTableManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevFeatures.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private Button buttonOptionA;
        [SerializeField] private Button buttonOptionB;
        [SerializeField] private TextMeshProUGUI tmpOptionA;
        [SerializeField] private TextMeshProUGUI tmpOptionB;
        [SerializeField] private TextMeshProUGUI tmpSpeakerName;
        [SerializeField] private TextMeshProUGUI tmpLine;
        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private Button buttonNext;
        private DialogueInfo _currentDialogue;

        private void Awake()
        {
            TaskManager.Instance.OnDialogueTaskInit += OnDialogueTaskInit;
      
            //typewriter.onTypewriterStart.AddListener(OnLineStart);
            typewriter.onTextShowed.AddListener(OnLineEnd);
            typewriter.onTextForceStopped.AddListener(OnLineEnd);
            buttonNext.onClick.AddListener(OnNextButtonPressed);
            buttonOptionA.onClick.AddListener(OnPressOptionA);
            buttonOptionB.onClick.AddListener(OnPressOptionB);
            
            // tween buttons
            
        }

        [Sirenix.OdinInspector.Button]
        private void OnPressOptionA()
        {
            PlayLine(_currentDialogue.NextForA);
            DisableOptionButtons();
        }

        private void DisableOptionButtons()
        {
            buttonOptionA.gameObject.SetActive(false);
            buttonOptionB.gameObject.SetActive(false);
        }

        [Sirenix.OdinInspector.Button]
        private void OnPressOptionB()
        {
            PlayLine(_currentDialogue.NextForB);
            DisableOptionButtons();
        }

        [Sirenix.OdinInspector.Button]
        private void OnNextButtonPressed()
        {
            var next = _currentDialogue?.Next ?? 0;
            if (next != default && next != 0)
            {
                PlayLine(next);
            }
            else
            {
                OnDialogueFinished();
            }
        }

        private void OnDialogueFinished()
        {
            TaskManager.Instance.CompleteCurrentTask();
        }

        private void ShowPlayerOptions()
        {
            
        }

        private void OnLineStart()
        {
            
        }
    
        private void OnLineEnd()
        {
            buttonOptionA.gameObject.SetActive(_currentDialogue.HasChoice);
            buttonOptionB.gameObject.SetActive(_currentDialogue.HasChoice);
            
            if (_currentDialogue?.Next == default)
            {
                TaskManager.Instance.CompleteCurrentTask();
            }
        }
        
        private void OnDialogueTaskInit(TaskInfo taskInfo)
        {
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
            
            // toggle options
            buttonNext.gameObject.SetActive(!_currentDialogue.HasChoice);
          
            tmpOptionA.SetText(_currentDialogue.ResponseA);
            tmpOptionB.SetText(_currentDialogue.ResponseB);
            
            tmpLine.gameObject.GetComponent<TypewriterByCharacter>().StartShowingText(true);
            OnLineStart();
        }
    }
}