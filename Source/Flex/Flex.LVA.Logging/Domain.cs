namespace Flex.LVA.Logging
{
    public class Domain
    {
        private readonly string myDomainName;

        public Domain(Type theDomain) : this(theDomain.FullName)
        {
        }

        public Domain(string theDomain)
        {
            myDomainName = theDomain;
        }

        public override string ToString()
        {
            return myDomainName;
        }
    }
}
