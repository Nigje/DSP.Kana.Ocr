using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class FixedSizeStack<T> : LinkedList<T>
    {
        private int limit;

        public FixedSizeStack(int limit)
            : base()
        {
            this.limit = limit;
        }

        public T Pop()
        {
            T obj = base.First.Value;
            base.RemoveFirst();
            return obj;
        }

        public void Push(T obj)
        {
            base.AddFirst(obj);
            if (this.Count > limit)
            {
                base.RemoveLast();
            }
        }
    }
}
