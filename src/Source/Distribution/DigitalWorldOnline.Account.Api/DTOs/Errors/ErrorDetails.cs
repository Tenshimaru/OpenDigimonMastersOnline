using Newtonsoft.Json;

namespace DigitalWorldOnline.Api.Dtos.Errors
{
    public class ErrorDetails
    {
        public int code { get; set; }

        public string error { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}