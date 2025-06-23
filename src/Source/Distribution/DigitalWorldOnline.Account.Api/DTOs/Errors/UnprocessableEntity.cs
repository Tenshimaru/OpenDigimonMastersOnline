using Newtonsoft.Json;

namespace DigitalWorldOnline.Api.Dtos.Errors
{
    public class UnprocessableEntity
    {
        public int code { get; set; }
        public string error { get; set; }

        public Dictionary<string, string[]> errorList { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
