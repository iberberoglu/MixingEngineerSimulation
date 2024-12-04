using UnityEngine;
using FMOD.Studio;
using FMOD;
using System.Collections.Generic;

public class MeteringDisplay : MonoBehaviour
{
    [SerializeField] private MixerControl mixerControl;

    private readonly ChannelGroup[] channelGroups = new ChannelGroup[4];
    private readonly DSP[] dsps = new DSP[4];
    private readonly Queue<float>[] peakValueQueues = new Queue<float>[4];

    private const int MeteringBufferLength = 10;

    [SerializeField] private Sprite[] levelSprites; // Assign 15 sprites here.
    [SerializeField] private UnityEngine.UI.Image[] images; // Assign the 4 images here.

    private void Start()
    {
        // Initialize the peak value queues
        for (int i = 0; i < peakValueQueues.Length; i++)
        {
            peakValueQueues[i] = new Queue<float>();
        }
    }

    private void Update()
    {
        // Ensure ChannelGroup and DSP for all tracks
        EnsureChannelGroupAndDSP(mixerControl.track1, ref channelGroups[0], ref dsps[0]);
        EnsureChannelGroupAndDSP(mixerControl.track2, ref channelGroups[1], ref dsps[1]);
        EnsureChannelGroupAndDSP(mixerControl.track3, ref channelGroups[2], ref dsps[2]);
        EnsureChannelGroupAndDSP(mixerControl.track4, ref channelGroups[3], ref dsps[3]);

        // Update dB levels and corresponding sprites
        for (int i = 0; i < dsps.Length; i++)
        {
            float dbLevel = GetSmoothedPeakLevel(dsps[i], peakValueQueues[i]);
            images[i].sprite = GetSpriteForDb(dbLevel);
        }
    }

    private void EnsureChannelGroupAndDSP(EventInstance track, ref ChannelGroup channelGroup, ref DSP dsp)
    {
        if (track.isValid())
        {
            if (!channelGroup.hasHandle())
            {
                track.getChannelGroup(out channelGroup);
            }

            if (channelGroup.hasHandle() && !dsp.hasHandle())
            {
                channelGroup.getDSP(0, out dsp);
                if (dsp.hasHandle())
                {
                    dsp.setMeteringEnabled(true, true);
                }
            }
        }
    }

    private float GetSmoothedPeakLevel(DSP dsp, Queue<float> peakValues)
    {
        if (dsp.hasHandle())
        {
            FMOD.DSP_METERING_INFO meteringInfo = new FMOD.DSP_METERING_INFO();
            dsp.getMeteringInfo(out _, out meteringInfo);

            float currentPeakLevel = meteringInfo.peaklevel.Length > 0 ? meteringInfo.peaklevel[0] : 0f;
            float currentPeakDb = 20f * Mathf.Log10(Mathf.Max(currentPeakLevel, 0.0001f));

            if (peakValues.Count >= MeteringBufferLength)
            {
                peakValues.Dequeue();
            }
            peakValues.Enqueue(currentPeakDb);

            float smoothedPeakLevel = 0f;
            foreach (float value in peakValues)
            {
                smoothedPeakLevel += value;
            }
            return smoothedPeakLevel / peakValues.Count;
        }
        return -80f; // Minimum dB value
    }

    private Sprite GetSpriteForDb(float dbValue)
    {
        int spriteIndex = Mathf.Clamp(
            Mathf.RoundToInt((dbValue + 80f) / (90f / (levelSprites.Length - 1))),
            0,
            levelSprites.Length - 1
        );

        return levelSprites[spriteIndex];
    }
}
