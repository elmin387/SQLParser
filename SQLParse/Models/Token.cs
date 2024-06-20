namespace SQLParse.Models
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }   
        public string Content { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class Statistics
    {
        public int ElapsedMicroseconds { get; set; }
        public int TotalLinesOfCode { get; set; }
        public int NonEmptyLinesOfCode { get; set; }
    }

    public class Value
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<Token> Tokens { get; set; }
        public Statistics Statistics { get; set; }
    }

    public class TokenListResponse
    {
        public Value Value { get; set; }
        public List<object> Formatters { get; set; }
        public List<object> ContentTypes { get; set; }
        public object DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }
}
