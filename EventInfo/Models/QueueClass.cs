
namespace EventInfo.Models
{
    public class QueueClass
    {
        private readonly Queue<EventInfoClass> messageQueue;
        private readonly int maxQueueSize;
        private static QueueClass? instance;
        private static object lockObj = new object();

        private QueueClass()
        {
            messageQueue = new Queue<EventInfoClass>();
            maxQueueSize = 100; // the number of event messages to store in the queue
        }

        public static QueueClass Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new QueueClass();
                    }
                    return instance;
                }
            }
        }

        public void Enqueue(EventInfoClass message)
        {
            if (messageQueue.Count >= maxQueueSize)
            {
                messageQueue.TryDequeue(out _);
            }
            messageQueue.Enqueue(message);
        }

        public EventInfoClass[] Messages()
        {
            return messageQueue.ToArray();
        }

        public int Count()
        {
            return messageQueue.Count;
        }

        public void Clear()
        {
            messageQueue.Clear();
        }
    }
}
