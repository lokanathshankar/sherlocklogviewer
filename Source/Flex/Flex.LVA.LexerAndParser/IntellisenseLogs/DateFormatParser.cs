using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Flex.LVA.Core.IntellisenseLogs
{
    internal class DateFormatParser
    {
    }

    internal class TimeFormatParser
    {
        private static char[] mySaperators = new char[] { ':', '.', '/', ',', '_', ' ' };


        internal static string PredictFormat(string theInput)
        {
            theInput = theInput.Trim();
            theInput = CleanupOffset(theInput);
            IList<string> aSplits = InterpolatedSplits(theInput);
            if (aSplits.Count > 7)
            {
                throw new ArgumentException("Time is too long to parse, format deduction not possible.");
            }

            //// HH MM SS FFFFFFF TT ZZZ PST/IST/GST  I am straight up ignoring the last one, just put it at the end and framework take care of it
            //// let's assume Hours first, minutes, seconds, millisecs, AM/PM, Offset and then Time Zone
            //// 22 60 55 888888 PM 5:30 GST
            IList<IList<string>> aFormat = new List<IList<string>>();
            bool aMsDetected = false;
            bool aOffsetDetected = false;
            bool aLiteralDetected = false;
            bool aHoursDetected = false;
            bool aSecondsDetected = false;
            bool aMinutesDetected = false;
            bool aAmPmDetected = false;
            foreach (string aItem in aSplits)
            {
                //// Only possible thing is millisecs or literals
                if (aItem.IsPureString())
                {
                    // AM PM or IST stuff
                    if (IsAmPm(aItem, aAmPmDetected))
                    {
                        if (aItem.Length == 1)
                        {
                            aAmPmDetected = UpdateAmPmStuff(aFormat, "t");
                            continue;
                        }
                        else
                        {
                            aAmPmDetected = UpdateAmPmStuff(aFormat, "tt");
                            continue;
                        }
                    }
                    else if (IsLiteral(aItem, aLiteralDetected))
                    {
                        aLiteralDetected = UpdateLiteralStuff(aFormat, aItem);
                        continue;
                    }
                }
                else if (!aMsDetected)
                {
                    if (aItem.Length == 7)
                    {
                        aMsDetected = UpdateMsStuff(aFormat, "fffffff");
                    }

                    if (aItem.Length == 6)
                    {
                        aMsDetected = UpdateMsStuff(aFormat, "ffffff");
                    }

                    if (aItem.Length == 5)
                    {
                        aMsDetected = UpdateMsStuff(aFormat, "fffff");
                    }
                }


                if (aItem.Length == 4)
                {
                    //// +400 being a offser can endup here along with ms
                    if (IsOffset(aItem, aOffsetDetected))
                    {
                        // This means it's offset
                        aOffsetDetected = UpdateOffsetStuff(aFormat, "zzz");
                    }
                    else
                    {
                        aMsDetected = UpdateMsStuff(aFormat, "ffff");
                    }
                }

                //// possible things are PST/LITERAL, OffSet or ms
                if (aItem.Length == 3)
                {
                    if (IsLiteral(aItem, aLiteralDetected))
                    {
                        aLiteralDetected = UpdateLiteralStuff(aFormat, aItem);
                    }
                    else
                    {
                        if (IsOffset(aItem, aOffsetDetected))
                        {
                            // This means it's offset
                            aOffsetDetected = UpdateOffsetStuff(aFormat, "zz");
                        }
                        else if (!aMsDetected)
                        {
                            aFormat.Add(new List<string>() { "fff" });
                            aMsDetected = true;
                        }
                    }
                }

                //// possible things are hours, min, sec, ms, am/pm , offset, literals

                if (aItem.Length == 2)
                {
                    string aHours = "HH";
                    if (IsAmPmPresentInString(theInput))
                    {
                        aHours = "hh";
                    }

                    SomethingWhichICanReuse(
                        aFormat,
                        ref aMsDetected,
                        ref aOffsetDetected,
                        ref aLiteralDetected,
                        ref aHoursDetected,
                        ref aSecondsDetected,
                        ref aMinutesDetected,
                        ref aAmPmDetected,
                        aItem,
                        "tt", "z", "ff", aHours, "mm", "ss");
                }

                if (aItem.Length == 1)
                {
                    string aHours = "H";
                    if (IsAmPmPresentInString(theInput))
                    {
                        aHours = "h";
                    }

                    SomethingWhichICanReuse(
                        aFormat,
                        ref aMsDetected,
                        ref aOffsetDetected,
                        ref aLiteralDetected,
                        ref aHoursDetected,
                        ref aSecondsDetected,
                        ref aMinutesDetected,
                        ref aAmPmDetected,
                        aItem,
                        "t", string.Empty, "f", aHours, "m", "s");
                }
            }

            foreach (var aEle in aFormat)
            {
                if (aEle.Count > 1 || aEle.Count == 0)
                {
                    throw new ArgumentException("Input data has lead to internal error");
                }
            }

            {
                // Rebuild format
                char[] aSepToFillIn = string.Join(' ', aFormat.Select(theX => theX[0])).ToCharArray();
                if (aSepToFillIn.Length == 0)
                {
                    throw new ArgumentException("unable to detect format, check if the time is valid");
                }

                if (aOffsetDetected)
                {
                    if (aSepToFillIn.Length + 2 < theInput.Length)
                    {
                        throw new ArgumentException("Unable to detect complete format, check if the timezones are is valid");
                    }
                }
                else
                {
                    if (aSepToFillIn.Length != theInput.Length)
                    {
                        throw new ArgumentException("Unable to detect complete format, check if the time is valid");
                    }
                }

                theInput = theInput.Replace("+", string.Empty).Replace("-", string.Empty);
                for (int aIndex = 0; aIndex < aSepToFillIn.Length; aIndex++)
                {
                    char aChar = aSepToFillIn[aIndex];
                    if (aChar == ' ')
                    {
                        aSepToFillIn[aIndex] = theInput[aIndex];
                    }
                }

                return new string(aSepToFillIn);
            }
        }

        private static string CleanupOffset(string theInput)
        {
            if (!IsPlusMinusPresent(theInput))
            {
                return theInput;
            }

            int aPlusLoc = theInput.IndexOf('+');
            if (aPlusLoc == -1)
            {
                aPlusLoc = theInput.IndexOf('-');
            }

            if (aPlusLoc == -1)
            {
                throw new ArgumentException("Internal error when parsing time-zone");
            }

            int aCharToRemove = theInput.IndexOf(":", aPlusLoc + 1);
            if (aCharToRemove == -1)
            {
                return theInput;
            }

            // +24:00
            if (aCharToRemove - aPlusLoc > 3)
            {
                return theInput;
            }

            return theInput.Remove(aCharToRemove, 1);
        }

        private static void SomethingWhichICanReuse(
            IList<IList<string>> aFormat,
            ref bool aMsDetected,
            ref bool aOffsetDetected,
            ref bool aLiteralDetected,
            ref bool aHoursDetected,
            ref bool aSecondsDetected,
            ref bool aMinutesDetected,
            ref bool aAmPmDetected,
            string aItem,
            string theT, string theZ, string theF, string theH, string theM, string theS)
        {
            if (IsOffset(aItem, aOffsetDetected))
            {
                aOffsetDetected = UpdateOffsetStuff(aFormat, theZ);
                return;
            }

            if (aItem.IsPureString())
            {
                // It can be AM/PM, literal
                if (IsAmPm(aItem, aAmPmDetected))
                {
                    aAmPmDetected = UpdateAmPmStuff(aFormat, theT);
                }
                else if (IsLiteral(aItem, aLiteralDetected))
                {
                    aLiteralDetected = UpdateLiteralStuff(aFormat, aItem);
                }
            }
            else
            {
                // It can be hours, min, sec, ms
                if (!uint.TryParse(aItem, CultureInfo.InvariantCulture, out uint aNumber))
                {
                    throw new ArgumentException("Unable to determine format.");
                }

                if (aNumber > 60)
                {
                    if (aMsDetected)
                    {
                        throw new ArgumentException("Unable to determine format. Sub-second was already determined.");
                    }

                    // ms case
                    aMsDetected = UpdateMsStuff(aFormat, theF);
                }
                else if (!aMsDetected && aHoursDetected && aMinutesDetected && aSecondsDetected)
                {
                    aMsDetected = UpdateMsStuff(aFormat, theF);
                }
                else
                {
                    // hours, min, sec
                    if (!aHoursDetected && aNumber <= 24)
                    {
                        if (!aHoursDetected)
                        {
                            aHoursDetected = UpdateHourStuff(aFormat, theH);
                        }
                    }
                    else
                    {
                        // Hours get priority followed by min and then seconds
                        if (!aHoursDetected && aNumber <= 24)
                        {
                            aHoursDetected = UpdateHourStuff(aFormat, theH);
                        }
                        else if (aHoursDetected && !aMinutesDetected)
                        {
                            aMinutesDetected = UpdateMinutesStuff(aFormat, theM);
                        }
                        else if (aHoursDetected && aMinutesDetected && !aSecondsDetected)
                        {
                            aSecondsDetected = UpdateSecondStuff(aFormat, theS);
                        }
                    }
                }
            }
        }

        private static bool UpdateAmPmStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool UpdateHourStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool UpdateSecondStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool UpdateMinutesStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool UpdateLiteralStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }
        private static bool UpdateMsStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool UpdateOffsetStuff(IList<IList<string>> aFormat, string theFormatter)
        {
            aFormat.Add(new List<string>() { theFormatter });
            return true;
        }

        private static bool IsLiteral(string aItem, bool theLiteralFlag)
        {
            if (theLiteralFlag)
            {
                return false;
            }

            return aItem.IsPureString();
        }
        private static bool IsAmPm(string aItem, bool theOffsetFlag)
        {
            if (theOffsetFlag)
            {
                return false;
            }

            return IsAmPmPresentInString(aItem);
        }

        private static bool IsAmPmPresentInString(string aItem)
        {
            return aItem.Contains("A", StringComparison.OrdinalIgnoreCase)
                || aItem.Contains("P", StringComparison.OrdinalIgnoreCase)
                || aItem.Contains("AM", StringComparison.OrdinalIgnoreCase)
                || aItem.Contains("PM", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOffset(string aItem, bool theOffsetFlag)
        {
            if (theOffsetFlag)
            {
                return false;
            }

            return IsPlusMinusPresent(aItem);
        }

        private static bool IsPlusMinusPresent(string theInput)
        {
            return theInput.Contains('+') || theInput.Contains('-');
        }

        private static IList<string> InterpolatedSplits(string theInput)
        {
            IList<string> aToReturn = new List<string>();
            int aStartIndex = 0;
            for (int aIndex = 0; aIndex < theInput.Length; aIndex++)
            {
                char aLetter = theInput[aIndex];
                if (mySaperators.Contains(aLetter))
                {
                    for (int aInIndex = aIndex; aInIndex < theInput.Length; aInIndex++)
                    {
                        char aNextLetter = theInput[aInIndex];
                        if (mySaperators.Contains(aNextLetter))
                        {
                            if(aStartIndex == aIndex)
                            {
                                aStartIndex = aInIndex + 1;
                                break;
                            }

                            aToReturn.Add(theInput.Substring(aStartIndex, aInIndex - aStartIndex));
                            aStartIndex = aInIndex + 1;
                            break;
                        }
                    }
                }
            }

            aToReturn.Add(theInput.Substring(aStartIndex, theInput.Length - aStartIndex));
            return aToReturn;
        }
    }

    internal static class StringExts
    {
        internal static int CountCharacterOccurance(this string theString, char theMatch)
        {
            int aCount = 0;
            foreach (char c in theString)
            {
                if (c == theMatch)
                {
                    aCount++;
                }
            }

            return aCount;
        }

        internal static bool IsPureString(this string theString)
        {
            foreach (char c in theString)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
