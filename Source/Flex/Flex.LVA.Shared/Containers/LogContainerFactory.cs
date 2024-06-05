using System.Globalization;

namespace Flex.LVA.Shared.Containers
{

    public class LogContainerFactory
    {
        public LogContainer GetLogContainer(LogElementType theElementType, object theValue, string theFormatter)
        {
            switch (theElementType)
            {
                case LogElementType.String:
                    return new LogStringContainer() { ValueTyped = Convert.ToString(theValue) };
                case LogElementType.Number:
                    if (int.TryParse(theValue.ToString(), out int aParsedValue))
                    {
                        return new LogIntContainer() { ValueTyped = aParsedValue };
                    }
                    else if(float.TryParse(theValue.ToString(), out float aFloatValue))
                    {
                        return new LogFloatContainer() { ValueTyped = aFloatValue };
                    }
                    else
                    {
                        return new LogStringContainer() { ValueTyped = Convert.ToString(theValue) };
                    }
                case LogElementType.Bool:
                    return new LogBoolContainer() { ValueTyped = Convert.ToBoolean(theValue) };
                case LogElementType.Date:
                    if(theValue is DateOnly)
                    {
                        return new LogDateContainer() { ValueTyped = (DateOnly)theValue };
                    }

                    if (string.IsNullOrEmpty(theFormatter))
                    {
                        return new LogDateContainer() { ValueTyped = DateOnly.Parse(theValue.ToString().Trim(), CultureInfo.InvariantCulture) };
                    }
                    else
                    {
                        return new LogDateContainer() { ValueTyped = DateOnly.ParseExact(theValue.ToString().Trim(), theFormatter, CultureInfo.InvariantCulture) };
                    }
                case LogElementType.Time:
                    if (theValue is TimeOnly)
                    {
                        return new LogTimeContainer() { ValueTyped = (TimeOnly)theValue };
                    }

                    if (string.IsNullOrEmpty(theFormatter))
                    {
                        return new LogTimeContainer() { ValueTyped = TimeOnly.Parse(theValue.ToString().Trim(), CultureInfo.InvariantCulture) };
                    }
                    else
                    {
                        return new LogTimeContainer() { ValueTyped = TimeOnly.ParseExact(theValue.ToString().Trim(), theFormatter, CultureInfo.InvariantCulture) };
                    }
                case LogElementType.DateTime:
                    if (theValue is DateTime)
                    {
                        return new LogDateTimeContainer() { ValueTyped = (DateTime)theValue };
                    }

                    if (string.IsNullOrEmpty(theFormatter))
                    {
                        return new LogDateTimeContainer() { ValueTyped = DateTime.Parse(theValue.ToString().Trim(), CultureInfo.InvariantCulture) };
                    }
                    else
                    {
                        return new LogDateTimeContainer() { ValueTyped = DateTime.ParseExact(theValue.ToString().Trim(), theFormatter, CultureInfo.InvariantCulture) };
                    }
            }

            return null;
        }
    }
}