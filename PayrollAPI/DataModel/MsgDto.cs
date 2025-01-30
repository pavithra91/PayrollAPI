namespace PayrollAPI.DataModel
{
    public class MsgDto
    {
        public char MsgCode { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public int total { get; set; }
        public string Data { get; set; }
    }
}
