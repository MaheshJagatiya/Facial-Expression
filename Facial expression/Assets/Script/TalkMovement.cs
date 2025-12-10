
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkMovement : MonoBehaviour
{
    [Header("Assign Face SkinnedMeshRenderer here")]
    public SkinnedMeshRenderer skinnedMesh;

    [Header("Mouth movement settings")]
    [Range(0f, 100f)] public float maxWeight = 60f;
    public float openCloseLerpSpeed = 15f;     // how fast mouth follows audio
    public string mouthOpenBlendShapeName = "MouthOpen"; // or Jaw_Down, etc.

    [Header("Eye settings (unchanged)")]
    public int[] eyeBlink;
    public AudioClip[] soundClip;

    public AudioSource voiceAudioSource;

    int mouthOpenIndex = -1;
    float currentMouthWeight = 0f;
    bool isTalking = false;
    bool isFirstSound = false;
    Coroutine eyeRoutine;
    Coroutine faceExpRoutine;
    Animator facexpresionAnimator;
   
    float[] audioSamples = new float[1024];

    void Awake()
    {
        CacheMouthShapeIndex();
    }

    void CacheMouthShapeIndex()
    {
        var mesh = skinnedMesh.sharedMesh;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            string name = mesh.GetBlendShapeName(i);
            if (name.Equals(mouthOpenBlendShapeName, System.StringComparison.OrdinalIgnoreCase))
            {
                mouthOpenIndex = i;
                break;
            }
        }

        if (mouthOpenIndex == -1)
        {
            Debug.LogWarning("Mouth blendshape not found: " + mouthOpenBlendShapeName);
        }
    }

    void Update()
    {
        if (!isTalking || mouthOpenIndex == -1)
            return;

        // Read audio data
        if (!voiceAudioSource.isPlaying)
        {
            // Audio finished → close mouth gradually
            currentMouthWeight = Mathf.Lerp(currentMouthWeight, 0f, Time.deltaTime * openCloseLerpSpeed);
        }
        else
        {
            voiceAudioSource.GetOutputData(audioSamples, 0);

            // Compute loudness (RMS)
            float sum = 0f;
            for (int i = 0; i < audioSamples.Length; i++)
            {
                float vValue = audioSamples[i];
                sum += vValue * vValue;
            }
            float rms = Mathf.Sqrt(sum / audioSamples.Length); // 0–1 approx

            //  Convert to target mouth weight
            // You can adjust the multiplier (for sensitivity)
            float targetWeight = Mathf.Clamp01(rms * 15f) * maxWeight;

            // Smooth follow
            currentMouthWeight = Mathf.Lerp(currentMouthWeight, targetWeight, Time.deltaTime * openCloseLerpSpeed);
        }

        // Apply to blendshape
        skinnedMesh.SetBlendShapeWeight(mouthOpenIndex, currentMouthWeight);
    }

    public void StartTalking(Animator animatorFacialEx)
    {
        facexpresionAnimator = animatorFacialEx;
        if (eyeRoutine != null)
            StopCoroutine(eyeRoutine);

        if (faceExpRoutine != null)
            StopCoroutine(faceExpRoutine);
        

        eyeRoutine = StartCoroutine(BlinkEye());
        faceExpRoutine = StartCoroutine(PlayRandomFaceAnimation());

        isTalking = true;
        currentMouthWeight = 0f;
        if (!voiceAudioSource.isPlaying)
            voiceAudioSource.Play();
    }

    public void StopTalking()
    {
        isTalking = false;
      
        currentMouthWeight = 0f;
        if (mouthOpenIndex != -1)
            skinnedMesh.SetBlendShapeWeight(mouthOpenIndex, 0f);
      
        if (eyeRoutine != null)
            StopCoroutine(eyeRoutine);

        if (faceExpRoutine != null)
            StopCoroutine(faceExpRoutine);

        foreach (int i in eyeBlink)
            skinnedMesh.SetBlendShapeWeight(i, 0f);

        voiceAudioSource.Stop();
    }
    IEnumerator BlinkEye()
    {
       
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
            if (eyeBlink.Length == 0)
                yield break;

            foreach (int i in eyeBlink)
                skinnedMesh.SetBlendShapeWeight(i, 0f);

            float target = 100;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * openCloseLerpSpeed;
                float w = Mathf.Lerp(0, target, t);
                skinnedMesh.SetBlendShapeWeight(0, w);
                skinnedMesh.SetBlendShapeWeight(1, w);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * openCloseLerpSpeed;
                float w = Mathf.Lerp(target, 0, t);
                skinnedMesh.SetBlendShapeWeight(0, w);
                skinnedMesh.SetBlendShapeWeight(1, w);
                yield return null;
            }
        }
    }

    public void ChangeSoundContain()
    {
        StopTalking();
        if (isFirstSound)
            voiceAudioSource.clip = soundClip[0];
        else
            voiceAudioSource.clip = soundClip[1];
        isFirstSound = !isFirstSound;

        StartTalking(facexpresionAnimator);
    }
    IEnumerator PlayRandomFaceAnimation()
    {
        while (true)
        {
            int r = Random.Range(1, 7); // 1 to 6
            facexpresionAnimator.Play("Face" + r, 0, 0f); // plays animation from start

            yield return new WaitForSeconds(Random.Range(2, 6));
        }
    }
}
