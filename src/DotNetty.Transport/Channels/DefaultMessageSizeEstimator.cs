﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Transport.Channels
{
    using DotNetty.Buffers;

    public sealed class DefaultMessageSizeEstimator : IMessageSizeEstimator
    {
        sealed class HandleImpl : IMessageSizeEstimatorHandle
        {
            readonly int _unknownSize;

            public HandleImpl(int unknownSize)
            {
                _unknownSize = unknownSize;
            }

            public int Size(object msg)
            {
                return msg switch
                {
                    IByteBuffer byteBuffer => byteBuffer.ReadableBytes,
                    IByteBufferHolder byteBufferHolder => byteBufferHolder.Content.ReadableBytes,
                    IFileRegion fileRegion => 0,
                    _ => _unknownSize,
                };
            }
        }

        /// <summary>
        /// Returns the default implementation, which returns <c>8</c> for unknown messages.
        /// </summary>
        public static readonly IMessageSizeEstimator Default = new DefaultMessageSizeEstimator(8);

        readonly IMessageSizeEstimatorHandle _handle;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="unknownSize">The size which is returned for unknown messages.</param>
        public DefaultMessageSizeEstimator(int unknownSize)
        {
            if ((uint)unknownSize > SharedConstants.TooBigOrNegative) // < 0
            {
                ThrowHelper.ThrowArgumentException_PositiveOrZero(unknownSize, ExceptionArgument.unknownSize);
            }
            _handle = new HandleImpl(unknownSize);
        }

        public IMessageSizeEstimatorHandle NewHandle() => _handle;
    }
}