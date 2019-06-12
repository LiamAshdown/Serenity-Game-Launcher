using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteerStone.Entry;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Forms;
using System.Threading;

namespace SteerStone.ZipHandler
{
    class Zip
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Zip(ref BackgroundWorker p_Worker, ref DoWorkEventArgs p_WorkEvent)
        {
            m_Worker = p_Worker;
            m_WorkEvent = p_WorkEvent;
        }

        /// <summary>
        /// SharpZip - Unpack zip file is user selected directory
        /// </summary>
        /// <param name="p_ZipStream"></param>
        /// <param name="p_OutFolder"></param>
        public void UnzipFromStream(Stream p_ZipStream, string p_OutFolder)
        {
            InitializeData(p_ZipStream);

            m_ZipInputStream = new ZipInputStream(m_MemoryStream);
            m_ZipEntry = m_ZipInputStream.GetNextEntry();

            while (m_ZipEntry != null)
            {
                /// Increment counter to show user we are reading x out of y
                m_Counter++;

                /// Name of file we are downloading
                string l_EntryFileName = m_ZipEntry.Name;
                /// Maximum we are reading per loop
                byte[] l_Buffer = new byte[4096];

                /// Manipulate the output filename here as desired.
                String l_FullZipPath = Path.Combine(p_OutFolder, l_EntryFileName);
                string l_DirectoryName = Path.GetDirectoryName(l_FullZipPath);
                if (l_DirectoryName.Length > 0)
                    Directory.CreateDirectory(l_DirectoryName);

                // Skip directory entry
                string l_FileName = Path.GetFileName(l_FullZipPath);
                if (l_FileName.Length == 0)
                {
                    m_ZipEntry = m_ZipInputStream.GetNextEntry();
                    continue;
                }

                /// Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                /// of the file, but does not waste memory.
                /// The "using" will close the stream even if an exception occurs.
                using (FileStream l_StreamWriter = File.Create(l_FullZipPath))
                {
                    StreamUtils.Copy(m_ZipInputStream, l_StreamWriter, l_Buffer, ProgressHandler, new TimeSpan(), string.Empty, l_EntryFileName);
                }
                m_ZipEntry = m_ZipInputStream.GetNextEntry();
            }
        }

        /// <summary>
        /// Get total bytes of zip file and total item count
        /// </summary>
        /// <param name="p_Stream"></param>
        private void InitializeData(Stream p_Stream)
        {
            m_MemoryStream = new MemoryStream();
            p_Stream.CopyTo(m_MemoryStream);
            m_MemoryStream.Position = 0;
        }

        /// <summary>
        /// This is called every time we copy data from stream, we call UI thread to update labels
        /// </summary>
        /// <param name="p_Sender"></param>
        /// <param name="p_Argument"></param>
        private void ProgressHandler(object p_Sender, ProgressEventArgs p_Argument)
        {
            /// If thread is requested to be paused, sleep for 1 second and check if we are still paused
            while (MainEntry.GetInstance.GetMain().IsPaused())
                Thread.Sleep(1000);

            /// Only way I can think of doing this at the moment. TODO; Clean this
            m_TotalProcessed += 4096;
            if (m_TotalProcessed <= p_Argument.Processed)
                m_TotalProcessed = p_Argument.Processed;

            double l_Progress = ((double)p_Argument.Processed / m_ZipEntry.Size) * 100;
            double l_TotalProgress = ((double)m_TotalProcessed / m_MemoryStream.Length) * 100;

            /// Update the labels on UI thread
            MainEntry.GetInstance.GetMain().Invoke((MethodInvoker)delegate 
            {
                /// Convert into megabytes to easier reading
                MainEntry.GetInstance.GetMain().UpdateDownloadProgress(l_Progress, l_TotalProgress, (int)((double)p_Argument.Processed * 0.000001), (int)((double)(int)m_ZipEntry.Size * 0.000001), m_ZipEntry.Name,
                    m_Counter, m_TotalCounter);
            });
        }

        private BackgroundWorker m_Worker;
        private DoWorkEventArgs m_WorkEvent;
        private ZipInputStream m_ZipInputStream;
        private ZipEntry m_ZipEntry;
        private uint m_Counter = 0; ///< Used to get the current file index we are currently reading from
        private uint m_TotalCounter = 1; ///< Total count of files in zip
        private long m_TotalProcessed; ///< How much we processed so far in total
        private MemoryStream m_MemoryStream;
    }
}
