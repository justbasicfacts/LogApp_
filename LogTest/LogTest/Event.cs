﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventLogProject
{
    public class Event : EventLog
    {
        [BsonIgnore]
        private readonly long _startTimeStamp;
        [BsonIgnore]
        private long _endTimeStamp;

        public Event() { }
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

    }
}
