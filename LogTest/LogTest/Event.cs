using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogApp
{
    public class Event
    {
        [BsonIgnore]
        private readonly EventLog _startLog;
        [BsonIgnore]
        private readonly EventLog _endLog;

        public Event() { }
        public Event(EventLog startLog, EventLog endLog)
        {
            _startLog = startLog;
            _endLog = endLog;

            EventID = startLog.ID;
            Host = startLog.Host;
            EventType = startLog.Type;
            Duration = endLog.TimeStamp - startLog.TimeStamp;
        }

        public long Duration { get; set; }
        public string Host { get; set; }
        public EventType EventType { get; set; }
        [BsonId]
        public string EventID { get; set; }
        public bool IsValid()
        {
            return this._startLog != null && this._endLog != null && this._endLog.TimeStamp - this._startLog.TimeStamp < 4;
        }


    }
}
