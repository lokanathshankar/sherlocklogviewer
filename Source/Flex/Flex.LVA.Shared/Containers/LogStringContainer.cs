namespace Flex.LVA.Shared.Containers
{
    using System.Globalization;

    using Flex.LVA.Shared.Exceptions;

    internal class LogStringContainer : LogContainer
    {
        internal string ValueTyped { get; init; }

        public override object Value => this.ValueTyped;
        public override string ToString() => this.ValueTyped;

        public override bool Equals(object obj)
        {
            return this.Same(obj);
        }

        public override bool Contains(object theValue)
        {
            if (theValue is string aValue)
            {
                return ValueTyped.Contains(aValue);
            }

            return ValueTyped.Contains(theValue.ToString() ?? throw new QueryFailureException($"Unable to check for contains on null object, this.Value {this.ValueTyped}"));
        }

        public override bool Greater(object theValue)
        {
            return GreaterOrEqual(theValue);
        }

        public override bool GreaterOrEqual(object theValue)
        {
            if (theValue is string aValue)
            {
                return string.Compare(ValueTyped, aValue, StringComparison.Ordinal) >= 0;
            }

            return string.Compare(ValueTyped, theValue.ToString(), StringComparison.Ordinal) >= 0;
        }

        public override bool Lesser(object theValue)
        {
            return !GreaterOrEqual(theValue);
        }

        public override bool LesserOrEqual(object theValue)
        {
            return !Greater(theValue);
        }

        public override bool NotSame(object theValue)
        {
            return !Same(theValue);
        }

        public override bool Same(object theValue)
        {
            if (theValue is string aValue)
            {
                return aValue == ValueTyped;
            }

            return theValue.ToString() == ValueTyped;
        }
    }
}