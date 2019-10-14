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
            _eventlogDictinoary = new ConcurrentDictionary<string, Event>();
        }

        private readonly string _path;
        private ConcurrentDictionary<string, Event> _eventlogDictinoary;
        private EventLogProcessor eventLogProcessor = new EventLogProcessor();

        public void ProcessLogs(bool isAsync = false)
        {
            ProcessFile(isAsync);
        }
        private void ProcessFile(bool isAsync)
        {
            using (LiteDatabase db = new LiteDatabase(@"Events.db"))
            {
                var events = db.GetCollection<Event>("Events");
                using (StreamReader streamReader = new StreamReader(_path))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        var logFile = GetLogFileObject(line);
                        ProcessEvent(logFile);
                    }
                }
                var filtered = _eventlogDictinoary.Where(t => t.Value.IsValidEvent()).Select(t => t.Value);
                Parallel.ForEach(filtered, new ParallelOptions { MaxDegreeOfParallelism = 8, CancellationToken = new CancellationToken() }, current =>
                {
                    lock (events)
                    {
                        events.Insert(current);
                    }
                });
            }
        }

        private Event ProcessAsync(EventLog currentEventLog)
        {
            return ProcessEvent(currentEventLog);
        }

        private Event ProcessEvent(EventLog currentEventLog)
        {
            try
            {
                var currentEvent = BuildEvent(currentEventLog);
                _eventlogDictinoary.AddOrUpdate(currentEventLog.ID, currentEvent, (key, value) => value = currentEvent);
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

            _eventlogDictinoary.TryGetValue(id, out Event @event);

            return @event;
        }

        private EventLog GetLogFileObject(string line)
        {
            return JsonSerializer.Deserialize<EventLog>(line);
        }
    }
}
