namespace Flex.LVA.Shared.Containers
{
    internal class LogTimeContainer : LogContainer
    {
        internal TimeOnly ValueTyped { get; init; }

        public override object Value => this.ValueTyped;
        public override string ToString() => this.ValueTyped.ToString("HH:mm:ss.fff");

        public override bool Equals(object obj)
        {
            return this.Same(obj);
        }

        public override bool Contains(object theValue)
        {
            throw new NotImplementedException("Lots of stuff needs to be addressed in datetime");
        }

        public override bool Greater(object theValue)
        {
            throw new NotImplementedException();
        }

        public override bool GreaterOrEqual(object theValue)
        {
            throw new NotImplementedException();
        }

        public override bool Lesser(object theValue)
        {
            throw new NotImplementedException();
        }

        public override bool LesserOrEqual(object theValue)
        {
            throw new NotImplementedException();
        }

        public override bool NotSame(object theValue)
        {
            throw new NotImplementedException();
        }

        public override bool Same(object theValue)
        {
            if (theValue is TimeOnly aValue)
            {
                return aValue == ValueTyped;
            }

            if (TimeOnly.TryParse(theValue.ToString(), out aValue))
            {
                return aValue == ValueTyped;
            }

            return false;
        }
    }
}