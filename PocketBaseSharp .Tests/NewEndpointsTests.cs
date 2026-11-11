using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace PocketBaseSharp.Tests
{
    /// <summary>
    /// Tests for the endpoints added between PocketBase v0.35.1 and v0.42.0.
    /// </summary>
    [TestClass]
    public class NewEndpointsTests
    {
        private HttpRequestMessage? _lastRequest;
        private string? _lastBody;

        private PocketBase CreateClient(HttpStatusCode status, string responseJson = "")
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>(async (req, ct) =>
                {
                    _lastRequest = req;
                    _lastBody = req.Content is null ? null : await req.Content.ReadAsStringAsync(ct);
                    return new HttpResponseMessage(status)
                    {
                        Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                    };
                });

            return new PocketBase("http://127.0.0.1:8090", httpClient: new HttpClient(handler.Object));
        }

        [TestMethod]
        public async Task Logs_TruncateAsync_Sends_Delete_To_Api_Logs()
        {
            var client = CreateClient(HttpStatusCode.NoContent);

            var result = await client.Log.TruncateAsync();

            result.IsSuccess.Should().BeTrue();
            _lastRequest!.Method.Should().Be(HttpMethod.Delete);
            _lastRequest.RequestUri!.AbsolutePath.Should().Be("/api/logs");
        }

        [TestMethod]
        public async Task Logs_TruncateAsync_Fails_On_Unauthorized()
        {
            var client = CreateClient(HttpStatusCode.Unauthorized, "{}");

            var result = await client.Log.TruncateAsync();

            result.IsFailed.Should().BeTrue();
        }

        [TestMethod]
        public async Task Sql_RunAsync_Posts_Query_And_Parses_Result()
        {
            const string json = """
                {"execTime":1.5,"affectedRows":0,
                 "columns":[{"name":"id","type":"TEXT","nullable":false},{"name":"n","type":"INTEGER","nullable":true}],
                 "rows":[["abc","1"],["def",null]]}
                """;
            var client = CreateClient(HttpStatusCode.OK, json);

            var result = await client.Sql.RunAsync("SELECT id, n FROM test");

            _lastRequest!.Method.Should().Be(HttpMethod.Post);
            _lastRequest.RequestUri!.AbsolutePath.Should().Be("/api/sql");
            _lastBody.Should().Contain("\"query\":\"SELECT id, n FROM test\"");

            result.IsSuccess.Should().BeTrue();
            result.Value.ExecTime.Should().Be(1.5);
            result.Value.Columns.Should().HaveCount(2);
            result.Value.Columns[1].Nullable.Should().BeTrue();
            result.Value.Rows[1][1].Should().BeNull();
        }

        [TestMethod]
        public async Task Collections_GetAllOAuth2ProvidersAsync_Gets_Meta_Endpoint()
        {
            const string json = """[{"name":"google","displayName":"Google","logo":"<svg></svg>"}]""";
            var client = CreateClient(HttpStatusCode.OK, json);

            var result = await client.Collections.GetAllOAuth2ProvidersAsync();

            _lastRequest!.Method.Should().Be(HttpMethod.Get);
            _lastRequest.RequestUri!.AbsolutePath.Should().Be("/api/collections/meta/oauth2-providers");
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(p => p.Name == "google" && p.DisplayName == "Google" && p.Logo == "<svg></svg>");
        }

        [TestMethod]
        public async Task Collections_DryRunViewQueryAsync_Posts_Query()
        {
            // shape captured from a live PocketBase v0.42.0 server
            const string json = """
                {"fields":[{"id":"text3208210256","name":"id","type":"text","primaryKey":true},{"id":"number1","name":"total","type":"number"}],
                 "sample":[{"collectionId":"pbc_1","collectionName":"temp_view_x","id":"1","total":3}]}
                """;
            var client = CreateClient(HttpStatusCode.OK, json);

            var result = await client.Collections.DryRunViewQueryAsync("SELECT id, count(*) as total FROM test");

            _lastRequest!.Method.Should().Be(HttpMethod.Post);
            _lastRequest.RequestUri!.AbsolutePath.Should().Be("/api/collections/meta/dry-run-view");
            _lastBody.Should().Contain("\"query\":");
            result.IsSuccess.Should().BeTrue();
            result.Value.Fields.Should().HaveCount(2);
            result.Value.Fields[1]["name"]!.ToString().Should().Be("total");
            result.Value.Sample.Should().ContainSingle();
            result.Value.Sample[0].Should().ContainKey("total");
        }
    }
}
