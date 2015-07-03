using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.EditorK.Datas
{
    public class DataRepository<T>
    {
        private List<HistoryData> history = new List<HistoryData>();
        private int nextIndex = 0;
        private HistoryData current;

        public void Operate(T data, string evt, Dictionary<string, object> args)
        {
            Clear(nextIndex);
            nextIndex += 1;
            current = new HistoryData(data, evt, args);
            history.Add(current);

            EventManager.Instance.FireEvent(evt, args);
        }

        public void Undo()
        {
            if (nextIndex <= 1)
                return;

            nextIndex -= 1;
            HistoryData last = history[nextIndex];
            current = history[nextIndex - 1];

            EventManager.Instance.FireEvent(last.Event, last.Args);
        }

        public void Redo()
        {
            if (nextIndex >= history.Count)
                return;

            current = history[nextIndex];
            nextIndex += 1;

            EventManager.Instance.FireEvent(current.Event, current.Args);
        }

        public void Clear(int toIndex)
        {
            history.RemoveRange(toIndex, history.Count - toIndex);
            nextIndex = toIndex;

            if (nextIndex > 0)
                current = history[nextIndex - 1];
            else
                current = null;
        }

        public void Clear()
        {
            Clear(0);
        }

        public T Data
        {
            get 
            {
                if (current != null)
                    return current.Data;
                return default(T);
            }
        }

        class HistoryData
        {
            public HistoryData(T data, string evt, Dictionary<string, object> args)
            {
                Data = SimpleJson.DeserializeObject<T>(SimpleJson.SerializeObject(data));
                Event = evt;
                Args = args;
            }

            public T Data;
            public string Event;
            public Dictionary<string, object> Args;
        }
    }
}
