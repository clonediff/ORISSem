namespace HttpServer2
{
    public static class Constants
    {
        public const string NotFoundPage =
@"<!DOCTYPE html> 
<html>
    <head>
        <meta charset=""utf-8"" />
        <title>404 Not Found</title>
    </head>
    <body style=""display:flex; justify-content:center;"">
        <h1>404 Not Found</h1>
    </body>
</html>";

        public const string SteamDBConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamBD;Integrated Security=True;";
    }

    public class ServerSettings
    {
        public int Port { get; init; }
        public string TemplatesFolder { get; init; }
    }
}
