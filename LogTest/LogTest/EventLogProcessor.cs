using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EventLogProject
{
    public class EventLogProcessor
    {
        const int ThreadCount = 4;
        private int currentThreadID = 0;
        Queue<Thread> threads;

        public EventLogProcessor()
        {
          
        }
        public void ProcessEvent(Event @event)
        {
            Apppend(@event);
        }

        private void Apppend(Event @event)
        {
            if (@event != null)
            {
                if (@event.GetDuration() > 4)
                {
                    AppendLog(@event);
                }
            }
        }

        public void AppendLog(Event @event)
        {
           
        }
    }
}
