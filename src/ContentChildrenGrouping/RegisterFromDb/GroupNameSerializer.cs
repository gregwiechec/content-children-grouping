using System;
using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Containers.RegisterFromDb
{
    //TODO: add tests
    [ServiceConfiguration(typeof(GroupNameSerializer))]
    public class GroupNameSerializer
    {
        private IObjectSerializerFactory _serializerFactory { get; set; }
        private ILogger _log = LogManager.GetLogger(typeof(GroupNameSerializer));

        public GroupNameSerializer(IObjectSerializerFactory serializerFactory)
        {
            _serializerFactory = serializerFactory;
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

            var dbAvailableGroupNameGenerators = ServiceLocator.Current
                .GetInstance<IEnumerable<IDbAvailableGroupNameGenerator>>().ToList();

            var groupNameGenerators = new List<IGroupNameGenerator>();
            foreach (var deserializedItem in deserializedItems)
            {
                var generator = dbAvailableGroupNameGenerators.FirstOrDefault(x => x.Key == deserializedItem.Name);
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
