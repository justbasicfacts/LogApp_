using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogApp
{
    public class EventLog
    {
        public EventLog()
        { }

        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventState State { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType Type { get; set; }

        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }
        [JsonPropertyName("host")]
        public string Host { get; set; }



    }
}
