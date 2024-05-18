

using EventInfo.Services;

namespace EventInfo.Controllers
{
    public class Controller : ControllerBase
    {

        public Controller()
        {}

        [Route("messages")]
        [HttpGet]
        public IEnumerable<EventInfoClass> GetMessages()
        {
            return QueueClass.Instance.Messages();
        }

        [Route("messages/{pondId}")]
        [HttpGet]
        public IEnumerable<EventInfoClass> GetMessage(string pondId)
        {
            return QueueClass.Instance.Messages().Where(m => m.LocationId == pondId);
        }

        [Route("messages/temperatureAlerts")]
        [HttpGet]
        public IEnumerable<EventInfoClass> GetTemperatureAlerts()
        {
            return QueueClass.Instance.Messages().Where(m => m.EventType.ToLower().Contains("temperature"));
        }

        [Route("messages/phAlerts")]
        [HttpGet]
        public IEnumerable<EventInfoClass> GetPhAlerts()
        {
            return QueueClass.Instance.Messages().Where(m => m.EventType.ToLower().Contains("ph"));
        }
    }
}
