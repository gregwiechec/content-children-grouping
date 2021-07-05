using System;
using EPiServer.Core;

//TODO: group move to another assembly

namespace EPiServer.Labs.ContentManager.ContentClustering
{


    internal class CreatedDateClusterGenerator : IClusterNameGenerator
    {
        private readonly string _format;
        private readonly string _defaultValue;

        public CreatedDateClusterGenerator(string format, string defaultValue = "No date")
        {
            _format = format;
            _defaultValue = defaultValue;
        }

        public string GetName(IContent content)
        {
            if (content is IChangeTrackable changeTrackable)
            {
                return changeTrackable.Created.ToString(this._format);
            }

            return _defaultValue;
        }
    }

    internal class NameClusterGenerator : IClusterNameGenerator
    {
        private readonly int _startIndex;
        private readonly int _countLetters;
        private readonly string _defaultName;

        public NameClusterGenerator(int startIndex, int countLetters, string defaultName)
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

    internal class ExpressionClusterGenerator : IClusterNameGenerator
    {
        private readonly Func<IContent, string> _expression;

        public ExpressionClusterGenerator(Func<IContent, string> expression)
        {
            _expression = expression;
        }

        public string GetName(IContent content)
        {
            return _expression(content);
        }
    }
}
