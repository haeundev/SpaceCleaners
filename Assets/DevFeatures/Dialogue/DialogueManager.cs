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
        }

        [Sirenix.OdinInspector.Button]
        private void OnNextButtonPressed()
        {
            if (_currentDialogue?.Next != default)
            {
                PlayLine(_currentDialogue.Next);
            }
        }

        private void OnLineStart()
        {
            
        }
    
        private void OnLineEnd()
        {
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
            tmpLine.gameObject.GetComponent<TypewriterByCharacter>().StartShowingText();
            OnLineStart();
        }
    }
}