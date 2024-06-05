namespace Flex.LVA.Shared.Containers
{
    public abstract class LogContainer
    {
        public abstract bool Same(object theValue);

        public abstract bool Contains(object theValue);

        public abstract bool Greater(object theValue);

        public abstract bool GreaterOrEqual(object theValue);

        public abstract bool LesserOrEqual(object theValue);

        public abstract bool Lesser(object theValue);

        public abstract bool NotSame(object theValue);

        public abstract object Value { get; }

    }
}