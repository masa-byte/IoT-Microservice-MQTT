using System.Collections;

namespace EventInfo.Models
{
    public class EventInfoClass
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string LocationId { get; set; }
        public ArrayList Values { get; set; }
        public DateTime EventDate { get; set; }

        public EventInfoClass()
        {
            EventType = string.Empty;
            LocationId = string.Empty;
            Values = new ArrayList();
            EventDate = DateTime.Now;
        }
    }
}
