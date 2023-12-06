using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Test;

namespace OPCServer
{
    public class ReferenceNodeManager : CustomNodeManager2
    {
        public ReferenceNodeManager(IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration, "http://opcfoundation.org/Quickstarts/ReferenceServer")
        {
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<ReferenceServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new ReferenceServerConfiguration();
            }

            m_dynamicNodes = new List<BaseDataVariableState>();
        }
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            IList<IReference> references = new List<IReference>(); 

            FolderState tags = CreateFolder(null, "SinGenerator", "SinGenerator");
            tags.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
            references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, tags.NodeId));
            tags.EventNotifier = EventNotifiers.SubscribeToEvents;
            AddRootNotifier(tags);
            CreateSinGeneratorVariable(tags, "Sinx", "Sinx", DataTypeIds.Float, ValueRanks.Scalar);
            AddPredefinedNode(SystemContext, tags);
            m_simulationTimer = new Timer(DoSimulation, null, 0, 100);
        }

        private void DoSimulation(object? state)
        {
            try
            {
                lock (Lock)
                {
                    var timeStamp = DateTime.UtcNow;
                    foreach (BaseDataVariableState variable in m_dynamicNodes)
                    {
                        if (variable.DisplayName.Text == "Sinx")
                        {
                            variable.Value = sinx.Value;
                            variable.Timestamp = timeStamp;
                            variable.ClearChangeMasks(SystemContext, false);
                            continue;
                        }
                        variable.Value = sinx.Value;
                        variable.Timestamp = timeStamp;
                        variable.ClearChangeMasks(SystemContext, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error doing simulation.");
            }
        }

        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = new NodeId(path, NamespaceIndex);
            folder.BrowseName = new QualifiedName(path, NamespaceIndex);
            folder.DisplayName = new LocalizedText("en", name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }
        private BaseDataVariableState CreateSinGeneratorVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = CreateSinGenerator(parent, path, name, dataType, valueRank);
            m_dynamicNodes.Add(variable);
            return variable;
        }
        private BaseDataVariableState CreateSinGenerator(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent);
            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            sinx.Start();
            variable.Value = sinx.Value;
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(variable);
            }
            return variable;
        }


        private ReferenceServerConfiguration m_configuration;
        private DataGenerator m_generator;
        private Timer m_simulationTimer;
        private UInt16 m_simulationInterval = 1000;
        private bool m_simulationEnabled = true;
        private List<BaseDataVariableState> m_dynamicNodes;
        private Generator sinx = new Generator();
    }
}
