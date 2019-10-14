using LiteDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EventLogProject
{
    public class EventLogsFileProcessor
    {
        public EventLogsFileProcessor(string path)
        {
            _path = path;
            _eventLogDictionary = new ConcurrentDictionary<string, Event>();
        }

        private readonly string _path;
        private ConcurrentDictionary<string, Event> _eventLogDictionary;

        public async void ProcessFile()
        {
            using (LiteDatabase db = new LiteDatabase(@"Events.db"))
            {
                var events = db.GetCollection<Event>("Events");
                using (StreamReader streamReader = new StreamReader(_path))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = await streamReader.ReadLineAsync();
                        var logFile = GetLogFileObject(line);
                        if (events.Count(e => e.ID == logFile.ID) == 0)
                        {
                            var currentEvent = await Task.Run(() => ProcessEvent(logFile));
                            if (currentEvent.State == EventState.FINISHED && currentEvent.IsValidEvent() && currentEvent.GetDuration() > 4)
                            {
                                currentEvent.Alert = true;
                                events.Insert(currentEvent);
                            }
                        }
                    }
                }

                db.Commit();
            }
        }

        private Event ProcessEvent(EventLog currentEventLog)
        {
            try
            {
                var currentEvent = BuildEvent(currentEventLog);
                _eventLogDictionary.AddOrUpdate(currentEventLog.ID, currentEvent, (key, value) => value = currentEvent);
                return currentEvent;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private Event BuildEvent(EventLog currentEventLog)
        {
            Event retVal = null;
            if (currentEventLog.State == EventState.STARTED)
            {
                retVal = new Event(currentEventLog);
            }
            else if (currentEventLog.State == EventState.FINISHED)
            {
                retVal = GetEventByID(currentEventLog.ID);
                if (retVal != null)
                {
                    retVal.SetAsFinished();
                    retVal.SetEndDate(currentEventLog.TimeStamp);
                }
                else
                {
                    retVal = new Event(currentEventLog);
                }
            }

            return retVal;
        }

        private Event GetEventByID(string id)
        {
            _eventLogDictionary.TryGetValue(id, out Event @event);
            return @event;
        }

        private EventLog GetLogFileObject(string line)
        {
            return JsonSerializer.Deserialize<EventLog>(line);
        }
    }
}
