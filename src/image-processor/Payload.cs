namespace ImageProcessor
{
    public class Payload
    {
        public PayloadMessage Message { get; set; }

        public string Subscription { get; set; }
    }

    public class PayloadMessage
    {
        public string Data { get; set; }

        public string MessageId { get; set; }
    }
}