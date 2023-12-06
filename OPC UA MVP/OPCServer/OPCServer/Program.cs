// See https://aka.ms/new-console-template for more information
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCServer;

ApplicationInstance application = new ApplicationInstance();
application.ApplicationType = ApplicationType.Server;
application.ConfigSectionName = "Quickstarts.ReferenceServer";
var server = new ReferenceServer();
await application.Start(server);
Console.WriteLine("Сервер запущен!");
Console.ReadKey();
application.Stop();


