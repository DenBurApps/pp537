using System;
using UnityEngine;

namespace AssigmentData
{
    [Serializable]
    public class AssigmentData
    {
        public string Name;
        public string Subject;
        public string Note;
        public string Date;
        public int TimeHr;
        public int TimeMin;

        public AssigmentData(string name, string subject, string note, string date, int timeHr, int timeMin)
        {
            Name = name;
            Subject = subject;
            Note = note;
            Date = date;
            TimeHr = timeHr;
            TimeMin = timeMin;
        }
    }
    
    
}
