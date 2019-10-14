using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventLogProject
{
    public class Event
    {
        [BsonIgnore]
        private readonly long _startTimeStamp;
        [BsonIgnore]
        private long _endTimeStamp;

        public bool Alert { get; set; }
        public Event() : base() { }
        public Event(EventLog log) : base()
        {
            if (State == EventState.STARTED)
            {
                _startTimeStamp = log.TimeStamp;
            }

            if (State == EventState.FINISHED)
            {
                _endTimeStamp = log.TimeStamp;
            }

            Host = log.Host;
            ID = log.ID;
            State = log.State;
            Type = log.Type;
        }

        public long GetDuration()
        {
            long retVal = 0;
            if (State == EventState.FINISHED)
            {
                retVal = _endTimeStamp - _startTimeStamp;
            }

            return retVal;
        }

        public void SetEndDate(long timeStamp)
        {
            _endTimeStamp = timeStamp;
        }

        public void SetAsFinished()
        {
            this.State = EventState.FINISHED;
        }

        public bool IsValidEvent()
        {
            return _startTimeStamp > 0 && _endTimeStamp > 0 && _endTimeStamp > _startTimeStamp;
        }

        public string ID { get; set; }
        public EventState State { get; set; }
        public EventType Type { get; set; }
        public long TimeStamp { get; set; }
        public string Host { get; set; }


    }
}
