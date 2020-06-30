using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace KeMai
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileEntity
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 文件大小 
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FileUtils
    {

        public const int BUFFER_SIZE = 4096;
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <param name="serverUtility"></param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                return strPath;
            }
            else
            {
                if (strPath.IndexOf("/") != 0)
                {
                    if (Path.IsPathRooted(strPath))
                    {
                        return strPath;
                    }
                }
            }
            try
            {
                if (HttpContext.Current != null)
                {
                    strPath = strPath.Replace("\\", "/");
                    string strIndex = strPath.Substring(0, 1);
                    if (strIndex != "/" && strIndex != "~")
                    {
                        strPath = "~/" + strPath;
                    }
                    return HostingEnvironment.MapPath(strPath);
                    //return HttpContext.Current.Server.MapPath(strPath);
                }
                else //非web程序引用
                {
                    return GetAppPath(strPath);
                }
            }
            catch (Exception)
            {
                return GetAppPath(strPath);
            }

        }
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <param name="serverUtility"></param>
        /// <returns>绝对路径</returns>
        public static string GetAppPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return string.Empty;
            strPath = strPath.Replace("~/", "\\").Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originFileName"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        public static string Format(string originFileName, string pathFormat)
        {
            if (String.IsNullOrWhiteSpace(pathFormat))
            {
                pathFormat = "{filename}{rand:6}";
            }

            var invalidPattern = new Regex(@"[\\\/\:\*\?\042\<\>\|]");
            originFileName = invalidPattern.Replace(originFileName, "");

            string extension = Path.GetExtension(originFileName);
            string filename = Path.GetFileNameWithoutExtension(originFileName);

            pathFormat = pathFormat.Replace("{filename}", filename);
            pathFormat = new Regex(@"\{rand(\:?)(\d+)\}", RegexOptions.Compiled).Replace(pathFormat, new MatchEvaluator(delegate (Match match)
            {
                var digit = 6;
                if (match.Groups.Count > 2)
                {
                    digit = Convert.ToInt32(match.Groups[2].Value);
                }
                var rand = new Random();
                return rand.Next((int)Math.Pow(10, digit), (int)Math.Pow(10, digit + 1)).ToString();
            }));

            pathFormat = pathFormat.Replace("{time}", DateTime.Now.Ticks.ToString());
            pathFormat = pathFormat.Replace("{yyyy}", DateTime.Now.Year.ToString());
            pathFormat = pathFormat.Replace("{yy}", (DateTime.Now.Year % 100).ToString("D2"));
            pathFormat = pathFormat.Replace("{mm}", DateTime.Now.Month.ToString("D2"));
            pathFormat = pathFormat.Replace("{dd}", DateTime.Now.Day.ToString("D2"));
            pathFormat = pathFormat.Replace("{hh}", DateTime.Now.Hour.ToString("D2"));
            pathFormat = pathFormat.Replace("{ii}", DateTime.Now.Minute.ToString("D2"));
            pathFormat = pathFormat.Replace("{ss}", DateTime.Now.Second.ToString("D2"));

            return pathFormat + extension;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileExtName"></param>
        /// <returns></returns>
        private static string GetContentType(string fileExtName)
        {
            switch (fileExtName)
            {
                #region 常用文件类型
                case "jpeg": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "js": return "application/x-javascript";
                case "jsp": return "text/html";
                case "gif": return "image/gif";
                case "htm": return "text/html";
                case "html": return "text/html";
                case "asf": return "video/x-ms-asf";
                case "avi": return "video/avi";
                case "bmp": return "application/x-bmp";
                case "asp": return "text/asp";
                case "wma": return "audio/x-ms-wma";
                case "wav": return "audio/wav";
                case "wmv": return "video/x-ms-wmv";
                case "ra": return "audio/vnd.rn-realaudio";
                case "ram": return "audio/x-pn-realaudio";
                case "rm": return "application/vnd.rn-realmedia";
                case "rmvb": return "application/vnd.rn-realmedia-vbr";
                case "xhtml": return "text/html";
                case "png": return "image/png";
                case "ppt": return "application/x-ppt";
                case "tif": return "image/tiff";
                case "tiff": return "image/tiff";
                case "xls": return "application/x-xls";
                case "xlw": return "application/x-xlw";
                case "xml": return "text/xml";
                case "xpl": return "audio/scpls";
                case "swf": return "application/x-shockwave-flash";
                case "torrent": return "application/x-bittorrent";
                case "dll": return "application/x-msdownload";
                case "asa": return "text/asa";
                case "asx": return "video/x-ms-asf";
                case "au": return "audio/basic";
                case "css": return "text/css";
                case "doc": return "application/msword";
                case "exe": return "application/x-msdownload";
                case "mp1": return "audio/mp1";
                case "mp2": return "audio/mp2";
                case "mp2v": return "video/mpeg";
                case "mp3": return "audio/mp3";
                case "mp4": return "video/mpeg4";
                case "mpa": return "video/x-mpg";
                case "mpd": return "application/vnd.ms-project";
                case "mpe": return "video/x-mpeg";
                case "mpeg": return "video/mpg";
                case "mpg": return "video/mpg";
                case "mpga": return "audio/rn-mpeg";
                case "mpp": return "application/vnd.ms-project";
                case "mps": return "video/x-mpeg";
                case "mpt": return "application/vnd.ms-project";
                case "mpv": return "video/mpg";
                case "mpv2": return "video/mpeg";
                case "wml": return "text/vnd.wap.wml";
                case "wsdl": return "text/xml";
                case "xsd": return "text/xml";
                case "xsl": return "text/xml";
                case "xslt": return "text/xml";
                case "htc": return "text/x-component";
                case "mdb": return "application/msaccess";
                case "zip": return "application/zip";
                case "rar": return "application/x-rar-compressed";

                case "*": return "application/octet-stream";
                case "001": return "application/x-001";
                case "301": return "application/x-301";
                case "323": return "text/h323";
                case "906": return "application/x-906";
                case "907": return "drawing/907";
                case "a11": return "application/x-a11";
                case "acp": return "audio/x-mei-aac";
                case "ai": return "application/postscript";
                case "aif": return "audio/aiff";
                case "aifc": return "audio/aiff";
                case "aiff": return "audio/aiff";
                case "anv": return "application/x-anv";
                case "awf": return "application/vnd.adobe.workflow";
                case "biz": return "text/xml";
                case "bot": return "application/x-bot";
                case "c4t": return "application/x-c4t";
                case "c90": return "application/x-c90";
                case "cal": return "application/x-cals";
                case "cat": return "application/vnd.ms-pki.seccat";
                case "cdf": return "application/x-netcdf";
                case "cdr": return "application/x-cdr";
                case "cel": return "application/x-cel";
                case "cer": return "application/x-x509-ca-cert";
                case "cg4": return "application/x-g4";
                case "cgm": return "application/x-cgm";
                case "cit": return "application/x-cit";
                case "class": return "java/*";
                case "cml": return "text/xml";
                case "cmp": return "application/x-cmp";
                case "cmx": return "application/x-cmx";
                case "cot": return "application/x-cot";
                case "crl": return "application/pkix-crl";
                case "crt": return "application/x-x509-ca-cert";
                case "csi": return "application/x-csi";
                case "cut": return "application/x-cut";
                case "dbf": return "application/x-dbf";
                case "dbm": return "application/x-dbm";
                case "dbx": return "application/x-dbx";
                case "dcd": return "text/xml";
                case "dcx": return "application/x-dcx";
                case "der": return "application/x-x509-ca-cert";
                case "dgn": return "application/x-dgn";
                case "dib": return "application/x-dib";
                case "dot": return "application/msword";
                case "drw": return "application/x-drw";
                case "dtd": return "text/xml";
                case "dwf": return "application/x-dwf";
                case "dwg": return "application/x-dwg";
                case "dxb": return "application/x-dxb";
                case "dxf": return "application/x-dxf";
                case "edn": return "application/vnd.adobe.edn";
                case "emf": return "application/x-emf";
                case "eml": return "message/rfc822";
                case "ent": return "text/xml";
                case "epi": return "application/x-epi";
                case "eps": return "application/x-ps";
                case "etd": return "application/x-ebx";
                case "fax": return "image/fax";
                case "fdf": return "application/vnd.fdf";
                case "fif": return "application/fractals";
                case "fo": return "text/xml";
                case "frm": return "application/x-frm";
                case "g4": return "application/x-g4";
                case "gbr": return "application/x-gbr";
                case "gcd": return "application/x-gcd";

                case "gl2": return "application/x-gl2";
                case "gp4": return "application/x-gp4";
                case "hgl": return "application/x-hgl";
                case "hmr": return "application/x-hmr";
                case "hpg": return "application/x-hpgl";
                case "hpl": return "application/x-hpl";
                case "hqx": return "application/mac-binhex40";
                case "hrf": return "application/x-hrf";
                case "hta": return "application/hta";
                case "htt": return "text/webviewhtml";
                case "htx": return "text/html";
                case "icb": return "application/x-icb";
                case "ico": return "application/x-ico";
                case "iff": return "application/x-iff";
                case "ig4": return "application/x-g4";
                case "igs": return "application/x-igs";
                case "iii": return "application/x-iphone";
                case "img": return "application/x-img";
                case "ins": return "application/x-internet-signup";
                case "isp": return "application/x-internet-signup";
                case "IVF": return "video/x-ivf";
                case "java": return "java/*";
                case "jfif": return "image/jpeg";
                case "jpe": return "application/x-jpe";
                case "la1": return "audio/x-liquid-file";
                case "lar": return "application/x-laplayer-reg";
                case "latex": return "application/x-latex";
                case "lavs": return "audio/x-liquid-secure";
                case "lbm": return "application/x-lbm";
                case "lmsff": return "audio/x-la-lms";
                case "ls": return "application/x-javascript";
                case "ltr": return "application/x-ltr";
                case "m1v": return "video/x-mpeg";
                case "m2v": return "video/x-mpeg";
                case "m3u": return "audio/mpegurl";
                case "m4e": return "video/mpeg4";
                case "mac": return "application/x-mac";
                case "man": return "application/x-troff-man";
                case "math": return "text/xml";
                case "mfp": return "application/x-shockwave-flash";
                case "mht": return "message/rfc822";
                case "mhtml": return "message/rfc822";
                case "mi": return "application/x-mi";
                case "mid": return "audio/mid";
                case "midi": return "audio/mid";
                case "mil": return "application/x-mil";
                case "mml": return "text/xml";
                case "mnd": return "audio/x-musicnet-download";
                case "mns": return "audio/x-musicnet-stream";
                case "mocha": return "application/x-javascript";
                case "movie": return "video/x-sgi-movie";
                case "mpw": return "application/vnd.ms-project";
                case "mpx": return "application/vnd.ms-project";
                case "mtx": return "text/xml";
                case "mxp": return "application/x-mmxp";
                case "net": return "image/pnetvue";
                case "nrf": return "application/x-nrf";
                case "nws": return "message/rfc822";
                case "odc": return "text/x-ms-odc";
                case "out": return "application/x-out";
                case "p10": return "application/pkcs10";
                case "p12": return "application/x-pkcs12";
                case "p7b": return "application/x-pkcs7-certificates";
                case "p7c": return "application/pkcs7-mime";
                case "p7m": return "application/pkcs7-mime";
                case "p7r": return "application/x-pkcs7-certreqresp";
                case "p7s": return "application/pkcs7-signature";
                case "pc5": return "application/x-pc5";
                case "pci": return "application/x-pci";
                case "pcl": return "application/x-pcl";
                case "pcx": return "application/x-pcx";
                case "pdf": return "application/pdf";
                case "pdx": return "application/vnd.adobe.pdx";
                case "pfx": return "application/x-pkcs12";
                case "pgl": return "application/x-pgl";
                case "pic": return "application/x-pic";
                case "pko": return "application/vnd.ms-pki.pko";
                case "pl": return "application/x-perl";
                case "plg": return "text/html";
                case "pls": return "audio/scpls";
                case "plt": return "application/x-plt";
                case "pot": return "application/vnd.ms-powerpoint";
                case "ppa": return "application/vnd.ms-powerpoint";
                case "ppm": return "application/x-ppm";
                case "pps": return "application/vnd.ms-powerpoint";
                case "pr": return "application/x-pr";
                case "prf": return "application/pics-rules";
                case "prn": return "application/x-prn";
                case "prt": return "application/x-prt";
                case "ps": return "application/x-ps";
                case "ptn": return "application/x-ptn";
                case "pwz": return "application/vnd.ms-powerpoint";
                case "r3t": return "text/vnd.rn-realtext3d";
                case "ras": return "application/x-ras";
                case "rat": return "application/rat-file";
                case "rdf": return "text/xml";
                case "rec": return "application/vnd.rn-recording";
                case "red": return "application/x-red";
                case "rgb": return "application/x-rgb";
                case "rjs": return "application/vnd.rn-realsystem-rjs";
                case "rjt": return "application/vnd.rn-realsystem-rjt";
                case "rlc": return "application/x-rlc";
                case "rle": return "application/x-rle";
                case "rmf": return "application/vnd.adobe.rmf";
                case "rmi": return "audio/mid";
                case "rmj": return "application/vnd.rn-realsystem-rmj";
                case "rmm": return "audio/x-pn-realaudio";
                case "rmp": return "application/vnd.rn-rn_music_package";
                case "rms": return "application/vnd.rn-realmedia-secure";
                case "rmx": return "application/vnd.rn-realsystem-rmx";
                case "rnx": return "application/vnd.rn-realplayer";
                case "rp": return "image/vnd.rn-realpix";
                case "rpm": return "audio/x-pn-realaudio-plugin";
                case "rsml": return "application/vnd.rn-rsml";
                case "rt": return "text/vnd.rn-realtext";
                case "rtf": return "application/msword";
                case "rv": return "video/vnd.rn-realvideo";
                case "sam": return "application/x-sam";
                case "sat": return "application/x-sat";
                case "sdp": return "application/sdp";
                case "sdw": return "application/x-sdw";
                case "sit": return "application/x-stuffit";
                case "slb": return "application/x-slb";
                case "sld": return "application/x-sld";
                case "slk": return "drawing/x-slk";
                case "smi": return "application/smil";
                case "smil": return "application/smil";
                case "smk": return "application/x-smk";
                case "snd": return "audio/basic";
                case "sol": return "text/plain";
                case "sor": return "text/plain";
                case "spc": return "application/x-pkcs7-certificates";
                case "spl": return "application/futuresplash";
                case "spp": return "text/xml";
                case "ssm": return "application/streamingmedia";
                case "sst": return "application/vnd.ms-pki.certstore";
                case "stl": return "application/vnd.ms-pki.stl";
                case "stm": return "text/html";
                case "sty": return "application/x-sty";
                case "svg": return "text/xml";
                case "tdf": return "application/x-tdf";
                case "tg4": return "application/x-tg4";
                case "tga": return "application/x-tga";
                case "tld": return "text/xml";
                case "top": return "drawing/x-top";
                case "tsd": return "text/xml";
                case "txt": return "text/plain";
                case "uin": return "application/x-icq";
                case "uls": return "text/iuls";
                case "vcf": return "text/x-vcard";
                case "vda": return "application/x-vda";
                case "vdx": return "application/vnd.visio";
                case "vml": return "text/xml";
                case "vpg": return "application/x-vpeg005";
                case "vsd": return "application/vnd.visio";
                case "vss": return "application/vnd.visio";
                case "vst": return "application/vnd.visio";
                case "vsw": return "application/vnd.visio";
                case "vsx": return "application/vnd.visio";
                case "vtx": return "application/vnd.visio";
                case "vxml": return "text/xml";
                case "wax": return "audio/x-ms-wax";
                case "wb1": return "application/x-wb1";
                case "wb2": return "application/x-wb2";
                case "wb3": return "application/x-wb3";
                case "wbmp": return "image/vnd.wap.wbmp";
                case "wiz": return "application/msword";
                case "wk3": return "application/x-wk3";
                case "wk4": return "application/x-wk4";
                case "wkq": return "application/x-wkq";
                case "wks": return "application/x-wks";
                case "wm": return "video/x-ms-wm";
                case "wmd": return "application/x-ms-wmd";
                case "wmf": return "application/x-wmf";
                case "wmx": return "video/x-ms-wmx";
                case "wmz": return "application/x-ms-wmz";
                case "wp6": return "application/x-wp6";
                case "wpd": return "application/x-wpd";
                case "wpg": return "application/x-wpg";
                case "wpl": return "application/vnd.ms-wpl";
                case "wq1": return "application/x-wq1";
                case "wr1": return "application/x-wr1";
                case "wri": return "application/x-wri";
                case "wrk": return "application/x-wrk";
                case "ws": return "application/x-ws";
                case "ws2": return "application/x-ws";
                case "wsc": return "text/scriptlet";
                case "wvx": return "video/x-ms-wvx";
                case "xdp": return "application/vnd.adobe.xdp";
                case "xdr": return "text/xml";
                case "xfd": return "application/vnd.adobe.xfd";
                case "xfdf": return "application/vnd.adobe.xfdf";
                case "xq": return "text/xml";
                case "xql": return "text/xml";
                case "xquery": return "text/xml";
                case "xwd": return "application/x-xwd";
                case "x_b": return "application/x-x_b";
                case "x_t": return "application/x-x_t";
                    #endregion
            }
            return "application/octet-stream";
        }
        /// <summary>
        /// 获得文件存放目录
        /// </summary>
        /// <returns></returns>
        private static string GetSavePathByDate()
        {
            StringBuilder saveDir = new StringBuilder("");
            saveDir.Append(DateTime.Now.ToString("yyyy"));
            saveDir.Append(Path.DirectorySeparatorChar);
            saveDir.Append(DateTime.Now.ToString("MM"));
            saveDir.Append(Path.DirectorySeparatorChar);
            saveDir.Append(DateTime.Now.ToString("dd"));
            saveDir.Append(Path.DirectorySeparatorChar);
            return saveDir.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public void BigFileRead(string strFilePath)
        {
            //每次读取的字节数
            int iBufferSize = 1024000;
            byte[] buffer = new byte[iBufferSize];
            FileStream fs = null;
            try
            {
                fs = new FileStream(strFilePath, FileMode.Open);
                //文件流的长度
                long lFileSize = fs.Length;
                //文件需要读取次数
                int iTotalCount = (int)Math.Ceiling((double)(lFileSize / iBufferSize));
                //当前读取次数
                int iTempCount = 0;

                while (iTempCount < iTotalCount)
                {
                    //每次从最后读到的位置读取下一个[iBufferSize]的字节数
                    fs.Read(buffer, 0, iBufferSize);
                    //将字节转换成字符
                    string strRead = Encoding.Default.GetString(buffer);
                    //此处加入你的处理逻辑
                    Console.Write(strRead);
                }
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
                throw ex;
                //异常处理
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static FileEntity CreateFileEntity(HttpPostedFileBase postedFile, string uploadDir = "Uploads", string strDir = "")
        {
            //Random random = new Random(unchecked((int)DateTime.Now.Ticks));
            string fileName = Path.GetFileName(postedFile.FileName);
            string fileExtName = Path.GetExtension(postedFile.FileName);
            if (!string.IsNullOrEmpty(fileExtName) && fileExtName.Length > 1)
            {
                fileExtName = fileExtName.Substring(1);
            }
            string fileType = postedFile.ContentType.ToLower();
            int fileSize = postedFile.ContentLength;
            //string newFileName = string.Format("{0}{1}.{2}",
            //                  (Environment.TickCount & int.MaxValue).ToString(),
            //                  random.Next(1000, 9999).ToString(),
            //                  fileExtName);
            string newFileName = string.Format("{0}.{1}",
                            Guid.NewGuid().ToString("n"),
                            fileExtName);
            if (fileType == "application/octet-stream")
            {
                fileType = GetContentType(fileExtName);
            }

            string saveDir = GetSavePathByDate();
            if (!string.IsNullOrEmpty(strDir))
            {
                saveDir = strDir + "/" + saveDir;
            }
            FileEntity entity = new FileEntity();
            entity.Name = newFileName;
            entity.ExtName = fileExtName;
            entity.Path = saveDir.Replace(Path.DirectorySeparatorChar, '/') + newFileName;
            entity.Size = fileSize;
            entity.Type = fileType;
            entity.SourceName = fileName;
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static HttpPostedFileBase ToHttpPostedFileBase(HttpPostedFile postedFile)
        {
            return new HttpPostedFileWrapper(postedFile) as HttpPostedFileBase;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static FileEntity CreateFileEntity(HttpPostedFile postedFile, string uploadDir = "Uploads", string strDir = "")
        {
            return CreateFileEntity(ToHttpPostedFileBase(postedFile), uploadDir, strDir);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="uploadDir"></param>
        /// <param name="strDir"></param>
        /// <param name="logger"></param>
        /// <param name="serverUtility"></param>
        /// <returns></returns>
        public static FileEntity UploadFile(HttpPostedFile postedFile, string uploadDir = "Uploads", string strDir = "")
        {
            return UploadFile(ToHttpPostedFileBase(postedFile), uploadDir, strDir);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="uploadDir"></param>
        /// <param name="strDir"></param>
        /// <param name="logger"></param>
        /// <param name="serverUtility"></param>
        /// <returns></returns>
        public static FileEntity UploadFile(HttpPostedFileBase postedFile, string uploadDir = "Uploads", string strDir = "")
        {
            try
            {
                string uploadPath = uploadDir;
                if (string.IsNullOrEmpty(uploadPath))
                {
                    uploadPath = "Uploads";
                }
                FileEntity entity = CreateFileEntity(postedFile, uploadDir, strDir);
                var saveDir = entity.Path.Substring(0, entity.Path.LastIndexOf('/'));
                uploadPath = GetMapPath(uploadDir);
                uploadPath = uploadPath.TrimEnd('\\') + "\\" + saveDir;
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                postedFile.SaveAs(uploadPath + Path.DirectorySeparatorChar + entity.Name);
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#if NET45
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="uploadDir"></param>
        /// <param name="strDir"></param>
        /// <param name="logger"></param>
        /// <param name="serverUtility"></param>
        /// <returns></returns>
        public static async Task<FileEntity> UploadFileAsync(HttpPostedFile postedFile, string uploadDir, string strDir = "")
        {
            return await UploadFileAsync(ToHttpPostedFileBase(postedFile), uploadDir, strDir);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="uploadDir"></param>
        /// <param name="strDir"></param>
        /// <param name="logger"></param>
        /// <param name="serverUtility"></param>
        /// <returns></returns>
        public static async Task<FileEntity> UploadFileAsync(HttpPostedFileBase postedFile, string uploadDir, string strDir = "")
        {
            try
            {
                FileEntity entity = CreateFileEntity(postedFile, uploadDir, strDir);
                string uploadPath = uploadDir;
                if (string.IsNullOrEmpty(uploadPath))
                {
                    uploadPath = "Uploads";
                }
                var saveDir = entity.Path.Substring(0, entity.Path.LastIndexOf('/'));
                uploadPath = GetMapPath(uploadDir);
                uploadPath = uploadPath.TrimEnd('\\') + "\\" + saveDir;
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                string filePath = uploadPath + Path.DirectorySeparatorChar + entity.Name;
                using (var fs = new FileStream(
                             path: filePath,
                             mode: FileMode.Create,
                             access: FileAccess.Write,
                             share: FileShare.None,
                             bufferSize: BUFFER_SIZE,
                             useAsync: true))
                {
                    var buffer = new byte[BUFFER_SIZE];
                    int numRead;
                    while ((numRead = await postedFile.InputStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await fs.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveFilePath"></param>
        private void UploadFileByRange(HttpPostedFile file, string saveFilePath)
        {
            UploadFileByRange(ToHttpPostedFileBase(file), saveFilePath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveFilePath"></param>
        private void UploadFileByRange(HttpPostedFileBase file, string saveFilePath)
        {
            long lStartPos = 0;
            int startPosition = 0;
            int endPosition = 0;
            var contentRange = HttpContext.Current.Request.Headers["Content-Range"];
            //bytes 10000-19999/1157632
            if (!string.IsNullOrEmpty(contentRange))
            {
                contentRange = contentRange.Replace("bytes", "").Trim();
                contentRange = contentRange.Substring(0, contentRange.IndexOf("/"));
                string[] ranges = contentRange.Split('-');
                startPosition = int.Parse(ranges[0]);
                endPosition = int.Parse(ranges[1]);
            }
            System.IO.FileStream fs;
            if (System.IO.File.Exists(saveFilePath))
            {
                fs = System.IO.File.OpenWrite(saveFilePath);
                lStartPos = fs.Length;

            }
            else
            {
                fs = new System.IO.FileStream(saveFilePath, System.IO.FileMode.Create);
                lStartPos = 0;
            }
            if (lStartPos > endPosition)
            {
                fs.Close();
                return;
            }
            else if (lStartPos < startPosition)
            {
                lStartPos = startPosition;
            }
            else if (lStartPos > startPosition && lStartPos < endPosition)
            {
                lStartPos = startPosition;
            }
            fs.Seek(lStartPos, System.IO.SeekOrigin.Current);
            int bufferLen = 512;
            byte[] nbytes = new byte[bufferLen];
            int nReadSize = 0;
            nReadSize = file.InputStream.Read(nbytes, 0, bufferLen);
            while (nReadSize > 0)
            {
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = file.InputStream.Read(nbytes, 0, bufferLen);
            }
            fs.Close();
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileInfo fi = new FileInfo(filePath);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(filePath);//直接删除其中的文件   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="filePath"></param>
        ///  <param name="logger"></param>
        public static Task DeleteFileAsync(string filePath)
        {
            return Task.Factory.StartNew(() =>
            {
                DeleteFile(filePath);
            });
        }

        /// <summary>
        /// Copy文件
        /// </summary>
        /// <param name="sourceFileName">源文件名:C:\Users\新建文本文档.txt </param>
        /// <param name="destFileName">目标文件名:C:\Users</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!File.Exists(sourceFileName))
            {
                throw new FileNotFoundException(sourceFileName + "文件不存在！");
            }
            if (sourceFileName != null)
            {
                if (!Directory.Exists(destFileName))
                {
                    Directory.CreateDirectory(destFileName);
                }
                //将sourceFileName复制到destFileName目录下，文件名相同。
                File.Copy(sourceFileName, Path.Combine(destFileName, Path.GetFileName(sourceFileName)), overwrite);
            }
            return true;
        }

        /// <summary>          
        /// Copy文件夹          
        /// </summary>          
        /// <param name="sPath">源文件夹路径</param>          
        /// <param name="dPath">目的文件夹路径</param>     
        /// <param name="logger"></param>
        /// <returns></returns>          
        public static void CopyFolder(string sPath, string dPath)
        {
            try
            {
                //创建目的文件夹                  
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }
                // 拷贝文件                  
                DirectoryInfo sDir = new DirectoryInfo(sPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(dPath + "\\" + file.Name, true);
                }
                // 循环子文件夹                  
                //DirectoryInfo dDir = new DirectoryInfo(dPath);
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, dPath + "//" + subDir.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = "复制文件夹错误：源文件夹路径:" + sPath + ",目的文件夹路径:" + dPath + "\n" + ex.Message;
                ex = new Exception(msg);
                throw ex;
            }

        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件   
                }
                else
                    DeleteFolder(d);//递归删除子文件夹   
            }
            Directory.Delete(dir);//删除已空文件夹   
        }


        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDir(string path)
        {
            if (path.Trim() == "" || !Directory.Exists(path)) return;
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            FileInfo[] fileInfos = dirInfo.GetFiles();
            if (fileInfos.Length > 0)
            {
                foreach (FileInfo fileInfo in fileInfos)
                {
                    //DateTime.Compare( fileInfo.LastWriteTime,DateTime.Now);
                    System.IO.File.Delete(fileInfo.FullName); //删除文件
                }
            }
        }

        #region 验证图片文件
        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            fileName = fileName.Trim();
            if (fileName.EndsWith(".") || fileName.IndexOf(".") == -1)
                return false;
            string extName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();
            List<string> tmpExtList = new List<string> { "jpg", "jpeg", "png", "bmp", "gif" };
            return tmpExtList.Exists(a => a == extName);
        }
        /// <summary>
        /// 根据文件头判断上传的文件类型
        /// </summary>
        /// <param name="stream">文件类型</param>
        /// <param name="logger"></param>
        /// <returns>返回true或false</returns>
        public static bool CheckImageByStream(Stream stream)
        {
            bool isImage = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);//严重更正过来
                isImage = true;
                image.Dispose();
                image = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isImage;
        }
        /// <summary>
        /// 根据二进制获取文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool IsImgFileByBytes(byte[] buffer)
        {

            /*文件扩展名说明
             * 208207 doc xls ppt wps
             * 8075 docx pptx xlsx zip
             * 5150 txt
             * 8297 rar
             * 7790 exe
             * 3780 pdf      
             * 
             * 4946/104116 txt
             * 
             * 7173        gif 
             * 255216      jpg
             * 13780       png
             * 6677        bmp
             * 239187      txt,aspx,asp,sql
             * 208207      xls.doc.ppt
             * 6063        xml
             * 6033        htm,html
             * 4742        js
             * 8075        xlsx,zip,pptx,mmap,zip
             * 8297        rar   
             * 01          accdb,mdb
             * 7790        exe,dll
             * 5666        psd 
             * 255254      rdp 
             * 10056       bt种子 
             * 64101       bat 
             * 4059        sgf    
             */
            if (buffer == null || buffer.Length < 2) return false;
            string extByte = buffer[0].ToString() + buffer[1];
            List<string> tmpExtList = new List<string> { "7173", "255216", "13780", "6677" };
            return tmpExtList.Exists(a => a == extByte);

        }
        /// <summary>
        /// 根据二进制获取文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool IsImgFileByMime(byte[] buffer)
        {
            string mime = GetMimeFromByBytes(buffer);
            List<string> tmpMimeList = new List<string> { "image/pjpeg", "image/jpeg", "image/jpeg2000", "image/png", "image/x-png", "image/bmp", "image/gif" };
            return tmpMimeList.Exists(a => a == mime);
        }
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static string GetMimeFromByBytes(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return string.Empty;
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
