
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This file Manage which animation and recorder play for event
/// </summary>
public class GameManager : MonoBehaviour
{
    public Animator smileAnimator;
    public TalkMovement talkMovementManage;
    [SerializeField] private Button smilePlayButton;
    [SerializeField] private Button recordPlayButton;
    [SerializeField] private Button changeSoundPlayButton;
    [SerializeField] bool isSmileAnimationRunning; 
    void Start()
    {
        smilePlayButton.onClick.RemoveListener(StartSmileAnimationEvent);
        smilePlayButton.onClick.AddListener(StartSmileAnimationEvent);
       

        recordPlayButton.onClick.RemoveListener(StartRecordVoice);
        recordPlayButton.onClick.AddListener(StartRecordVoice);

        changeSoundPlayButton.onClick.RemoveListener(ChangeRecourSoundVoice);
        changeSoundPlayButton.onClick.AddListener(ChangeRecourSoundVoice);


        StartSmileAnimationEvent();
    }
    public void StartSmileAnimationEvent()
    {
        if (isSmileAnimationRunning) return;

        isSmileAnimationRunning = true;
        talkMovementManage.StopTalking();
        StartCoroutine(StartSmileAnimation());
    }
    IEnumerator StartSmileAnimation()
    {       
        smileAnimator.enabled = true;
        smileAnimator.SetBool("PlaySeq", true);
        smileAnimator.Play("Ideal", 0, 0f);    
        yield return new WaitForSeconds(4.0f);
        smileAnimator.SetBool("PlaySeq", false);
        yield return new WaitForSeconds(4f);
         smileAnimator.enabled = false;    
        isSmileAnimationRunning = false;
    }
    public void StartRecordVoice()
    {
        if (isSmileAnimationRunning) return;
        smileAnimator.enabled = false;
        talkMovementManage.StartTalking();
    }
    public void ChangeRecourSoundVoice()
    {
        if (isSmileAnimationRunning) return;

        talkMovementManage.ChangeSoundContain();
    }
}
