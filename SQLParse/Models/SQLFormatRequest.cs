namespace SQLParse.Models
{
    public class SQLFormatRequest
    {
        public string Dialect { get; set; }
        public string Script { get; set; }
        public bool SpaceAroundOperators { get; set; } = true;
        public bool CommasBefore { get; set; } = false;
        public bool AlignUpdateSet { get; set; } = true;
    }
}
