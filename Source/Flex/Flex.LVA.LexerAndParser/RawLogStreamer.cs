using Flex.LVA.Core.Interfaces;
using Flex.LVA.Logging;
using System.Buffers;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Flex.LVA.Core
{
    internal class RawLogStreamer : IRawLogStore, IDisposable
    {
        internal const string ErrorConstant = "Error reading raw log...";

        private Domain myDomain;

        private long myId;

        private StreamReader myBaseStream;

        private long myStreamLength;

        private string myFilePath;

        public Stream Writer { get; private set; }

        private TextReader myFileReader;

        private readonly IDictionary<int, Range> myRanges = new Dictionary<int, Range>();

        internal RawLogStreamer(long theId)
        {
            this.myDomain = new Domain($"{typeof(RawLogStreamer)}.{theId}");
            this.myId = theId;
            myFilePath = Path.Combine(FlexDirectories.LvaTempDir, $"{myId}.txt");
            SetupWriter();
        }

        private void SetupWriter()
        {
            if (Writer != null)
            {
                return;
            }

            Writer = new FileStream(myFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            using (var aTracer = new Tracer(myDomain))
            {
                this.myRanges.Clear();
                FreeUpFileReader();
                FreeUpFileWriter();
                if (!File.Exists(myFilePath))
                {
                    aTracer.Debug($"File {myFilePath} Does Not Exist...");
                    return;
                }

                File.Delete(myFilePath);
                aTracer.Info($"File {myFilePath} Cleaned Up...");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ResetStore()
        {
            this.myRanges.Clear();
            FreeUpFileWriter();
            FreeUpFileReader();
            myStreamLength = 0;
            File.Delete(myFilePath);
            SetupWriter();
        }

        private void FreeUpFileWriter()
        {
            if (this.Writer != null)
            {
                this.Writer.Close();
                this.Writer.Dispose();
                this.Writer = null;
            }
        }

        private void FreeUpFileReader()
        {
            if (this.myFileReader != null)
            {
                this.myFileReader.Close();
                this.myFileReader.Dispose();
                this.myFileReader = null;
                this.myBaseStream = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadStore(in string theFallbackString)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                try
                {
                    if (this.Writer == null)
                    {
                        SetupFallbackReader(theFallbackString);
                        aTracer.Info("Raw Log Streamer Setup Done With String Cache Due To Invalid Writer...");
                        return;
                    }

                    FreeUpFileWriter();
                    myFileReader = File.OpenText(myFilePath);
                    myBaseStream = myFileReader as StreamReader;
                    myStreamLength = myBaseStream.BaseStream.Length;
                    aTracer.Info("Raw Log Streamer Setup Done With File Cache...");
                }
                catch (IOException)
                {
                    SetupFallbackReader(theFallbackString);
                    aTracer.Info("Raw Log Streamer Setup Done With String Cache...");
                }
                catch (UnauthorizedAccessException)
                {
                    SetupFallbackReader(theFallbackString);
                    aTracer.Info("Raw Log Streamer Setup Done With String Cache...");
                }
               
                aTracer.Info($"Load Store Setup With Stream Length Of {myStreamLength}, Input String Length {theFallbackString.Length} Base Stream Length {myStreamLength} For Id {myId}");
            }

            void SetupFallbackReader(string theFallbackString)
            {
                myFileReader = new StringReader(theFallbackString);
                myStreamLength = theFallbackString.Length;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool GetString(int theRangeId, out string theString)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                theString = ErrorConstant;
                if (myFileReader == null)
                {
                    aTracer.Error("Raw Log Reader Not Ready...");
                    return false;
                }

                if (!myRanges.TryGetValue(theRangeId, out Range aRange))
                {
                    aTracer.Error($"No Range Found For ID : {theRangeId}");
                    return false;
                }

                int aLength = aRange.End.Value - aRange.Start.Value;
                char[] aBuffer = ArrayPool<char>.Shared.Rent(aLength);
                myBaseStream.BaseStream.Seek(aRange.Start.Value, SeekOrigin.Begin);
                myBaseStream.DiscardBufferedData();
                myFileReader.Read(aBuffer, 0, aLength);
                theString = new string(aBuffer, 0, aLength);
                ArrayPool<char>.Shared.Return(aBuffer);
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]

        public bool StoreRange(int theRangeId, int theStartIndex, int theEndIndex)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                if (theEndIndex > myStreamLength)
                {
                    aTracer.Error($"End Index {theEndIndex} Exceeds Stream Capacity Of {myStreamLength}");
                    return false;
                }

                return myRanges.TryAdd(theRangeId, new Range(theStartIndex, theEndIndex));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool UpdateRange(int theRangeId, int theEndIndex)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                if (theEndIndex > myStreamLength)
                {
                    aTracer.Error($"End Index {theEndIndex} Exceeds Stream Capacity Of {myStreamLength}");
                    return false;
                }

                if (!myRanges.TryGetValue(theRangeId, out Range aRange))
                {
                    aTracer.Error($"No Range Found For ID : {theRangeId}");
                    return false;
                }

                myRanges[theRangeId] = new Range(aRange.Start.Value, theEndIndex);
                return true;
            }
        }
    }
}