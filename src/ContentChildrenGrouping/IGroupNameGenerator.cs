using System;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping
{
    /// <summary>
    /// Generating name for structure
    /// </summary>
    public interface IGroupNameGenerator
    {
        /// <summary>
        /// unique generator name
        /// </summary>
        string Key { get; }

        string GetName(IContent content);
    }

    /// <summary>
    /// When generator class implements this interface, then generator is available as DB plugin
    /// </summary>
    public interface IDbAvailableGroupNameGenerator
    {
    }

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
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
            return result;
        }
    }

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
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
    }

    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    internal class ByExpressionGroupNameGenerator : IGroupNameGenerator
    {
        public string Key => "Expression";
        
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