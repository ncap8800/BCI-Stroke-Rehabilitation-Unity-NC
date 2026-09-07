using System;
using UnityEngine;
using LSL;

namespace BCI
{

    public enum MarkerLabel { Action, Rest, Ignore }

    [Serializable]
    public class MarkerMapping
    {
        public string rawValue;
        public MarkerLabel label = MarkerLabel.Ignore;
    }
    public class LSLInlet : MonoBehaviour
    {
        public static LSLInlet Instance { get; private set; }

        [Header("Stream lookup")]
        public string streamType = "Markers";

        [Header("Marker mapping (raw value -> logical label)")]
        public MarkerMapping[] mappings = new MarkerMapping[]
        {
            new MarkerMapping{ rawValue = "action", label = MarkerLabel.Action},
            new MarkerMapping{ rawValue = "33025", label = MarkerLabel.Action},
            new MarkerMapping{ rawValue = "rest", label = MarkerLabel.Rest},
            new MarkerMapping{ rawValue = "33024", label = MarkerLabel.Rest},
        };

        public event Action<MarkerLabel, string> OnMarkerReceived;

        private ContinuousResolver resolver;
        private StreamInlet inlet;

        private readonly float[] sampleBuffer = new float[1];

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            resolver = new ContinuousResolver("type", streamType);
        }

        void Update()
        {
            if (inlet == null)
            {
                TryResolve();
                return;
            }

            double timestamp;
            while ((timestamp = inlet.pull_sample(sampleBuffer, 0.0)) != 0.0)
            {
                string raw = sampleBuffer[0].ToString();
                MarkerLabel label = MapRaw(raw);
                Debug.Log($"LSLInlet: received '{raw}' -> mapped to {label}");
                OnMarkerReceived?.Invoke(label, raw);
            }
        }

        void TryResolve()
        {
            var results = resolver.results();
            if (results.Length > 0)
            {
                inlet = new StreamInlet(results[0]);
                Debug.Log($"LSLInlet: connected to stream '{results[0].name()}' (type '{results[0].type()}', " +
            $"channel_count {results[0].channel_count()}, channel_format {results[0].channel_format()})");
            }
        }

        MarkerLabel MapRaw(string raw)
        {
            foreach (var m in mappings)
            {
                if (string.Equals(m.rawValue, raw, StringComparison.OrdinalIgnoreCase))
                    return m.label;
            }
            return MarkerLabel.Ignore;
        }
    }
}