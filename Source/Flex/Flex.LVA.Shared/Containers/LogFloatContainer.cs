using Flex.LVA.Shared.Exceptions;
using System.Globalization;

namespace Flex.LVA.Shared.Containers
{
    internal class LogFloatContainer : LogContainer
    {
        internal float ValueTyped { get; init; }

        public override object Value => this.ValueTyped;
        public override string ToString() => this.ValueTyped.ToString();

        public override bool Equals(object obj)
        {
            return this.Same(obj);
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
            if (theValue is float aValue)
            {
                return ValueTyped > aValue;
            }

            if (float.TryParse(theValue.ToString(), out aValue))
            {
                return ValueTyped > aValue;
            }

            return false;
        }

        public override bool GreaterOrEqual(object theValue)
        {
            if (theValue is float aValue)
            {
                return ValueTyped >= aValue;
            }

            if (float.TryParse(theValue.ToString(), out aValue))
            {
                return ValueTyped >= aValue;
            }

            return false;
        }

        public override bool Lesser(object theValue)
        {
            return !this.GreaterOrEqual(theValue);
        }

        public override bool LesserOrEqual(object theValue)
        {
            return !this.Greater(theValue);
        }

        public override bool NotSame(object theValue)
        {
            return !Same(theValue);
        }

        public override bool Same(object theValue)
        {
            if (theValue is float aValue)
            {
                return aValue == ValueTyped;
            }

            if (float.TryParse(theValue.ToString(), out aValue))
            {
                return aValue == ValueTyped;
            }

            return false;
        }
    }
}