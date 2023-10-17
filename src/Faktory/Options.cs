using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Faktory.Logging;

namespace Faktory
{
    public class Options : IEnumerable<KeyValuePair<string, string>>
    {
        readonly Dictionary<string, string> _options = new Dictionary<string, string>();

        public bool Valid { get; private set; } = true;
        public Options(string args)
        {
            Parse(args);
            ValidateOptions();
        }

        void Parse(string commandLine)
        {
            const char space = ' ';
            const char equal = '=';
            const char quote = '"';
            var currentKey = "";
            var currentVal = "";
            var findingKey=true;
            var insideQuotes=false;
            for (var i = 0; i < commandLine.Length; i++)
            {
                var c = commandLine[i];

                if (findingKey)
                {
                    
                    if (c == space){
                        // We are looking for a key so we likely should skip this space
				
                    }
                    else if (c == equal)
                    {
                        // Stop finding KeyMode
                        findingKey = false;
                        // At this point currentKey should equal the key of ''
                    }
                    else
                    {
                        //Add this character to the key
                        currentKey += commandLine[i];
                    }
                }
                else
                {
                    // Must be finding Value
                    if (insideQuotes)
                    {
                        if (c == quote)
                        {
                            // stop quote mode
                            insideQuotes = false;
                            // Stop finding Value
                            findingKey = true;
                            // Store this key/value pair
                            _options[currentKey] = currentVal;
                            currentKey = "";
                            currentVal = "";
                        }
                        else
                        {
                            //Add this character to the key
                            currentVal += commandLine[i];
                        }
                    }
                    else
                    {
                        if (c == quote){
                            // Enter quote mode
                            insideQuotes = true;
                            // Do not add this character
                            // Do not change from value finding mode
                        }
                        else if (c == space)
                        {

                            // Stop finding Value
                            findingKey = true;
                            // Store this key/value pair
                            _options[currentKey] = currentVal;
                            currentKey = "";
                            currentVal = "";
                        }
                        else
                        {
                            //Add this character to the key
                            currentVal += commandLine[i];
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(currentKey) == false)
            {
                _options[currentKey] = currentVal;
            }
        }

        void ValidateOptions()
        {
            if (InvalidOptions.Any())
            {
                Valid = false;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> InvalidOptions => _options.Where(x => string.IsNullOrEmpty(x.Value));

        public bool VerboseMode => this["verbose"] != null && this["verbose"].ToLowerInvariant() == "true";

        public string this[string key] => _options.TryGetValue(key.ToLower(), out var value) ? value : null;

        public bool HasAll(List<string> options)
        {
            foreach (var x in options)
            {
                if (_options.ContainsKey(x.ToLower())) continue;

                Boot.Logger.Error($"Missing required argument '{x}'");
                return false;
            }

            return true;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _options.GetEnumerator();
        }
    }
}