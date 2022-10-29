using System;
using System.Text.Json;

namespace HttpServer2
{
    internal class Program
    {
        private static bool _appIsRunning = true;
        static void Main(string[] args)
        {
            using (var server = new HttpServer())
            {
                server.Start();
                while (_appIsRunning)
                    CmdListening(Console.ReadLine()?.ToLower(), server);
            }
        }

        static void CmdListening(string? command, HttpServer server)
        {
            switch (command)
            {
                case "start":
                        server.Start();
                    break;
                case "stop":
                    server.Stop();
                    break;
                case "restart":
                    server.Restart();
                    break;
                case "exit":
                    _appIsRunning = false;
                    break;
                case "cls":
                    Console.Clear();
                    break;
                case "status":
                    Console.WriteLine("Server Status = {0}", server.Status);
                    break;
                case "url":
                    Console.WriteLine(server._listener.Prefixes.FirstOrDefault());
                    break;
            }
        }
    }
}