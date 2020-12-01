using System;

namespace SnowFlakeDotnetCore
{
    public class SnowFlakeShorter
    {
        public SnowFlakeShorter(long appid, long machineid)
        {
            appId = appid;
            machineId = machineid;
            init();
        }
        private int _timestampLength = 7;
        private int _appIdLength = 2;
        private int _machineIdLength = 4;
        private int _sequenceLength = 2;
        private int _encodingBitsPerChar = 5;
        private long _lastTimestamp = 0L;
        private long _sequence = 0L;
        private long twepoch = 1606266525537L;
        private char[] base32BitMap = new char[32];
        private long appId = 0L;
        private long machineId = 0L;

        private bool include(char[] excludes, char c)
        {
            for (int i = 0; i < excludes.Length; i++)
            {
                if (excludes[i] == c) return true;
            }
            return false;
        }
        private void init()
        {
            char[] excludes = new char[] { 'o', 'l', 'g', 'q' };
            int pushIndex = 0;
            for (char i = '0'; i <= '9'; i++)
            {
                base32BitMap[pushIndex] = i;
                pushIndex++;
            }
            for (char i = 'a'; i <= 'z'; i++)
            {
                if (!include(excludes, i) && pushIndex < base32BitMap.Length)
                {
                    base32BitMap[pushIndex] = i;
                    pushIndex++;
                }
            }

        }
        protected long getTimestamp()
        {
            DateTime datetime = new DateTime(1970,1,1,0,0,0);
            Double dTimestamp = Math.Floor((double)(((DateTime.UtcNow.Ticks - datetime.Ticks) / 10000 - twepoch) / 1000));
            return (long)dTimestamp;
        }
        private int extractBitsFrom(long n, int begin, int end)
        {
            return (int)((n % Math.Pow(2, end) - n % Math.Pow(2, begin)) / Math.Pow(2, begin));
        }
        private String base32encode(long number, int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                var bits = extractBitsFrom(number, i * _encodingBitsPerChar, (i + 1) * _encodingBitsPerChar);
                chars[i] = base32BitMap[bits];
            }
            String str_join = "";
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                str_join += "" + chars[i];
            }
            return str_join;
        }
        private void resetStateIfNewTimestamp(long timestamp)
        {
            if (timestamp > _lastTimestamp)
            {
                _sequence = 0;
                _lastTimestamp = timestamp;
            }
        }

        private long fixedTimestampIfClockShiftedBack(long timestamp)
        {
            if (timestamp < _lastTimestamp)
            {
                timestamp = _lastTimestamp;
            }
            return timestamp;
        }

        private void updateSequenceAndTimestamp()
        {
            _sequence = _sequence + 1;
            _sequence = (long)(_sequence % Math.Pow(2, _sequenceLength * _encodingBitsPerChar));
            if (_sequence == 0)
            {
                _lastTimestamp = _lastTimestamp + 1;
            }
        }



        public String nextId()
        {
            long timestamp = getTimestamp();
            resetStateIfNewTimestamp(timestamp);
            timestamp = fixedTimestampIfClockShiftedBack(timestamp);
            String _appIdCode = base32encode(appId, _appIdLength);
            String _machineIdCode = base32encode(machineId, _machineIdLength);
            String timestampCode = base32encode(timestamp, _timestampLength);
            String sequenceCode = base32encode(_sequence, _sequenceLength);

            updateSequenceAndTimestamp();

            return timestampCode + _appIdCode + _machineIdCode + sequenceCode;
        }
    }
}
