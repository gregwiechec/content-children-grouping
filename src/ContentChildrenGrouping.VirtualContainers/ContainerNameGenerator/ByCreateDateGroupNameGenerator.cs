using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator
{
    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    [ServiceConfiguration(typeof(IDbAvailableGroupNameGenerator))]
    public class ByCreateDateGroupNameGenerator : IGroupNameGenerator, IDbAvailableGroupNameGenerator
    {
        public string Key => "Create date";

        public const string DefaultFormat = "yy-MMM";
        public const string DefaultName = "!default";

        private readonly string _dateFormat;
        private readonly string _defaultValue;

        public ByCreateDateGroupNameGenerator(string dateFormat = DefaultFormat, string defaultValue = DefaultName)
        {
            _dateFormat = dateFormat;
            _defaultValue = defaultValue;
        }

        public string GetName(IContent content)
        {
            if (content is IChangeTrackable page)
            {
                return page.Created.ToString(_dateFormat);
            }

            return _defaultValue;
        }

        public Dictionary<string, string> Settings => new Dictionary<string, string>
        {
            { "dateFormat", _dateFormat },
            { "defaultValue", _defaultValue }
        };

        public IGroupNameGenerator CreateGenerator(Dictionary<string, string> settings)
        {
            if (settings == null)
            {
                return new ByCreateDateGroupNameGenerator();
            }
            settings.TryGetValue("dateFormat", out var dateFormat);
            settings.TryGetValue("defaultValue", out var defaultValue);

            var result = new ByCreateDateGroupNameGenerator(dateFormat, defaultValue);
            return result;
        }
    }
}