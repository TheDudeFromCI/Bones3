using System;
using System.Collections.Generic;

namespace WraithavenGames.Bones3.Meshing
{
    public class ChunkTaskPool
    {
        private Dictionary<Type, List<IRemeshTask>> list = new Dictionary<Type, List<IRemeshTask>>();

        public T Get<T>()
            where T : IRemeshTask, new()
        {
            Type type = typeof(T);

            if (list.ContainsKey(type))
            {
                List<IRemeshTask> l = list[type];
                if (l.Count > 0)
                {
                    var t = l[l.Count - 1];
                    l.RemoveAt(l.Count - 1);
                    return (T)t;
                }
            }

            return new T();
        }

        public void Put<T>(T task)
            where T : IRemeshTask
        {
            Type type = typeof(T);

            List<IRemeshTask> l;
            if (list.ContainsKey(type))
                l = list[type];
            else
            {
                l = new List<IRemeshTask>();
                list[type] = l;
            }

            if (l.Count >= 8)
                task.Dispose();
            else
                l.Add(task);
        }

        public void Dispose()
        {
            foreach (var l in list.Values)
                foreach (var n in l)
                    n.Dispose();

            list.Clear();
        }
    }
}