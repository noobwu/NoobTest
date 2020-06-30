using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace KeMai.Tests.Tests
{
    [TestClass]
    public static class IdTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            int count = 2000000;
            List<string> cardCodeList = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                //cardCodeList.Add(CardCode());
                //cardCodeList.Add(CardCode());
                //cardCodeList.Add(Guid.NewGuid().ToString());
                //cardCodeList.Add(SQLStringToHash(Guid.NewGuid().ToString(), 0).ToString());
                //cardCodeList.Add(Guid.NewGuid().ToString().GetHashCode().ToString());
            }
            Console.WriteLine("Distinct Count:"+cardCodeList.Distinct().Count()+ ",count:"+count);
            Console.WriteLine("MaxValue:"+int.MaxValue);
        }
        [EdmFunctionAttribute("SqlServer", "CHECKSUM")]
        public static Nullable<int> Checksum(
    string arg1,
    string arg2,
    string arg3
)
        public string CardCode()
        {
            string guid = Guid.NewGuid().ToString();
            long checkSum = SQLBinaryChecksum(guid);
            //if (checkSum >= 0)
            //{
            //    return checkSum.ToString().PadLeft(10, '0');
            //}
            //return unchecked(int.MaxValue * 2 + 1 + checkSum).ToString().PadLeft(10, '0');
            return checkSum.ToString();
        }

        private long SQLBinaryChecksum(string text)
        {
            long sum = 0;
            byte overflow;
            for (int i = 0; i < text.Length; i++)
            {
                sum = (long)((16 * sum) ^ Convert.ToUInt32(text[i]));
                overflow = (byte)(sum / 4294967296);
                sum = sum - overflow * 4294967296;
                sum = sum ^ overflow;
            }

            if (sum > 2147483647)
                sum = sum - 4294967296;
            else if (sum >= 32768 && sum <= 65535)
                sum = sum - 65536;
            else if (sum >= 128 && sum <= 255)
                sum = sum - 256;

            return sum;
        }

        static System.Security.Cryptography.MD5CryptoServiceProvider md5Provider =
   new System.Security.Cryptography.MD5CryptoServiceProvider();
        // the database is usually set to Latin1_General_CI_AS which is codepage 1252
        static System.Text.Encoding encoding =
           System.Text.Encoding.GetEncoding(1252);

        static Int32 SQLStringToHash(string sourceString, int modulo = 0)
        {
            var md5Bytes = md5Provider.ComputeHash(encoding.GetBytes(sourceString));
            var result = BitConverter.ToInt32(new byte[] {
      md5Bytes[15], md5Bytes[14], md5Bytes[13], md5Bytes[12] }, 0);
            if (modulo == 0) return result;
            else return Math.Abs(result) % modulo;
        }
    }
}
