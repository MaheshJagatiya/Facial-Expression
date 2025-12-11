using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class VoiceManager : MonoBehaviour
{
    /// <summary>
    /// manage Star animation button event
    /// </summary>
    public Animator smileAnimator;
    public TalkMovement talkMovementManage;
    [SerializeField] private Button faceexPlayButton;
    [SerializeField] private bool isSmileAnimationRunning;
    private void Start()
    {
        faceexPlayButton.onClick.RemoveListener(StartAnimation);
        faceexPlayButton.onClick.AddListener(StartAnimation);
       
    }
    public void StartAnimation()
    {
        StartSmileAnimationEvent();
    }
    public void StartSmileAnimationEvent()
    {
       if (isSmileAnimationRunning) return;

        isSmileAnimationRunning = true;
        talkMovementManage.StopTalking();
    

        StartCoroutine(StartSmileAnimation());
        StartRecordVoice();
    }
    IEnumerator StartSmileAnimation()
    {

        smileAnimator.enabled = true;
      
        smileAnimator.Play("Smile", 0, 0f);
        yield return new WaitForSeconds(16.0f);        
        isSmileAnimationRunning = false;
    }
    public void StartRecordVoice()
    {    
        talkMovementManage.StartTalking();
    }
}
