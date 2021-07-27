using System;
using EPiServer.Core;

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

    public class ByNameGroupNameGenerator : IGroupNameGenerator
    {
        public string Key => "Name";

        private readonly int _startIndex;
        private readonly int _countLetters;
        private readonly string _defaultName;

        public ByNameGroupNameGenerator(int startIndex, int countLetters, string defaultName)
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

    public class ByCreateDateGroupNameGenerator : IGroupNameGenerator
    {
        public string Key => "Create date";

        private readonly string _dateFormat;
        private readonly string _defaultValue;

        public ByCreateDateGroupNameGenerator(string dateFormat, string defaultValue)
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

    internal class ByExpressionGroupNameGenerator : IGroupNameGenerator
    {
        public string Key => "Expression"; //TODO: filter this generator from database selection
        
        private readonly Func<IContent, string> _expression;

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