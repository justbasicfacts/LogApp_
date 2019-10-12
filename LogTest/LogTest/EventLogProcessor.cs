using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LogApp
{
    public class EventLogProcessor
    {
        public EventLogProcessor(string path)
        {
            _path = path;
        }
        private readonly string _path;

        public void ProcessFile()
        {
            List<EventLog> eventLogList = new List<EventLog>();
            using (StreamReader sw = new StreamReader(_path))
            {
                using (var db = new LiteDatabase(@"LogApp.db"))
                {
                    var events = db.GetCollection<Event>("Events");
                    events.EnsureIndex("EventID");
                    while (sw.Peek() >= 0)
                    {
                        string line = sw.ReadLine();
                        var currentEventLog = GetLogFileObject(line);
                        ProcessEvent(eventLogList, events, currentEventLog);
                    }
                }
            }
        }

        private void ProcessEvent(List<EventLog> eventLogList, LiteCollection<Event> events, EventLog currentEventLog)
        {
            bool sameEventInList = HasSameEventInList(out int existedItemIndex, eventLogList, currentEventLog.ID);
            if (events.Count(e => e.EventID == currentEventLog.ID) == 0)
            {
                if (sameEventInList)
                {
                    var firstEventLog = GetEventByID(eventLogList, currentEventLog.ID);

                    Event @event = null;

                    if (firstEventLog.State == EventState.STARTED && currentEventLog.State == EventState.FINISHED)
                    {
                        @event = new Event(firstEventLog, currentEventLog);
                    }
                    else if (firstEventLog.State == EventState.FINISHED && currentEventLog.State == EventState.STARTED)
                    {
                        @event = new Event(currentEventLog, firstEventLog);
                    }

                    if (@event != null)
                    {
                        if (!@event.IsValid())
                        {
                            events.Insert(@event);
                        }
                    }
                    eventLogList.RemoveAt(existedItemIndex);
                }
                else
                {
                    eventLogList.Add(currentEventLog);
                }
            }
        }

        private bool HasSameEventInList(out int index, List<EventLog> eventLogList, string id)
        {
            index = 0;
            for (int i = 0; i < eventLogList.Count; i++)
            {
                var logItem = eventLogList[i];
                if (logItem != null && logItem.ID == id)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        private EventLog GetEventByID(List<EventLog> eventLogList, string id)
        {
            for (int i = 0; i < eventLogList.Count; i++)
            {
                var logItem = eventLogList[i];
                if (logItem != null && logItem.ID == id)
                {
                    return logItem;
                }
            }

            return null;
        }

        private EventLog GetLogFileObject(string line)
        {
            return JsonSerializer.Deserialize<EventLog>(line);
        }
    }
}
