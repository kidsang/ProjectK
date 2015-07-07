using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK.Base
{
    public delegate void EventListener(params object[] args);

    class EventManager
    {
        private Dictionary<string, EventListener> listeners = new Dictionary<string, EventListener>();
        private Dictionary<IDisposable, List<ListenerInfo>> owners = new Dictionary<IDisposable, List<ListenerInfo>>();

        private EventManager()
        {
            Log.Debug("EventManager Init.");
        }

        public void Register(IDisposable owner, string type, EventListener listener)
        {
            Log.Assert(owner != null);
            Log.Assert(type != null);
            Log.Assert(listener != null);

            if (listeners.ContainsKey(type))
                listeners[type] += listener;
            else
                listeners[type] = listener;

            if (owners.ContainsKey(owner))
                owners[owner].Add(new ListenerInfo(type, listener));
            else
                owners[owner] = new List<ListenerInfo>() { new ListenerInfo(type, listener) };
        }

        public void Unregister(IDisposable owner, string type, EventListener listener)
        {
            Log.Assert(owner != null);
            Log.Assert(type != null);
            Log.Assert(listener != null);

            if (listeners.ContainsKey(type))
            {
                listeners[type] -= listener;
                if (listeners[type] == null)
                    listeners.Remove(type);


                List<ListenerInfo> infos = owners[owner];
                for (int i = infos.Count - 1; i >= 0; --i)
                {
                    ListenerInfo info = infos[i];
                    if (info.type == type && info.listener == listener)
                    {
                        infos.Remove(info);
                        break;
                    }
                }
                if (infos.Count == 0)
                    owners.Remove(owner);
            }
        }

        public void UnregisterAll(IDisposable owner)
        {
            if (owners.ContainsKey(owner))
            {
                List<ListenerInfo> infos = owners[owner];
                for (int i = infos.Count - 1; i >= 0; --i)
                {
                    ListenerInfo info = infos[i];
                    listeners[info.type] -= info.listener;
                    if (listeners[info.type] == null)
                        listeners.Remove(info.type);
                }

                owners.Remove(owner);
            }
        }

        public void FireEvent(string type, params object[] args)
        {
            if (!listeners.ContainsKey(type))
                return;

            listeners[type](args);
        }

        public void FireEvent(string type)
        {
            if (!listeners.ContainsKey(type))
                return;

            listeners[type](null);
        }

        private class ListenerInfo
        {
            public string type;
            public EventListener listener;

            public ListenerInfo(string type, EventListener listener)
            {
                this.type = type;
                this.listener = listener;
            }
        }

        private static EventManager instance = new EventManager();
        public static EventManager Instance
        {
            get { return instance; }
        }
    }
}
