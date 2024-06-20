using SQLParse.Models;

namespace SQLParse.Services
{
    public interface ISQLFormatHandler
    {
        public Task<string> Format(SQLFormatRequest options);
    }
}
