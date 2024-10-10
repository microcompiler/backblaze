using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Hosting;

class TestServer : IDisposable
{
    WebApplication _application;

    public WebApplication Application => _application;

    public Uri BaseUrl => new Uri(_application.Urls.First());

    public TestServer()
    {
        var builder = WebApplication.CreateBuilder();

        _application = builder.Build();

        // Without this route on the root, routes on subpaths seem to always return 404.
        _application.MapGet("/", () => "Test server");

        var lifetime = (IHostApplicationLifetime)_application.Services.GetService(typeof(IHostApplicationLifetime));

        var started = new ManualResetEvent(initialState: false);

        lifetime.ApplicationStarted.Register(() => started.Set());

        Task.Run(() => _application.Run());

        started.WaitOne();
    }

    public RouteBuilder When(string route) => When(HttpMethod.Get, route);
    public RouteBuilder When(HttpMethod method, string route) => new RouteBuilder(this, method, route);

    public class RouteBuilder
    {
        TestServer _server;
        HttpMethod _method;
        string _route;

        public RouteBuilder(TestServer server, HttpMethod method, string route)
        {
            _server = server;
            _method = method;
            _route = route;
        }

        public void Then(object result)
            => Then(() => Results.Json(result));

        public void Then(Delegate action)
        {
            if (_method == HttpMethod.Get)
                _server.Application.MapGet(_route, action);
            else if (_method == HttpMethod.Post)
                _server.Application.MapPost(_route, action);
            else
                throw new NotSupportedException("Not supported in TestServer: HTTP method " + _method);
        }
    }

    public void Dispose()
    {
        var task = _application.StopAsync();

        task.ConfigureAwait(false);
        task.Wait();
    }
}
