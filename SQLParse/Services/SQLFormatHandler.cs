using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SQLParse.Models;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace SQLParse.Services
{
    public class SQLFormatHandler : ISQLFormatHandler
    {
        private readonly HttpClient _httpClient;
        private readonly Config _apiSettings;

        public SQLFormatHandler(HttpClient httpClient, IOptions<Config> apisettings)
        {
            _httpClient = httpClient;
            _apiSettings = apisettings.Value;
        }

        public async Task<string> Format(SQLFormatRequest options)
        {
            var tokenList = await GetTokens(options);
            if (tokenList == null)
            {
                return options.Script;
            }

            return ApplyFormatting(tokenList.Value.Tokens, options);
        }

        private async Task<TokenListResponse> GetTokens(SQLFormatRequest options)
        {
          
                var requestBody = new {options.Dialect, options.Script };
                HttpResponseMessage response = await _httpClient.PostAsync(_apiSettings.BaseUrl,
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var tokenListResponse = JsonConvert.DeserializeObject<TokenListResponse>(responseBody);

                return tokenListResponse ?? new TokenListResponse();
            
        }

        private string ApplyFormatting(List<Token> tokens, SQLFormatRequest options)
        {
            var formattedScript = new StringBuilder();
            bool newLine = false;
            bool previousWasWhitespace = false;

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                switch (token.Type)
                {
                    case "ReservedKeyword":
                        if (newLine)
                        {
                            formattedScript.AppendLine();
                            newLine = false;
                        }
                        if (i > 0 && !previousWasWhitespace && formattedScript.Length > 0)
                        {
                            formattedScript.Append(" ");
                        }
                        formattedScript.Append(token.Content.ToUpper());
                        previousWasWhitespace = false;
                        break;

                    case "Identifier":
                    case "Literal":
                        if (i > 0 && !previousWasWhitespace)
                        {
                            formattedScript.Append(" ");
                        }
                        formattedScript.Append(token.Content);
                        previousWasWhitespace = false;
                        break;

                    case "Operator":
                        if (i > 0 && !previousWasWhitespace)
                        {
                            formattedScript.Append(" ");
                        }
                        formattedScript.Append(token.Content);
                        if (i < tokens.Count - 1 && tokens[i + 1].Type != "Whitespace" && tokens[i + 1].Type != "CloseParenthesis")
                        {
                            formattedScript.Append(" ");
                        }
                        previousWasWhitespace = false;
                        break;

                    case "Comma":
                        formattedScript.Append(token.Content);
                        if (!options.CommasBefore && i < tokens.Count - 1 && tokens[i + 1].Type != "Whitespace")
                        {
                            formattedScript.Append(" ");
                        }
                        if (options.CommasBefore)
                        {
                            formattedScript.AppendLine();
                        }
                        previousWasWhitespace = false;
                        break;

                    case "Dot":
                    case "OpenParenthesis":
                    case "CloseParenthesis":
                        formattedScript.Append(token.Content);
                        previousWasWhitespace = false;
                        break;

                    case "Whitespace":
                        if (!previousWasWhitespace)
                        {
                            formattedScript.Append(" ");
                        }
                        previousWasWhitespace = true;
                        break;
                    case "Semicolon":
                        if (formattedScript.Length > 0 && formattedScript[formattedScript.Length - 1] == ' ')
                        {
                            formattedScript.Length--;
                        }
                        formattedScript.Append(token.Content);
                        if (i < tokens.Count - 1 && tokens[i + 1].Type == "Whitespace" && i + 2 < tokens.Count && tokens[i + 2].Content == "--")
                        {
                            i++;
                        }
                        previousWasWhitespace = false;
                        break;
                    case "Comment":
                        formattedScript.Append(token.Content);
                        previousWasWhitespace = true;
                        break;

                    default:
                        if (i > 0 && !previousWasWhitespace)
                        {
                            formattedScript.Append(" ");
                        }
                        formattedScript.Append(token.Content);
                        previousWasWhitespace = false;
                        break;
                }


                if (token.Type == "ReservedKeyword" && (token.Content.Equals("SELECT", StringComparison.OrdinalIgnoreCase) ||
                                                        token.Content.Equals("UPDATE", StringComparison.OrdinalIgnoreCase) ||
                                                        token.Content.Equals("DELETE", StringComparison.OrdinalIgnoreCase) ||
                                                        token.Content.Equals("INSERT", StringComparison.OrdinalIgnoreCase) ||
                                                        token.Content.Equals("WHERE", StringComparison.OrdinalIgnoreCase) ||
                                                        token.Content.Equals("FROM", StringComparison.OrdinalIgnoreCase)))
                {
                    newLine = true;
                }
            }

            return formattedScript.ToString().Trim();
        }



    }
}

