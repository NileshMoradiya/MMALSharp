﻿// <copyright file="StreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using MMALSharp.Common.Utility;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    /// <typeparam name="T">The <see cref="Stream"/> type.</typeparam>
    public abstract class StreamCaptureHandler<T> : OutputCaptureHandlerProcessor
        where T : Stream
    {
        /// <summary>
        /// A Stream instance that we can process image data to.
        /// </summary>
        public T CurrentStream { get; protected set; }
        
        /// <summary>
        /// The total size of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }

        /// <inheritdoc />
        public override void Process(byte[] data, bool eos)
        {
            this.Processed += data.Length;
                        
            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            try
            {
                MMALLog.Logger.Info($"Successfully processed {Helpers.ConvertBytesToMegabytes(this.Processed)}.");
                
                if (this.CurrentStream != null && this.CurrentStream.Length > 0)
                {
                    if (this.OnManipulate != null && this.ImageContext != null)
                    {
                        byte[] arr = null;

                        using (var ms = new MemoryStream())
                        {
                            this.CurrentStream.Position = 0;
                            this.CurrentStream.CopyTo(ms);

                            arr = ms.ToArray();

                            this.ImageContext.Data = arr;
                            
                            this.OnManipulate(new FrameProcessingContext(this.ImageContext));
                        }

                        using (var ms = new MemoryStream(this.ImageContext.Data))
                        {   
                            this.CurrentStream.SetLength(0);
                            this.CurrentStream.Position = 0;
                            ms.CopyTo(this.CurrentStream);
                        }    
                    }
                }
            }
            catch(Exception e)
            {
                MMALLog.Logger.Warn($"Something went wrong while processing stream: {e.InnerException?.Message}. {e.StackTrace}");
            }
        }
        
        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{Helpers.ConvertBytesToMegabytes(this.Processed)}";
        }

        /// <summary>
        /// Releases the underlying stream.
        /// </summary>
        public override void Dispose()
        {
            CurrentStream?.Dispose();
        }
    }
}
