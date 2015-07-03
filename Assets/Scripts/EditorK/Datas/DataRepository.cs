using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.EditorK.Datas
{
    public class DataRepository<T> where T : class, new()
    {
        private List<HistoryData> history = new List<HistoryData>();
        private int nextIndex = 0;
        public T Data { get; private set; }

        public void New(T data, string evt, Dictionary<string, object> args)
        {
            Clear(0);
            Data = data;
            Modify(evt, args);
        }

        public void Modify(string evt, Dictionary<string, object> args)
        {
            Clear(nextIndex);
            nextIndex += 1;
            HistoryData current = new HistoryData(Data, evt, args);
            history.Add(current);
            Data = Clone(current.Data);

            EventManager.Instance.FireEvent(evt, args);
        }

        public void Undo()
        {
            if (nextIndex <= 1)
                return;

            nextIndex -= 1;
            HistoryData last = history[nextIndex];
            HistoryData current = history[nextIndex - 1];
            Data = Clone(current.Data);

            EventManager.Instance.FireEvent(last.Event, last.Args);
        }

        public void Redo()
        {
            if (nextIndex >= history.Count)
                return;

            HistoryData current = history[nextIndex];
            Data = Clone(current.Data);
            nextIndex += 1;

            EventManager.Instance.FireEvent(current.Event, current.Args);
        }

        private void Clear(int toIndex)
        {
            history.RemoveRange(toIndex, history.Count - toIndex);
            nextIndex = toIndex;

            if (nextIndex > 0)
            {
                HistoryData current = history[nextIndex - 1];
                Data = Clone(current.Data);
            }
            else
            {
                Data = null;
            }
        }

        private T Clone(T data)
        {
            return SimpleJson.DeserializeObject<T>(SimpleJson.SerializeObject(data));
        }

        class HistoryData
        {
            public HistoryData(T data, string evt, Dictionary<string, object> args)
            {
                Data = data;
                Event = evt;
                Args = args;
            }

            public T Data;
            public string Event;
            public Dictionary<string, object> Args;
        }
    }
}
