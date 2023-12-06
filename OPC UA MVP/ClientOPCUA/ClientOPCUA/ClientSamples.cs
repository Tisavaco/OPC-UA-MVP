using Opc.Ua.Client;
using Opc.Ua;
using System.Collections;


public interface IUAClient
{
    ISession Session { get; }
};
internal class ClientSamples
{
    const int kMaxSearchDepth = 128;
    public ClientSamples(TextWriter output, Action<IList, IList> validateResponse, ManualResetEvent quitEvent = null, bool verbose = false)
    {
        m_output = output;
        m_validateResponse = validateResponse ?? ClientBase.ValidateResponse;
        m_quitEvent = quitEvent;
        m_verbose = verbose;
    }

    #region Public Sample Methods

    /// <summary>
    /// Create Subscription and MonitoredItems for DataChanges
    /// </summary>
    public void SubscribeToDataChanges(ISession session, uint minLifeTime)
    {
        if (session == null || session.Connected == false)
        {
            m_output.WriteLine("Session not connected!");
            return;
        }

        try
        {
            // Create a subscription for receiving data change notifications

            // Define Subscription parameters
            Subscription subscription = new Subscription(session.DefaultSubscription)
            {
                DisplayName = "Console ReferenceClient Subscription",
                PublishingEnabled = true,
                PublishingInterval = 100,
                LifetimeCount = 0,
                MinLifetimeInterval = minLifeTime,
            };

            session.AddSubscription(subscription);

            // Create the subscription on Server side
            subscription.Create();
            m_output.WriteLine("New Subscription created with SubscriptionId = {0}.", subscription.Id);

            // Create MonitoredItems for data changes (Reference Server)

            MonitoredItem monitoredItem = new MonitoredItem(subscription.DefaultItem);

            monitoredItem.StartNodeId = new NodeId("ns=2;s=Sinx");
            monitoredItem.AttributeId = Attributes.Value;
            monitoredItem.DisplayName = "SinGenerator";
            monitoredItem.SamplingInterval = 100;
            monitoredItem.QueueSize = 10;
            monitoredItem.DiscardOldest = true;
            monitoredItem.Notification += OnMonitoredItemNotification;

            subscription.AddItem(monitoredItem);

            // Create the monitored items on Server side
            subscription.ApplyChanges();
            m_output.WriteLine("MonitoredItems created for SubscriptionId = {0}.", subscription.Id);
        }
        catch (Exception ex)
        {
            m_output.WriteLine("Subscribe error: {0}", ex.Message);
        }
    }
    #endregion

    /// <summary>
    /// Handle DataChange notifications from Server
    /// </summary>
    private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        try
        {
            // Log MonitoredItem Notification event
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            m_output.WriteLine("\"{0}\" and Value = {1}.", monitoredItem.ResolvedNodeId, notification.Value);
        }
        catch (Exception ex)
        {
            m_output.WriteLine("OnMonitoredItemNotification error: {0}", ex.Message);
        }
    }

   

    private Action<IList, IList> m_validateResponse;
    private readonly TextWriter m_output;
    private readonly ManualResetEvent m_quitEvent;
    private readonly bool m_verbose;
}