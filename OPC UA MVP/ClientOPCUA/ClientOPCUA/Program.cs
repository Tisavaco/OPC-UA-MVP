using ClientOPCUA;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

const string sinTag = "Sinx";

Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");


TextWriter output = Console.Out;
bool autoAccept = false;
string username = null;
string userpassword = null;
bool verbose = false;
bool noSecurity = false;

var applicationName = "ConsoleReferenceClient";
var configSectionName = "Quickstarts.ReferenceClient";
var usage = $"Usage: dotnet {applicationName}.dll [OPTIONS]";
CertificatePasswordProvider PasswordProvider = new CertificatePasswordProvider(null);
ApplicationInstance application = new ApplicationInstance
{
    ApplicationName = applicationName,
    ApplicationType = ApplicationType.Client,
    ConfigSectionName = configSectionName,
    CertificatePasswordProvider = PasswordProvider
};

var config = await application.LoadApplicationConfiguration(silent: false).ConfigureAwait(false);

ReverseConnectManager reverseConnectManager = null;

var quitCTS = new CancellationTokenSource();
var quitEvent = ConsoleUtils.CtrlCHandler(quitCTS);
bool quit = false;
int waitTime = int.MaxValue;
using (UAClient uaClient = new UAClient(application.ApplicationConfiguration, reverseConnectManager, output, ClientBase.ValidateResponse)
{
    AutoAccept = autoAccept,
    SessionLifeTime = 600_000,
})
{
    // set user identity
    if (!String.IsNullOrEmpty(username))
    {
        uaClient.UserIdentity = new UserIdentity(username, userpassword ?? string.Empty);
    }

    bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), !noSecurity, quitCTS.Token).ConfigureAwait(false);
    if (connected)
    {
        output.WriteLine("Connected! Ctrl-C to quit.");

        uaClient.ReconnectPeriod = 100;
        uaClient.ReconnectPeriodExponentialBackoff = 10000;
        uaClient.Session.MinPublishRequestCount = 3;
        uaClient.Session.TransferSubscriptionsOnReconnect = true;

        var samples = new ClientSamples(output, ClientBase.ValidateResponse, quitEvent, verbose);
        samples.SubscribeToDataChanges(uaClient.Session, 100);
        Console.ReadKey();


        output.WriteLine("Client disconnected.");
        uaClient.Disconnect();
    }
    else
    {
        output.WriteLine("Could not connect to server! Retry in 10 seconds or Ctrl-C to quit.");
        quit = quitEvent.WaitOne(Math.Min(10_000, waitTime));
    }
}
