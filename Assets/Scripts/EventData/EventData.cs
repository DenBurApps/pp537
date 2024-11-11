using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventData
{
    [Serializable]
    public class EventData
    {
        public int TimeHr;
        public int TimeMin;
        public string Date;
        public string Name;
        public string Note;
        public int DurationHr;
        public int DurationMin;
        public ExamData ExamData;
        public bool isCompleted;

        public EventData(int timeHr, int timeMin, string date, string name, string note, int durationHr, int durationMin)
        {
            TimeHr = timeHr;
            TimeMin = timeMin;
            Date = date;
            Name = name;
            Note = note;
            DurationHr = durationHr;
            DurationMin = durationMin;
        }
    }

    [Serializable]
    public class ExamData
    {
        public List<ExamStep> Steps;

        public float GetCompletionPercentage()
        {
            if (Steps == null || Steps.Count == 0)
                return 0;

            int completedCount = 0;
            foreach (var step in Steps)
            {
                if (step.IsCompleted)
                    completedCount++;
            }

            return (float)completedCount / Steps.Count * 100;
        }
    }

    [Serializable]
    public class ExamStep
    {
        public string Name;
        public bool IsCompleted;
    }
}
