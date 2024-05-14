﻿using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Masuit.Tools.Files
{
    /// <summary>
    /// 大文件操作扩展类
    /// </summary>
    public static class FileExt
    {
        /// <summary>
        /// 以文件流的形式复制大文件
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        public static void CopyToFile(this Stream fs, string dest, int bufferSize = 1024 * 8 * 1024)
        {
            fs.Seek(0, SeekOrigin.Begin);
            using var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var stream = new BufferedStream(fs, bufferSize);
            stream.CopyTo(fsWrite);
            fs.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// 以文件流的形式复制大文件(异步方式)
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        public static Task CopyToFileAsync(this Stream fs, string dest, int bufferSize = 1024 * 1024 * 8)
        {
            using var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var stream = new BufferedStream(fs, bufferSize);
            return stream.CopyToAsync(fsWrite);
        }

        /// <summary>
        /// 将内存流转储成文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        public static void SaveFile(this Stream ms, string filename)
        {
            ms.Seek(0, SeekOrigin.Begin);
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            var stream = new BufferedStream(ms, 1048576);
            stream.CopyTo(fs);
            ms.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// 将内存流转储成文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        public static async Task SaveFileAsync(this Stream ms, string filename)
        {
            ms.Seek(0, SeekOrigin.Begin);
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            var stream = new BufferedStream(ms, 1048576);
            await stream.CopyToAsync(fs);
            ms.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// 计算文件的 MD5 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string GetFileMD5(this FileStream fs) => HashFile(fs);

        /// <summary>
        /// 计算文件的 sha1 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>sha1 值16进制字符串</returns>
        public static string GetFileSha1(this Stream fs) => HashFile(fs, nameof(SHA1));

        /// <summary>
        /// 计算文件的 sha256 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>sha256 值16进制字符串</returns>
        public static string GetFileSha256(this Stream fs) => HashFile(fs, nameof(SHA256));

        /// <summary>
        /// 计算文件的 sha512 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>sha512 值16进制字符串</returns>
        public static string GetFileSha512(this Stream fs) => HashFile(fs, nameof(SHA512));

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fs">被操作的源数据流</param>
        /// <param name="algo">加密算法</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(Stream fs, string algo = nameof(MD5))
        {
            fs.Seek(0, SeekOrigin.Begin);
            using HashAlgorithm crypto = algo switch
            {
                nameof(SHA1) => SHA1.Create(),
                nameof(SHA256) => SHA256.Create(),
                nameof(SHA512) => SHA512.Create(),
                _ => MD5.Create(),
            };
            var stream = new BufferedStream(fs, 1048576);
            byte[] hash = crypto.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("x2"));
            }

            fs.Seek(0, SeekOrigin.Begin);
            return sb.ToString();
        }
    }
}
