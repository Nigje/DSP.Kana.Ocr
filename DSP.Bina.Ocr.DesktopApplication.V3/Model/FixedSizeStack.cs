using System;
using System.Collections.Generic;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class FixedSizeStack<T> : LinkedList<T>
    {
        private readonly int capacity;

        public FixedSizeStack(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            this.capacity = capacity;
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
            if (Count > capacity) RemoveLast();
        }
    }
}
