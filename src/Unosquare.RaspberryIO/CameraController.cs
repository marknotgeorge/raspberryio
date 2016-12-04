﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public class CameraController
    {
        private static CameraController m_Instance = null;

        private static readonly object SyncLock = new object();
        private const string RaspiVideo = "raspivid";

        static internal CameraController Instance
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new CameraController();
                    }

                    return m_Instance;
                }
            }
        }

        public bool IsBusy { get; private set; }

        public async Task<byte[]> CaptureJpeg(int width, int height, CancellationToken ct)
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var arguments = new PictureArguments()
                {
                    Width = width,
                    Height = height,
                    Quality = 90
                };

                var process = arguments.CreateProcess();
                if (process.Start() == false)
                    return new byte[] { };

                var outputStream = new MemoryStream();

                { // replacement for fined graned control equivalent to process.StandardOutput.BaseStream.CopyToAsync
                    var swapBuffer = new byte[2048];
                    var readCount = -1;

                    process.StandardOutput.DiscardBufferedData();
                    while (ct.IsCancellationRequested == false)
                    {
                        readCount = await process.StandardOutput.BaseStream.ReadAsync(swapBuffer, 0, swapBuffer.Length, ct);
                        if (readCount <= 0) break;
                        await outputStream.WriteAsync(swapBuffer, 0, readCount);
                    }

                }

                process.WaitForExit();
                return outputStream.ToArray();
            }, ct);

            try
            {
                if (IsBusy)
                    throw new InvalidOperationException("Cannot use camera module because it is currently busy.");

                IsBusy = true;
                var result = await task.Unwrap();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}
