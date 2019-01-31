using System.Linq;
using System.Numerics;
using System.Text;

namespace Eir.Common.Common
{
    /// <summary>
    /// Base36 is a good way of shortening numerical values, 
    /// and uses lower ascii values only which eases transport.
    /// .net does not have a own implimentation of this.
    /// 
    /// This implimentation is only for numbers and is not for binary because
    /// there is not padding for bits.
    /// 
    /// Advantages.
    /// Base 36 is human readable,
    /// can be transmittet safely
    /// case insesitive 
    /// safe file names.
    /// 
    /// Bjarni 2017
    /// </summary>
    public class Base36NumberEncoder
    {

        private const string ACCEPTED_VALUES = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Decode a Base36 encoded string 
        /// Case insensitive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger Decode(string value)
        {
            var sign = 1;

            if (value.StartsWith("-"))
            {
                sign = -1;
                value = value.Substring(1);
            }

            var rev = value.ToLower().Reverse();
            var res = BigInteger.Zero;
            var pos = 0;
            foreach (char c in rev)
            {
                res = BigInteger.Add(res, BigInteger.Multiply(ACCEPTED_VALUES.IndexOf(c), BigInteger.Pow(36, pos)));
                pos++;
            }
            return sign * res;
        }

        /// <summary>
        /// Encode number into Base36 string for shortening
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Base36 strting</returns>
        public string Encode(BigInteger value)
        {
            var sign = "";
            if (value.Sign < 0)
            {
                sign = "-";
                value *= -1;
            }

            var result = new StringBuilder();
            while (!value.IsZero)
            {
                var index = (int)(value % 36);
                result.Insert(0,ACCEPTED_VALUES[index]);
                value = BigInteger.Divide(value, 36);
            }

            return sign + result;
        }
    }

    /// <summary>
    /// Slightly better compression than base32.
    /// </summary>
    public class Base62NumberEncoder
    {

        private const string ACCEPTED_VALUES = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly int _encodingLength = ACCEPTED_VALUES.Length;
        /// <summary>
        /// Decode a previously Base62 encoded string
        ///<para>
        /// This is based on the Base32 encoding and the base52 (bitcoin) encoding
        /// but ignores the mixed letter problem addressed in base52
        /// </para> 
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BigInteger Decode(string value)
        {
            var sign = 1;

            if (value.StartsWith("-"))
            {
                sign = -1;
                value = value.Substring(1);
            }

            var rev = value.Reverse();
            var res = BigInteger.Zero;
            var pos = 0;
            foreach (char c in rev)
            {
                res = BigInteger.Add(res, BigInteger.Multiply(ACCEPTED_VALUES.IndexOf(c), BigInteger.Pow(_encodingLength, pos)));
                pos++;
            }
            return sign * res;
        }

        /// <summary>
        /// Encode number into Base62 string for shortening
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Base62 string</returns>
        public string Encode(BigInteger value)
        {
            var sign = "";
            if (value.Sign < 0)
            {
                sign = "-";
                value *= -1;
            }

            var result = new StringBuilder();
            while (!value.IsZero)
            {
                var index = (int)(value % _encodingLength);
                result.Insert(0,ACCEPTED_VALUES[index]);
                value = BigInteger.Divide(value, _encodingLength);
            }
            return sign + result;
        }
    }
}
//speed test.
//var a = new Base62NumberEncoder();
//var b = new Base36NumberEncoder();
//var res = a.Encode(BigInteger.Parse("80445317051817052100000"));
//var res2 = b.Encode(BigInteger.Parse("80445317051817052100000"));
//Trace.WriteLine(res);
//Trace.WriteLine(a.Decode(res));
//res = a.Encode(BigInteger.Parse("-80445317051817052100000"));
//Trace.WriteLine(res);
//Trace.WriteLine(a.Decode(res));
//var time = new Stopwatch();

//long bTime = 0;
//long aTime = 0;

//for (int ii = 0; ii< 100; ii++)
//{
//time.Start();
//for (int i = 0; i< 1000; i++)
//{
//    b.Encode(BigInteger.Parse("804453154545454545878788972412358877051817052100000" + i* 3));
//}
//time.Stop();

//bTime += time.ElapsedMilliseconds;
//time.Reset();
//time.Start();
//for (int i = 0; i< 1000; i++)
//{
//    a.Encode(BigInteger.Parse("804453154545454545878788972412358877051817052100000" + i* 3));
//}
//time.Stop();

//aTime += time.ElapsedMilliseconds;
//time.Reset();

//}
//Trace.WriteLine("=================================");
//Trace.WriteLine(bTime);
//Trace.WriteLine(aTime);