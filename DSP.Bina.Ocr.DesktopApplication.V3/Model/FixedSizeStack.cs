using System;
using System.Collections.Generic;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class FixedSizeStack<T> : LinkedList<T>
    {
        private readonly int capacity;
        private readonly Action<T> onEvicted;

        public FixedSizeStack(int capacity, Action<T> onEvicted = null)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            this.capacity = capacity;
            this.onEvicted = onEvicted;
        }

        public T Pop()
        {
            if (Count == 0) throw new InvalidOperationException("The stack is empty.");
            T item = First.Value;
            RemoveFirst();
            return item;
        }

        public void Push(T item)
        {
            AddFirst(item);
            if (Count > capacity)
            {
                T evicted = Last.Value;
                RemoveLast();
                onEvicted?.Invoke(evicted);
            }
        }
    }
}
