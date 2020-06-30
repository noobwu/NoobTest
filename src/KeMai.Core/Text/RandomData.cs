//Github: https://github.com/exceptionless/Exceptionless.RandomData
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class RandomData
    {
        static RandomData()
        {
            Instance = new Random(System.Environment.TickCount);
        }

        public static Random Instance { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetInt(int min, int max)
        {
            if (min == max)
                return min;
            if (min >= max)
            {
                Console.WriteLine("min>=max,min:" + min + ",max:" + max);
                throw new Exception("Min value must be less than max value.");
            }
            if (max < Int32.MaxValue)
            {
                max += 1;
            }
            return Instance.Next(min, max);
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string GetVersion(string min, string max)
        {
            if (String.IsNullOrEmpty(min))
                min = "0.0.0.0";
            if (String.IsNullOrEmpty(max))
                max = "25.100.9999.9999";
            Version minVersion, maxVersion;
            if (!Version.TryParse(min, out  minVersion))
                minVersion = new Version(0, 0, 0, 0);
            if (!Version.TryParse(max, out  maxVersion))
                maxVersion = new Version(25, 100, 9999, 9999);

            minVersion = new Version(
                minVersion.Major != -1 ? minVersion.Major : 0,
                minVersion.Minor != -1 ? minVersion.Minor : 0,
                minVersion.Build != -1 ? minVersion.Build : 0,
                minVersion.Revision != -1 ? minVersion.Revision : 0);

            maxVersion = new Version(
                maxVersion.Major != -1 ? maxVersion.Major : 0,
                maxVersion.Minor != -1 ? maxVersion.Minor : 0,
                maxVersion.Build != -1 ? maxVersion.Build : 0,
                maxVersion.Revision != -1 ? maxVersion.Revision : 0);

            var major = GetInt(minVersion.Major, maxVersion.Major);
            var minor = GetInt(minVersion.Minor, major == maxVersion.Major ? maxVersion.Minor : 100);
            var build = GetInt(minVersion.Build, minor == maxVersion.Minor ? maxVersion.Build : 9999);
            var revision = GetInt(minVersion.Revision, build == maxVersion.Build ? maxVersion.Revision : 9999);

            return new Version(major, minor, build, revision).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetInt()
        {
            return GetInt(DataValueConstants.MIN_INT, DataValueConstants.MAX_INT);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long GetLong(long min, long max)
        {
            if (min == max)
                return min;

            if (min >= max)
                throw new Exception("Min value must be less than max value.");

            var buf = new byte[8];
            Instance.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetLong()
        {
            return GetLong(Int64.MinValue, Int64.MaxValue);
        }
        /// <summary>
        /// 获取随机坐标
        /// </summary>
        /// <returns></returns>
        public static string GetCoordinate()
        {
            return GetDouble(-90.0, 90.0) + "," + GetDouble(-180.0, 180.0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(DateTime? start = null, DateTime? end = null)
        {
            if (start.HasValue && end.HasValue && start.Value == end.Value)
                return start.Value;

            if (start.HasValue && end.HasValue && start.Value >= end.Value)
                throw new Exception("Start date must be less than end date.");

            start = start ?? DataValueConstants.MIN_DATE;
            end = end ?? DataValueConstants.MAX_DATE;

            TimeSpan timeSpan = end.Value - start.Value;
            var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

            return start.Value + newSpan;
        }
        /// <summary>
        /// 获取随机时间点，通常以相对于协调世界时 (UTC) 的日期和时间来表示。
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeOffset(DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            if (start.HasValue && end.HasValue && start.Value >= end.Value)
                throw new Exception("Start date must be less than end date.");

            start = start ?? DateTimeOffset.MinValue;
            end = end ?? DateTimeOffset.MaxValue;

            TimeSpan timeSpan = end.Value - start.Value;
            var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

            return start.Value + newSpan;
        }
        /// <summary>
        /// 获取随机时间间隔
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(TimeSpan? min = null, TimeSpan? max = null)
        {
            if (min.HasValue && max.HasValue && min.Value == max.Value)
                return min.Value;

            if (min.HasValue && max.HasValue && min.Value >= max.Value)
                throw new Exception("Min span must be less than max span.");

            min = min ?? TimeSpan.Zero;
            max = max ?? TimeSpan.MaxValue;

            return min.Value + new TimeSpan((long)(new TimeSpan(max.Value.Ticks - min.Value.Ticks).Ticks * Instance.NextDouble()));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static bool GetBool(int chance = 50)
        {
            chance = Math.Min(chance, 100);
            chance = Math.Max(chance, 0);
            double c = 1 - (chance / 100.0);
            return Instance.NextDouble() > c;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double GetDouble(double? min = null, double? max = null)
        {
            if (min.HasValue && max.HasValue && min.Value == max.Value)
                return min.Value;

            if (min.HasValue && max.HasValue && min.Value >= max.Value)
                throw new Exception("Min value must be less than max value.");

            min = min ?? Double.MinValue;
            max = max ?? Double.MaxValue;

            return Instance.NextDouble() * (max.Value - min.Value) + min.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal()
        {
            return GetDecimal(DataValueConstants.MIN_INT, GetInt());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal GetDecimal(int min, int max)
        {
            return (decimal)GetDouble(min, max);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetEnum<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum type.");

            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(GetInt(0, values.Length - 1));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIp4Address()
        {
            return String.Concat(GetInt(0, 255), ".", GetInt(0, 255), ".", GetInt(0, 255), ".", GetInt(0, 255));
        }
        /// <summary>
        /// 
        /// </summary>
        private const string DEFAULT_RANDOM_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="allowedChars"></param>
        /// <returns></returns>
        public static string GetString(int minLength = 5, int maxLength = 20, string allowedChars = DEFAULT_RANDOM_CHARS)
        {
            if (minLength >= maxLength)
            {
                minLength = 0;
            }
            int length = minLength != maxLength ? GetInt(minLength, maxLength) : minLength;

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            using (var rng = RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];

                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i])
                            continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }

                return result.ToString();
            }
        }

        // Some characters are left out because they are hard to tell apart.
        private const string DEFAULT_ALPHA_CHARS = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetAlphaString(int minLength = 5, int maxLength = 20)
        {
            return GetString(minLength, maxLength, DEFAULT_ALPHA_CHARS);
        }

        // Some characters are left out because they are hard to tell apart.
        private const string DEFAULT_ALPHANUMERIC_CHARS = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetAlphaNumericString(int minLength = 5, int maxLength = 20)
        {
            return GetString(minLength, maxLength, DEFAULT_ALPHANUMERIC_CHARS);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <returns></returns>
        public static string GetTitleWords(int minWords = 2, int maxWords = 10)
        {
            return GetWords(minWords, maxWords, titleCaseAllWords: true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="titleCase"></param>
        /// <returns></returns>
        public static string GetWord(bool titleCase = true)
        {
            return titleCase ? UpperCaseFirstCharacter(_words[GetInt(0, _words.Length - 1)]) : _words[GetInt(0, _words.Length - 1)];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <param name="titleCaseFirstWord"></param>
        /// <param name="titleCaseAllWords"></param>
        /// <returns></returns>
        public static string GetWords(int minWords = 2, int maxWords = 10, bool titleCaseFirstWord = true, bool titleCaseAllWords = true)
        {
            if (minWords < 2)
                throw new ArgumentException("minWords must 2 or more.", "minWords");
            if (maxWords < 2)
                throw new ArgumentException("maxWords must 2 or more.", "maxWords");

            var builder = new StringBuilder();
            int numberOfWords = GetInt(minWords, maxWords);
            for (int i = 1; i < numberOfWords; i++)
                builder.Append(' ').Append(GetWord(titleCaseAllWords || (i == 0 && titleCaseFirstWord)));

            return builder.ToString().Trim();
        }
        /// <summary>
        /// 获取随机句子
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <returns></returns>
        public static string GetSentence(int minWords = 5, int maxWords = 25)
        {
            if (minWords < 3)
                throw new ArgumentException("minWords must 3 or more.", "minWords");
            if (maxWords < 3)
                throw new ArgumentException("maxWords must 3 or more.", "maxWords");

            var builder = new StringBuilder();
            builder.Append(UpperCaseFirstCharacter(_words[GetInt(0, _words.Length - 1)]));
            int numberOfWords = GetInt(minWords, maxWords);
            for (int i = 1; i < numberOfWords; i++)
                builder.Append(' ').Append(_words[GetInt(0, _words.Length - 1)]);

            builder.Append('.');
            return builder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string UpperCaseFirstCharacter(string input)
        {
            if (String.IsNullOrEmpty(input))
                return null;

            char[] inputChars = input.ToCharArray();
            for (int i = 0; i < inputChars.Length; ++i)
            {
                if (inputChars[i] != ' ' && inputChars[i] != '\t')
                {
                    inputChars[i] = Char.ToUpper(inputChars[i]);
                    break;
                }
            }

            return new String(inputChars);
        }
        /// <summary>
        /// 获取随机短评 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="minSentences"></param>
        /// <param name="maxSentences"></param>
        /// <param name="minSentenceWords"></param>
        /// <param name="maxSentenceWords"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetParagraphs(int count = 3, int minSentences = 3, int maxSentences = 25, int minSentenceWords = 5, int maxSentenceWords = 25, bool html = false)
        {
            if (count < 1)
                throw new ArgumentException("Count must be 1 or more.", "count");
            if (minSentences < 1)
                throw new ArgumentException("minSentences must be 1 or more.", "minSentences");

            var builder = new StringBuilder();
            if (html)
                builder.Append("<p>");

            builder.Append("Lorem ipsum dolor sit amet. ");
            int sentenceCount = GetInt(minSentences, maxSentences) - 1;

            for (int i = 0; i < sentenceCount; i++)
                builder.Append(GetSentence(minSentenceWords, maxSentenceWords)).Append(" ");

            if (html)
                builder.Append("</p>");

            for (int i = 1; i < count; i++)
            {
                if (html)
                    builder.Append("<p>");
                for (int x = 0; x < sentenceCount; x++)
                    builder.Append(GetSentence(minSentenceWords, maxSentenceWords)).Append(" ");

                if (html)
                    builder.Append("</p>");
                else
                    builder.Append(System.Environment.NewLine).Append(System.Environment.NewLine);
            }

            return builder.ToString();
        }

        private static string[] _words = { "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
         "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
         "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
         "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
         "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
         "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
         "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
         "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
         "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
         "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
         "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
         "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "duis",
         "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
         "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eros", "et",
         "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum", "zzril", "delenit",
         "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "lorem", "ipsum", "dolor", "sit", "amet",
         "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet",
         "dolore", "magna", "aliquam", "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis",
         "nostrud", "exerci", "tation", "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea",
         "commodo", "consequat", "duis", "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate",
         "velit", "esse", "molestie", "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at",
         "vero", "eros", "et", "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum",
         "zzril", "delenit", "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "nam", "liber", "tempor",
         "cum", "soluta", "nobis", "eleifend", "option", "congue", "nihil", "imperdiet", "doming", "id", "quod", "mazim",
         "placerat", "facer", "possim", "assum", "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing",
         "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam",
         "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exerci", "tation",
         "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "duis",
         "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
         "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eos", "et", "accusam",
         "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea",
         "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
         "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
         "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua", "at", "vero", "eos", "et",
         "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no",
         "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
         "amet", "consetetur", "sadipscing", "elitr", "at", "accusam", "aliquyam", "diam", "diam", "dolore", "dolores",
         "duo", "eirmod", "eos", "erat", "et", "nonumy", "sed", "tempor", "et", "et", "invidunt", "justo", "labore",
         "stet", "clita", "ea", "et", "gubergren", "kasd", "magna", "no", "rebum", "sanctus", "sea", "sed", "takimata",
         "ut", "vero", "voluptua", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
         "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
         "labore", "et", "dolore", "magna", "aliquyam", "erat", "consetetur", "sadipscing", "elitr", "sed", "diam",
         "nonumy", "eirmod", "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed",
         "diam", "voluptua", "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea",
         "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum" };
    }
    /// <summary>
    /// 
    /// </summary>
    public static class EnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> items, T defaultValue = default(T))
        {
            if (items == null)
                return defaultValue;

            var list = items.ToList();
            int count = list.Count();
            if (count == 0)
                return defaultValue;

            return list.ElementAt(RandomData.Instance.Next(count));
        }
    }
}
