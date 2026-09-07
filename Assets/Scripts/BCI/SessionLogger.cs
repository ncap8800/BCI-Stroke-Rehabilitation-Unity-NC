using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace BCI
{
    public class SessionLogger : MonoBehaviour
    {
        private StreamWriter writer;
        private string filePath;

        void Start()
        {
            string fileName = $"session_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
            filePath = Path.Combine(Application.persistentDataPath, fileName);
            writer = new StreamWriter(filePath, append: false);
            writer.WriteLine("trial_index,cue_time,imagery_start_time,class_label_received,reaction_time,hit");
            writer.Flush();
            Debug.Log($"SessionLogger: writing to {filePath}");

            if (TrialManager.Instance != null)
                TrialManager.Instance.OnTrialEnd += HandleTrialEnd;
            else
                Debug.LogError("SessionLogger: no TrialManager found in the scene.");
        }

        void HandleTrialEnd(TrialResult result)
        {
            string label = string.IsNullOrEmpty(result.rawMarkerValue) ? "none" : result.rawMarkerValue;
            string reactionTime = result.reactionTime >= 0f ? result.reactionTime.ToString("F3", CultureInfo.InvariantCulture) : "";
            writer.WriteLine(string.Join(",",
                result.trialIndex.ToString(CultureInfo.InvariantCulture),
                result.cueTime.ToString("F3", CultureInfo.InvariantCulture),
                result.imageryStartTime.ToString("F3", CultureInfo.InvariantCulture),
                label,
                reactionTime,
                result.hit ? "1" : "0"));
            writer.Flush();
        }

        void OnDestroy()
        {
            if (TrialManager.Instance != null)
                TrialManager.Instance.OnTrialEnd -= HandleTrialEnd;
            CloseWriter();
        }

        void OnApplicationQuit()
        {
            CloseWriter();
        }

        void CloseWriter()
        {
            if (writer == null)
                return;
            writer.Flush();
            writer.Close();
            writer = null;
        }
       
    }
}