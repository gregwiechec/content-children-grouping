using System;
using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using EPiServer.Framework.Serialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Containers.RegisterFromDb
{
    //TODO: add tests
    [ServiceConfiguration(typeof(GroupNameSerializer))]
    public class GroupNameSerializer
    {
        private readonly IObjectSerializerFactory _serializerFactory;
        private static ILogger _log = LogManager.GetLogger(typeof(GroupNameSerializer));
        private readonly IEnumerable<IDbAvailableGroupNameGenerator> _dbAvailableGroupNameGenerators;

        public GroupNameSerializer(IObjectSerializerFactory serializerFactory,
            IEnumerable<IDbAvailableGroupNameGenerator> dbAvailableGroupNameGenerators)
        {
            _serializerFactory = serializerFactory;
            _dbAvailableGroupNameGenerators = dbAvailableGroupNameGenerators;
        }

        public string Serialize(IEnumerable<IGroupNameGenerator> generators)
        {
            var itemsToSerialize = generators.OfType<IDbAvailableGroupNameGenerator>().Select(x => new GeneratorDto
            {
                Name = x.Key,
                Settings = x.Settings
            });
            var result = _serializerFactory.GetSerializer(KnownContentTypes.Json).Serialize(itemsToSerialize);
            return result;
        }

        public IEnumerable<IGroupNameGenerator> Deserialize(string serializedValue)
        {
            IEnumerable<GeneratorDto> deserializedItems;
            try
            {
                deserializedItems = _serializerFactory.GetSerializer(KnownContentTypes.Json)
                    .Deserialize<IEnumerable<GeneratorDto>>(serializedValue);
            }
            catch (Exception e)
            {
                _log.Error("Cannot deserialize configurations", e);
                return Enumerable.Empty<IGroupNameGenerator>();
            }


            var groupNameGenerators = new List<IGroupNameGenerator>();
            foreach (var deserializedItem in deserializedItems)
            {
                var generator = _dbAvailableGroupNameGenerators.FirstOrDefault(x => x.Key == deserializedItem.Name);
                if (generator != null)
                {
                    var groupNameGenerator = generator.CreateGenerator(deserializedItem.Settings);
                    groupNameGenerators.Add(groupNameGenerator);
                }
            }

            return groupNameGenerators;
        }

        public class GeneratorDto
        {
            public string Name { get; set; }
            public Dictionary<string, string> Settings { get; set; }
        }
    }
}
