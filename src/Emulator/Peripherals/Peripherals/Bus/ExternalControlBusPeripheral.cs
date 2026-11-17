//
// Copyright (c) 2010-2026 Antmicro
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using System;

using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;

namespace Antmicro.Renode.Peripherals.Bus
{
    public class ExternalControlBusPeripheral : IKnownSize, IBytePeripheral, IWordPeripheral, IDoubleWordPeripheral, IQuadWordPeripheral, IMultibyteWritePeripheral, IExecutableIO, IAbsoluteAddressAware
    {
        public ExternalControlBusPeripheral(IMachine machine, long size)
        {
            sysbus = machine.GetSystemBus(this);
            Size = size;
        }

        public void Reset()
        {
        }

        public byte ReadByte(long offset)
        {
            return ReadOrLogWarning(OnReadByte, nameof(ReadByte), () => OnReadByte(CurrentAccessAddress));
        }

        public void WriteByte(long offset, byte value)
        {
            WriteOrLogWarning(OnWriteByte, nameof(WriteByte), () => OnWriteByte(CurrentAccessAddress, value));
        }

        public ushort ReadWord(long offset)
        {
            return ReadOrLogWarning(OnReadWord, nameof(ReadWord), () => OnReadWord(CurrentAccessAddress));
        }

        public void WriteWord(long offset, ushort value)
        {
            WriteOrLogWarning(OnWriteWord, nameof(WriteWord), () => OnWriteWord(CurrentAccessAddress, value));
        }

        public uint ReadDoubleWord(long offset)
        {
            return ReadOrLogWarning(OnReadDoubleWord, nameof(ReadDoubleWord), () => OnReadDoubleWord(CurrentAccessAddress));
        }

        public void WriteDoubleWord(long offset, uint value)
        {
            WriteOrLogWarning(OnWriteDoubleWord, nameof(WriteDoubleWord), () => OnWriteDoubleWord(CurrentAccessAddress, value));
        }

        public ulong ReadQuadWord(long offset)
        {
            return ReadOrLogWarning(OnReadQuadWord, nameof(ReadQuadWord), () => OnReadQuadWord(CurrentAccessAddress));
        }

        public void WriteQuadWord(long offset, ulong value)
        {
            WriteOrLogWarning(OnWriteQuadWord, nameof(WriteQuadWord), () => OnWriteQuadWord(CurrentAccessAddress, value));
        }

        public byte[] ReadBytes(long offset, int count, IPeripheral context = null)
        {
            return ReadOrLogWarning(OnReadBytes, nameof(ReadBytes), () => OnReadBytes(CurrentAccessAddress, count));
        }

        public void WriteBytes(long offset, byte[] array, int startingIndex, int count, IPeripheral context = null)
        {
            WriteOrLogWarning(OnWriteBytes, nameof(WriteBytes), () => OnWriteBytes(CurrentAccessAddress, array, startingIndex, count));
        }

        public long Size { get; }

        public Func<ulong, int, byte[]> OnReadBytes;
        public Action<ulong, byte[], int, int> OnWriteBytes;
        public Func<ulong, ulong> OnReadQuadWord;
        public Action<ulong, ulong> OnWriteQuadWord;
        public Func<ulong, uint> OnReadDoubleWord;
        public Action<ulong, uint> OnWriteDoubleWord;
        public Func<ulong, ushort> OnReadWord;
        public Action<ulong, ushort> OnWriteWord;
        public Func<ulong, byte> OnReadByte;
        public Action<ulong, byte> OnWriteByte;

        private T ReadOrLogWarning<T>(Delegate callback, string operation, Func<T> read)
        {
            if(callback is null)
            {
                this.Log(LogLevel.Warning, $"{typeof(ExternalControlBusPeripheral)} peripheral '{this.GetName()}' does not implement {operation} callback, returning 0");
                return default;
            }
            return read();
        }

        private void WriteOrLogWarning(Delegate callback, string operation, Action write)
        {
            if(callback is null)
            {
                this.Log(LogLevel.Warning, $"{typeof(ExternalControlBusPeripheral)} peripheral '{this.GetName()}' does not implement {operation} callback, ignoring write");
                return;
            }
            write();
        }

        private ulong CurrentAccessAddress => sysbus.TryGetCurrentAccessAddress(out var address) ? address : 0;

        private readonly IBusController sysbus;
    }
}
