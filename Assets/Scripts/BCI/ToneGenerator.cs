using UnityEngine;

namespace BCI
{
    public static class ToneGenerator
    {
        public static AudioClip CreateTone(string name, float frequency, float duration, float volume = 0.5f, int sampleRate = 44100)
        {
            int sampleCount = Mathf.CeilToInt(duration * sampleRate);
            var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);

            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume;
            }

            clip.SetData(samples, 0);
            return clip;
        }
    }
}