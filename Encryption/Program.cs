using System;
using System.Collections.Generic;
using System.IO;

namespace Encryption
{
    internal static class Helper
    {
        private const int BlockLength = 8;

        private static int[,] routes =
        {
            { 4, 0, 2, 3, 1, 5, 7, 6},
            { 4, 6, 2, 0, 1, 5, 7, 3},
            { 0, 7, 1, 6, 2, 5, 3, 4},
            { 0, 2, 4, 6, 1, 3, 5, 7},
            { 0, 1, 2, 3, 4, 5, 6, 7}, // consistently
            { 7, 6, 5, 4, 3, 2, 1, 0}, // reverse
        };

        public static List<string> ReadFile(string path)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(path))
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    while (!stream.EndOfStream)
                    {
                        list.Add(stream.ReadLine());
                    }
                }
            }
            return list;
        }

        public static void WriteFile(string path, List<string> file)
        {
            if (!string.IsNullOrEmpty(path) && file.Count > 0)
            {
                using (StreamWriter stream = new StreamWriter(path))
                {
                    foreach (var row in file)
                    {
                        stream.WriteLine(row);
                    }
                }
            }
        }

        public static void PrintFile(List<string> file)
        {
            foreach (var row in file)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
        }

        private static List<string> GetSubstrings(string row, int blockLength)
        {
            var bloks = new List<string>();
            for (int i = 0; i < row.Length; i += blockLength)
            {
                var block = string.Empty;
                var remainder = row.Length - i;
                if (remainder > blockLength)
                {
                    block = row.Substring(i, blockLength);
                }
                else
                {
                    block = row.Substring(i);
                    for (int k = 0; k < blockLength - remainder; k++)
                    {
                        block += " ";
                    }
                }
                bloks.Add(block);
            }
            return bloks;
        }

        #region Encrypt
        private static string EncryptBlock(string sourceBlock, int routeNumber)
        {
            var encryptedBlock = string.Empty;
            for (int i = 0; i < sourceBlock.Length; i++)
            {
                encryptedBlock += sourceBlock[routes[routeNumber, i]];
            }
            return encryptedBlock;
        }

        private static string EncryptRow(string sourceRow, int blockLength, int[] keys)
        {
            var encryptedRow = string.Empty;
            var blocks = GetSubstrings(sourceRow, blockLength);
            for (int i = 0; i < blocks.Count; i++)
            {
                encryptedRow += EncryptBlock(blocks[i], keys[i % keys.Length]);
            }
            return encryptedRow;
        }

        public static List<string> EncryptFile(List<string> sourceFile, int[] keys)
        {
            var encryptFile = new List<string>();
            foreach (var sourceRow in sourceFile)
            {
                encryptFile.Add(EncryptRow(sourceRow, BlockLength, keys));
            }
            return encryptFile;
        }
        #endregion

        #region Decrypt
        private static string DecryptBlock(string encryptedBlock, int routeNumber)
        {
            var decryptedBlock = new char[encryptedBlock.Length];
            for (int i = 0; i < encryptedBlock.Length; i++)
            {
                decryptedBlock[routes[routeNumber, i]] = encryptedBlock[i];
            }
            return new string(decryptedBlock);
        }

        private static string DecryptRow(string encryptedRow, int blockLength, int[] keys)
        {
            var decryptedRow = string.Empty;
            var blocks = GetSubstrings(encryptedRow, blockLength);
            for (int i = 0; i < blocks.Count; i++)
            {
                decryptedRow += DecryptBlock(blocks[i], keys[i % keys.Length]);
            }
            return decryptedRow;
        }

        public static List<string> DecryptFile(List<string> encryptFile, int[] keys)
        {
            var decryptFile = new List<string>();
            foreach (var encryptRow in encryptFile)
            {
                decryptFile.Add(DecryptRow(encryptRow, BlockLength, keys));
            }
            return decryptFile;
        }
        #endregion
    }

    internal class Program
    {
        private const string InputPath = "input.txt";
        private const string EncryptPath = "encrypt.txt";
        private const string DecryptPath = "decrypt.txt";
        private static int[] keys = { 1, 3 };
        static void Main(string[] args)
        {
            var sourceFile = Helper.ReadFile(InputPath);
            Console.WriteLine("Source text:");
            Helper.PrintFile(sourceFile);

            var encryptFile = Helper.EncryptFile(sourceFile, keys);
            Helper.WriteFile(EncryptPath, encryptFile);
            Console.WriteLine("Encrypt text:");
            Helper.PrintFile(encryptFile);

            var decryptFile = Helper.DecryptFile(encryptFile, keys);
            Helper.WriteFile(DecryptPath, decryptFile);
            Console.WriteLine("Decrypt text:");
            Helper.PrintFile(decryptFile);

            Console.ReadLine();
        }
    }
}
