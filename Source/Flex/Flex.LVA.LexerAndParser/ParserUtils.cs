using Flex.LVA.Shared.Containers;
using Flex.LVA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Flex.LVA.Core
{
    internal static class ParserUtils
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetHeader(LogContainerFactory theContainerFac, bool theValue, LogGraphMutable theParsedLogs)
        {
            string aKey = theValue.ToString();
            if (theParsedLogs.Graph[Constants.Header].TryGetValue(aKey, out object aTemp))
            {
                return aTemp;
            }

            theParsedLogs.Graph[Constants.Header][aKey] = theContainerFac.GetLogContainer(LogElementType.Bool, theValue, null);
            return theParsedLogs.Graph[Constants.Header][aKey];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetFooter(LogContainerFactory theContainerFac, in string theValue, LogGraphMutable theParsedLogs)
        {
            string aKey = theValue;
            if (theParsedLogs.Graph[Constants.Footer].TryGetValue(aKey, out object aTemp))
            {
                return aTemp;
            }

            theParsedLogs.Graph[Constants.Footer][aKey] = theContainerFac.GetLogContainer(LogElementType.String, theValue, null);
            return theParsedLogs.Graph[Constants.Footer][aKey];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetLogColumn(LogContainerFactory theContainerFac, in string theProperty, LogElement theElement, LogGraphMutable theParsedLogs)
        {
            if (theParsedLogs.Graph[theElement.Name].TryGetValue(theProperty, out object aTemp))
            {
                return aTemp;
            }

            theParsedLogs.Graph[theElement.Name][theProperty] = theContainerFac.GetLogContainer(theElement.Type, theProperty, theElement.DateTimeFormat);
            return theParsedLogs.Graph[theElement.Name][theProperty];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetLogColumnString(LogContainerFactory theContainerFac, in string theRawString, in string theProperty, LogGraphMutable theParsedLogs)
        {
            if (theParsedLogs.Graph[theProperty].TryGetValue(theRawString, out object aTemp))
            {
                return aTemp;
            }

            theParsedLogs.Graph[theProperty][theRawString] = theContainerFac.GetLogContainer(LogElementType.String, theRawString, null);
            return theParsedLogs.Graph[theProperty][theRawString];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetLogColumn(LogContainerFactory theContainerFac, in string theRawString, in string theProperty, LogGraphMutable theParsedLogs, string theDateTimeFormat, LogElementType theElementType)
        {
            if (theParsedLogs.Graph[theProperty].TryGetValue(theRawString, out object aTemp))
            {
                return aTemp;
            }

            theParsedLogs.Graph[theProperty][theRawString] = theContainerFac.GetLogContainer(theElementType, theRawString, theDateTimeFormat);
            return theParsedLogs.Graph[theProperty][theRawString];
        }

        internal static string SkipLinesFromString(string theString, int theLinesToSkip)
        {
            if (theLinesToSkip == 0)
            {
                return theString;
            }

            int aLinesSkipped = 0;
            int aStart = -1;
            while (aLinesSkipped < theLinesToSkip)
            {
                aStart = theString.IndexOf('\n', aStart + 1);
                aLinesSkipped++;
            }

            return theString.Substring(++aStart);
        }
    }
}
