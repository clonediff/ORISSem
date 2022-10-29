using HttpServer2.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer2;

public class HttpServer : IDisposable
{
    internal HttpListener _listener;
    public ServerStatus Status = ServerStatus.Stop;
    private ServerSettings _settings;
    private AttributeRequestsHandler _handler = new();
    private DefaultResponseSender _responseSender = new();

    public HttpServer()
    {
        _listener = new();
    }

    public void Start()
    {
        if (Status == ServerStatus.Start)
        {
            Console.WriteLine("Сервер уже запущен");
            return;
        }

        var settingsPath = "settings.json";
        if (File.Exists(settingsPath))
            _settings = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText(settingsPath));
        else
        {
            Console.WriteLine("Не найден файл настроек. Невозможно запустить сервер");
            return;
        }

        var mainUri = $"http://localhost:{_settings.Port}/";
        _listener.Prefixes.Clear();
        _listener.Prefixes.Add(mainUri);
        Console.WriteLine("Запуск сервера...");
        _listener.Start();
        Status = ServerStatus.Start;
        Console.WriteLine($"Сервер запущен на {mainUri}");

        ListeningAsync();
    }

    public void Stop()
    {
        if (Status == ServerStatus.Stop) return;

        Console.WriteLine("Остановка сервера...");
        _listener.Stop();
        Status = ServerStatus.Stop;
        Console.WriteLine("Сервер остановлен");
    }

    public void Restart()
    {
        if (Status == ServerStatus.Stop)
        {
            Console.WriteLine("Сервер уже остановлен. Для запуска используйте start вместо restart");
            return;
        }

        //Console.WriteLine("Перезапуск сервера...");
        Stop();
        Start();
        //Console.WriteLine("Сервер перезапущен");
    }

    private async Task ListeningAsync()
    {
        while (Status == ServerStatus.Start)
        {
            var context = await _listener.GetContextAsync();
            if (!_handler.MethodHandler(context))
                _responseSender.SetDefaultResponse(context, _settings);
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
