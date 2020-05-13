using System.Collections.Generic;

// Borrowed from Telepathy: https://github.com/vis2k/Telepathy/blob/master/Telepathy/SafeQueue.cs
public sealed class SafeQueue<T>
{
    readonly Queue<T> queue = new Queue<T>();

    public int Count
    {
        get
        {
            lock (queue)
            {
                return queue.Count;
            }
        }
    }

    public void Enqueue(T item)
    {
        lock (queue)
        {
            queue.Enqueue(item);
        }
    }

    public bool TryDequeue(out T result)
    {
        lock (queue)
        {
            result = default(T);
            if (queue.Count > 0)
            {
                result = queue.Dequeue();
                return true;
            }
            return false;
        }
    }

    public bool TryDequeueAll(out T[] result)
    {
        lock (queue)
        {
            result = queue.ToArray();
            queue.Clear();
            return result.Length > 0;
        }
    }

    public void Clear()
    {
        lock (queue)
        {
            queue.Clear();
        }
    }
}