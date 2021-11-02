using System;
using System.Collections.Generic;
using ContentChildrenGrouping.Core;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Containers
{
    /// <summary>
    /// When generator class implements this interface, then generator is available as DB plugin
    /// </summary>
    public interface IDbAvailableGroupNameGenerator: IGroupNameGenerator //TODO: remove this interface
    {
        /// <summary>
        /// unique generator name
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Readonly generator settings
        /// </summary>
        Dictionary<string, string> Settings { get; }

        IGroupNameGenerator CreateGenerator(Dictionary<string, string> settings);
    }

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    [ServiceConfiguration(typeof(IDbAvailableGroupNameGenerator))]
    public class ByNameGroupNameGenerator : IGroupNameGenerator, IDbAvailableGroupNameGenerator
    {
        public string Key => "Name";

        private readonly int _startIndex;
        private readonly int _countLetters;
        private readonly string _defaultName;

        public ByNameGroupNameGenerator(int startIndex = 0, int countLetters = 1, string defaultName = "_default")
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

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    [ServiceConfiguration(typeof(IDbAvailableGroupNameGenerator))]
    public class ByCreateDateGroupNameGenerator : IGroupNameGenerator, IDbAvailableGroupNameGenerator
    {
        public string Key => "Create date";

        private readonly string _dateFormat;
        private readonly string _defaultValue;

        public ByCreateDateGroupNameGenerator(string dateFormat = "yy-MMM", string defaultValue = "_default")
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
            { "dateFormat", _defaultValue },
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

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    internal class ByExpressionGroupNameGenerator : IGroupNameGenerator
    {
        private readonly Func<IContent, string> _expression;

        public ByExpressionGroupNameGenerator()
        {
            _expression = content => content.Name;
        }

        public ByExpressionGroupNameGenerator(Func<IContent, string> expression)
        {
            _expression = expression;
        }

        public string GetName(IContent content)
        {
            return _expression(content);
        }
    }
}