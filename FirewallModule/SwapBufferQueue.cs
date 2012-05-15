using System.Collections.Generic;

namespace FM
{
    /// <summary>
    /// The point of this is to make a queue that can take input from multiple threads at the same time while a single thread is dumping the information in the queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SwapBufferQueue<T>
    {
        private Queue<T> bufferA = new Queue<T>();
        private Queue<T> bufferB = new Queue<T>();

        //if false, A is being filled
        private bool swap = false;
        private readonly object dumpLock = new object();

        /// <summary>
        /// Adds an object to the current queue for filling
        /// </summary>
        /// <param name="t">The object to be added</param>
        public void Enqueue(T t)
        {
            lock (this)
            {
                if (swap)
                {
                    bufferB.Enqueue(t);
                }
                else
                {
                    bufferA.Enqueue(t);
                }
            }
        }

        /// <summary>
        /// Swaps the current buffer states, and empties the queue and returns a copy
        /// </summary>
        /// <returns>A copy of the queue</returns>
        public Queue<T> DumpBuffer()
        {
            lock (dumpLock)
            {
                lock (this)
                {
                    swap = !swap;
                }
                Queue<T> ret;
                if (swap)
                {
                    ret = new Queue<T>(bufferA);
                    bufferA.Clear();
                }
                else
                {
                    ret = new Queue<T>(bufferB);
                    bufferB.Clear();
                }
                return ret;
            }
        }
    }
}
