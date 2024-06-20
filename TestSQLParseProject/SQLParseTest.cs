using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using SQLParse.Controllers;
using SQLParse.Models;
using SQLParse.Services;

namespace TestSQLParseProject
{
    public class SQLParseTest
    {
        [Fact]
            public async Task SQLFormatController_ValidRequest_ReturnsFormattedSql()
            {
            // Arrange
            var httpClient = new HttpClient();
            var mockOptions = new Mock<IOptions<Config>>();
            mockOptions.Setup(o => o.Value).Returns(new Config
            {
                BaseUrl = "https://sqlparsedev.azurewebsites.net/api/TokenList"
            });
            var sqlFormatHanlder = new SQLFormatHandler(httpClient,mockOptions.Object);
                var controller = new SQLFormatController(sqlFormatHanlder);
                var request = new SQLFormatRequest
                {
                    Dialect = "sqlserver",
                    Script = "select * from x where a = 'robert'; drop table students;-- 'smith);"
                };

                // Act
                var result = await controller.SQLFormat(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            // Example of expected result
            Assert.Equal("SELECT * \r\nFROM x \r\nWHERE a = 'robert'; \r\nDROP TABLE students;-- 'smith);", okResult.Value); 
        }

        }
    }
