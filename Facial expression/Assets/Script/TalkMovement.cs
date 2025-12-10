
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkMovement : MonoBehaviour
{
    /// <summary>
    /// this file code manage lip movement and eye movement of character
    /// </summary>
    [Header("Assign Face SkinnedMeshRenderer here")]
    public SkinnedMeshRenderer skinnedMesh;

    [Header("Mouth movement settings")]
    [Range(0f, 100f)] public float maxWeight = 60f;
    public float lerpSpeed = 15f;
    public Vector2 holdTimeRange = new Vector2(0.05f, 0.12f);

    public AudioSource voiceAudioSource;

    int[] mouthShapeIndices;
    public int[] eyeBlink;
    Coroutine talkRoutine;
    Coroutine eyeRoutine;

  
    void Awake()
    {
        CacheMouthShapes();    
    }

    void CacheMouthShapes()
    {
        var mesh = skinnedMesh.sharedMesh;
        var list = new List<int>();

        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            string name = mesh.GetBlendShapeName(i);

            // Sirf Mouth se start hone wale shapes lo
            if (!name.StartsWith("Mouth", System.StringComparison.OrdinalIgnoreCase))
                continue;

            // Skip Naarow 
            if (name.Contains("Narrow"))
                continue;

            list.Add(i);
            //Add more moth open movement
            if(name.Contains("MouthOpen"))
            {
                list.Add(i);
                list.Add(i);
            }
        }
        mouthShapeIndices = list.ToArray();       
    }

    public void StartTalking()
    {
        if (talkRoutine != null)
            StopCoroutine(talkRoutine);

        if (eyeRoutine != null)
            StopCoroutine(eyeRoutine);

        talkRoutine = StartCoroutine(TalkRoutine());
        eyeRoutine = StartCoroutine(BlinkEye());
        voiceAudioSource.Play();
    }

    public void StopTalking()
    {
        if (talkRoutine != null)
            StopCoroutine(talkRoutine);

        if (eyeRoutine != null)
            StopCoroutine(eyeRoutine);

        // sab mouth shapes reset
        foreach (int index in mouthShapeIndices)
            skinnedMesh.SetBlendShapeWeight(index, 0f);

        skinnedMesh.SetBlendShapeWeight(0, 0f);
        skinnedMesh.SetBlendShapeWeight(1, 0f);

        voiceAudioSource.Stop();
    }

    IEnumerator TalkRoutine()
    {
        while (true)
        {
            if (mouthShapeIndices.Length == 0)
                yield break;

            // 1) random mouth shape choose
            int index = mouthShapeIndices[Random.Range(0, mouthShapeIndices.Length)];

            // 2) sab mouth shapes first zero
            foreach (int i in mouthShapeIndices)
                skinnedMesh.SetBlendShapeWeight(i, 0f);

            float target = Random.Range(maxWeight * 0.4f, maxWeight);

            // 3) open
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                float w = Mathf.Lerp(0f, target, t);
                skinnedMesh.SetBlendShapeWeight(index, w);
                yield return null;
            }

            // 4) few time hold
            yield return new WaitForSeconds(Random.Range(holdTimeRange.x, holdTimeRange.y));

            // 5) close
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                float w = Mathf.Lerp(target, 0f, t);
                skinnedMesh.SetBlendShapeWeight(index, w);
                yield return null;
            }
        }
    }
    IEnumerator BlinkEye()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
            if (eyeBlink.Length == 0)
                yield break;          

            // 1) Every mouth shape zero first
            foreach (int i in eyeBlink)
                skinnedMesh.SetBlendShapeWeight(i, 0f);

            float target = 100;

            // 3) open
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                float w = Mathf.Lerp(0f, target, t);
                skinnedMesh.SetBlendShapeWeight(0, w);
                skinnedMesh.SetBlendShapeWeight(1, w);
                yield return null;
            }

            // 4) some hold
            yield return new WaitForSeconds(0.1f);

            // 5) close
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                float w = Mathf.Lerp(target, 0f, t);
                skinnedMesh.SetBlendShapeWeight(0, w);
                skinnedMesh.SetBlendShapeWeight(1, w);
                yield return null;
            }
        }
    }
}
