using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xvue.Framework.API
{

    public class RecentEventsTraceListener : TraceListener
    {
        ConcurrentQueue<string> _logItems;
        int _itemsCapacity;
        public const int DefaultCapacity = 100;

        public RecentEventsTraceListener(int itemsCapacity)
        {
            if (itemsCapacity < 1)
                itemsCapacity = 1;

            _itemsCapacity = itemsCapacity;
            _logItems = new ConcurrentQueue<string>();
        }

        public RecentEventsTraceListener()
        {
            _itemsCapacity = DefaultCapacity;
            _logItems = new ConcurrentQueue<string>();
        }

        public override void Write(string message)
        {
            while (_logItems.Count >= _itemsCapacity)
            {
                string lastItem;
                _logItems.TryDequeue(out lastItem);
            }
            _logItems.Enqueue(message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public IEnumerable<string> Messages
        {
            get { return _logItems.ToArray(); }
        }

    }
}
