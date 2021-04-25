using System;
using System.Text;

namespace LA2RTS
{
    public class Utils
    {
        static public Random RNG = new Random();
        static private long _nextClientID = 1;
        static private object _lock = new object();

        static public int ReadIntFromBuff(byte[] packet, ref int currentOffeset)
        {
            int res = BitConverter.ToInt32(packet, currentOffeset);
            currentOffeset += 4;
            return res;
        }

        static public long ReadLongFromBuff(byte[] packet, ref int currentOffeset)
        {
            long res = BitConverter.ToInt64(packet, currentOffeset);
            currentOffeset += 8;
            return res;
        }

        static public ushort ReadUInt16FromBuff(byte[] packet, ref int currentOffeset)
        {
            ushort res = BitConverter.ToUInt16(packet, currentOffeset);
            currentOffeset += 2;
            return res;
        }

        static public double ReadDoubleFromBuff(byte[] packet, ref int currentOffeset)
        {
            double res = BitConverter.ToDouble(packet, currentOffeset);
            currentOffeset += 8;
            return res;
        }

        static public string ReadStrFromBuff(byte[] packet, ref int currentOffeset)
        {
            int len = BitConverter.ToInt32(packet, currentOffeset);
            currentOffeset += 4;
            string res = Encoding.Unicode.GetString(packet, currentOffeset, len);
            currentOffeset += res.Length * 2;
            return res;
        }

        static public bool ReadBoolFromBuff(byte[] packet, ref int currentOffeset)
        {
            bool res = !(packet[currentOffeset] == 0);
            currentOffeset += 1;
            return res;
        }

        static public byte ReadByteFromBuff(byte[] packet, ref int currentOffeset)
        {
            byte res = packet[currentOffeset];
            currentOffeset += 1;
            return res;
        }

        static public void PutStrInBuff(byte[] packet, string str, ref int currentOffset)
        {
            BitConverter.GetBytes(str.Length).CopyTo(packet, currentOffset);
            currentOffset += 4;
            Encoding.Unicode.GetBytes(str).CopyTo(packet, currentOffset);
            currentOffset += str.Length * 2;
        }

        static public void PutIntInBuff(byte[] packet, int val, ref int currentOffset)
        {
            BitConverter.GetBytes(val).CopyTo(packet, currentOffset);
            currentOffset += 4;
        }

        static public void PutBoolInBuff(byte[] packet, bool val, ref int currentOffset)
        {
            packet[currentOffset] = (byte) (val ? 1 : 0);
            currentOffset += 1;
        }

        static public long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }

        static public long GenerateClientID()
        {
            long r;
            lock (_lock)
            {
                r = _nextClientID++;
            }
            return r;
        }


    }
}
