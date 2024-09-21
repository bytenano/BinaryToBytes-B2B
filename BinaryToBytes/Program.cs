using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryToBytes
{
    internal class Program
    {

        private static string binaryPath = null;
        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string outputPath = Path.Combine(desktopPath, "output.txt");

        private static int rawType;
        static void Main(string[] args)
        {
            Console.Title = "B2B Converter";
            if (args.Length > 0)
            {
                binaryPath = args[0];
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
        ____ ___   ____     ______                           __           
       / __ )__ \ / __ )   / ____/___  ____ _   _____  _____/ /____  _____
      / __  |_/ // __  |  / /   / __ \/ __ \ | / / _ \/ ___/ __/ _ \/ ___/
     / /_/ / __// /_/ /  / /___/ /_/ / / / / |/ /  __/ /  / /_/  __/ /    
    /_____/____/_____/   \____/\____/_/ /_/|___/\___/_/   \__/\___/_/     ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.WriteLine("    Coded: Nano [ github.com/bytenano ] ");

            if (binaryPath == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
    [Invalid path error] Try again by a Drug&Drop file!");
                Console.ForegroundColor = ConsoleColor.White;

                Console.ReadKey();
                return;
            }

            Console.WriteLine("    [*] Path to file: " + binaryPath);
            Console.WriteLine("");
            Thread.Sleep(200);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"

    =================
    [1] - Hexadecimal
    [2] - Numerical
    [3] - Raw
    =================
    
");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(@"    [+] Enter conversion type: ");
            string input = Console.ReadLine();
            Console.WriteLine("");
            Console.Write("    [?] Want to encrypt bytes in AES-256? (y/n): ");
            string answer = Console.ReadLine();
            if (answer?.ToLower() == "y")
            {
                Console.WriteLine("");
                byte[] keygen = GenerateRandomBytes(32);
                byte[] ivgen = GenerateRandomBytes(16);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("    [?] Generated Key: " + BitConverter.ToString(keygen).Replace("-", ""));
                Console.WriteLine("    [?] Generated IV: " + BitConverter.ToString(ivgen).Replace("-", ""));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.Write("    [+] Enter 32 bytes of key (in hexadecimal): ");
                string keyHex = Console.ReadLine();
                byte[] key = GetValidHexInput(keyHex, 32);

                Console.Write("    [+] Enter 16 bytes IV (in hexadecimal): ");
                string ivHex = Console.ReadLine();
                byte[] iv = GetValidHexInput(ivHex, 16);

                if (int.TryParse(input, out rawType))
                {
                    if (rawType == 1)
                    {
                        Hexademical(key, iv);
                    }
                    else if (rawType == 2)
                    {
                        Numerical(key, iv);
                    }
                    else if (rawType == 3)
                    {
                        RawSide(key, iv);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("    [Error] Invalid value entered.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.ReadKey();
                return;
            }
            Console.WriteLine("");

            if (int.TryParse(input, out rawType))
            {
                if (rawType == 1)
                {
                    Hexademical();
                }
                else if (rawType == 2)
                {
                    Numerical();
                }
                else if (rawType == 3)
                {
                    RawSide();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("    [Error] Enter only 1 or 2");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("    [Error] Invalid value entered.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ReadKey();
            return;
        }

        #region EcryptedConverter
        public static void Hexademical(byte[] key, byte[] iv)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);
                byte[] encryptedBytes = Encrypt(fileBytes, key, iv);

                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (byte b in encryptedBytes)
                    {
                        writer.Write($"0x{b:X2}, ");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("");
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public static void Numerical(byte[] key, byte[] iv)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);
                byte[] encryptedBytes = Encrypt(fileBytes, key, iv);

                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    for (int i = 0; i < encryptedBytes.Length; i++)
                    {
                        writer.WriteLine($"{encryptedBytes[i]},");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("");
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public static void RawSide(byte[] key, byte[] iv)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);
                byte[] encryptedBytes = Encrypt(fileBytes, key, iv);
                StringBuilder hexString = new StringBuilder();

                for (int i = 0; i < encryptedBytes.Length; i++)
                {
                    hexString.Append(encryptedBytes[i].ToString("X2"));
                    if (i < encryptedBytes.Length - 1)
                    {
                        hexString.Append("-");
                    }
                }
                File.WriteAllText(outputPath, hexString.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("");
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
#endregion

        #region DefaultConverter
        public static void Hexademical()
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);

                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (byte b in fileBytes)
                    {
                        writer.Write($"0x{b:X2}, ");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public static void Numerical()
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);

                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        writer.WriteLine($"{fileBytes[i]},");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                return;
            }
        }
        
        public static void RawSide()
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(binaryPath);
                StringBuilder hexString = new StringBuilder();

                for (int i = 0; i < fileBytes.Length; i++)
                {
                    hexString.Append(fileBytes[i].ToString("X2"));
                    if (i < fileBytes.Length - 1)
                    {
                        hexString.Append("-");
                    }
                }
                File.WriteAllText(outputPath, hexString.ToString());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    [!] Data successfully written to file: {outputPath}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        #endregion

        #region Encryption
        private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
        }

        public static byte[] GetValidHexInput(string hex, int expectedLength)
        {
            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentException("Input cannot be null or empty.");
            }

            if (hex.Length != expectedLength * 2)
            {
                throw new ArgumentException($"Hex string must be exactly {expectedLength * 2} characters long.");
            }

            return ConvertHexStringToByteArray(hex);
        }

        public static byte[] ConvertHexStringToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                return randomBytes;
            }
        }
        #endregion
    }
}
