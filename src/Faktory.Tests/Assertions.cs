using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace Faktory.Tests
{
    public class ContainsTrimmed : Constraint
    {
        private readonly IEnumerable<string> _expected;
    
        public ContainsTrimmed(IEnumerable<string> expected)
        {
            _expected = expected;
        }
    
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var isMatch = _expected.Any(expectedItem => expectedItem.Trim() == actual.ToString());
            return new ConstraintResult(this, actual, isMatch);
        }
    
        public override string Description
        {
            get
            {
                return $"some item equal to \"{string.Join("\", \"", _expected)}\"";
            }
        }
    }
}