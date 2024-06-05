using Flex.LVA.Shared.Exceptions;
using System.Globalization;

namespace Flex.LVA.Shared.Containers
{
    internal class LogBoolContainer : LogContainer
    {
        internal bool ValueTyped { get; init; }

        public override object Value => this.ValueTyped;

        public override string ToString() => this.ValueTyped.ToString();

        public override bool Equals(object obj)
        {
            return this.Same(obj);
        }

        public override bool Same(object theValue)
        {
            if (theValue is bool aValue)
            {
                return aValue == ValueTyped;
            }

            if (bool.TryParse(theValue.ToString(), out aValue))
            {
                return aValue == ValueTyped;
            }

            return false;
        }

        public override bool Contains(object theValue)
        {
            if (theValue is string aValue)
            {
                return ValueTyped.ToString().Contains(aValue);
            }

            return ValueTyped.ToString(CultureInfo.InvariantCulture).Contains(theValue.ToString() ?? throw new QueryFailureException($"Unable to check for contains on null object, this.Value {this.ValueTyped}"));
        }

        public override bool Greater(object theValue)
        {
            return Same(theValue);
        }

        public override bool GreaterOrEqual(object theValue)
        {
            return Same(theValue);
        }

        public override bool LesserOrEqual(object theValue)
        {
            return !Same(theValue);
        }

        public override bool Lesser(object theValue)
        {
            return !Same(theValue);
        }

        public override bool NotSame(object theValue)
        {
            return !Same(theValue);
        }
    }
}