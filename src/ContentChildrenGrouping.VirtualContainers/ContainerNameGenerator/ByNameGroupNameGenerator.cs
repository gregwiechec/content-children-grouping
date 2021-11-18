using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator
{
    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    [ServiceConfiguration(typeof(IDbAvailableGroupNameGenerator))]
    public class ByNameGroupNameGenerator : IGroupNameGenerator, IDbAvailableGroupNameGenerator
    {
        public string Key => "Name";

        public const string DefaultName = "!default";

        private readonly int _startIndex;
        private readonly int _countLetters;
        private readonly string _defaultName;

        public ByNameGroupNameGenerator(int startIndex = 0, int countLetters = 1, string defaultName = DefaultName)
        {
            _startIndex = startIndex;
            _countLetters = countLetters;
            _defaultName = defaultName;
        }

        public string GetName(IContent content)
        {
            var nameLength = content.Name.Length;

            if (nameLength <= _startIndex || nameLength < (_startIndex + _countLetters))
            {
                return _defaultName;
            }

            var result = content.Name.Substring(_startIndex, _countLetters);
            result = result.Trim();

            if (string.IsNullOrWhiteSpace(result))
            {
                return _defaultName;
            }

            return result;
        }

        public Dictionary<string, string> Settings => new Dictionary<string, string>
        {
            { "startIndex", _startIndex.ToString() },
            { "countLetters", _countLetters.ToString() },
            { "defaultName", _defaultName }
        };

        public IGroupNameGenerator CreateGenerator(Dictionary<string, string> settings)
        {
            if (settings == null)
            {
                return new ByNameGroupNameGenerator();
            }
            var startIndex = 0;
            if (settings.TryGetValue("startIndex", out var value))
            {
                int.TryParse(value, out startIndex);
            }

            var countLetters = 1;
            if (settings.TryGetValue("countLetters", out var value2))
            {
                int.TryParse(value2, out countLetters);
            }

            settings.TryGetValue("defaultName", out var defaultName);

            var result = new ByNameGroupNameGenerator(startIndex, countLetters, defaultName);
            return result;
        }
    }
}