using Opc.UaFx.Client;



const string sinTag = "Sinx";

Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");
var client = new OpcClient(serverUrl);


Console.WriteLine("Подключение к серверу...");
client.Connect();
Console.WriteLine("Connected!");

var subscription = client.SubscribeNode("ns=2;s=" + sinTag);

subscription.DataChangeReceived += (sender, e) =>
{

    var data = client.ReadNode("ns=2;s=" + sinTag);
    Console.WriteLine(sinTag +": "+ data.ToString());
};


Console.ReadKey();


client.Disconnect();
