using System.Collections.Generic;

namespace Gnomic.Util
{
    /// <summary>
    /// Used to cache objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cache<T> where T : new()
    {
        private Stack<T> _stack;

        public Cache()
        {
            _stack = new Stack<T>();
        }

        public Cache(int size)
        {
            _stack = new Stack<T>(size);
            for (int i = 0; i < size; i++)
            {
                _stack.Push(new T());
            }
        }

        public int Count { get { return _stack.Count; } }

        public T Fetch()
        {
            if (_stack.Count > 0)
            {
                return _stack.Pop();
            }
            return new T();
        }

        public void Insert(T item)
        {
            _stack.Push(item);
        }
    }

    /// <summary>
    /// Used to cache objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheQueue<T> where T : class, new()
    {
        private Queue<T> _queue;

        public CacheQueue()
        {
            _queue = new Queue<T>();
        }

        public CacheQueue(int size)
        {
            _queue = new Queue<T>(size);
            for (int i = 0; i < size; i++)
            {
                _queue.Enqueue(new T());
            }
        }

        public int Count { get { return _queue.Count; } }

        public T Fetch()
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            return null;
        }

        public T Fetch(T objToFetch)
        {
            if (_queue.Count > 0)
            {
                T tmp = _queue.Dequeue();
                while (tmp != objToFetch)
                {
                    _queue.Enqueue(tmp);
                    tmp = _queue.Dequeue();
                }
                return tmp;
            }
            return null;
        }

        public T Peek()
        {
            if (_queue.Count > 0)
            {
                return _queue.Peek();
            }
            return null;
        }

        public void Insert(T item)
        {
            _queue.Enqueue(item);
        }
    }
}
