using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Models.Base;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace DigitalWorldOnline.Commons.Extensions
{
    //TODO: Abstrair para Cryptography
    //TODO: Inserir senha nos Secrets
    public static class Extensions
    {
        public static void RemoveWhere<T>(this LinkedList<T> list, Predicate<T> match)
        {
            var current = list.First;
            while (current != null)
            {
                var next = current.Next;
                if (match(current.Value))
                {
                    list.Remove(current);
                }
                current = next;
            }
        }

        public static string HashPassword(this string password)
        {
            try
            {
                using var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedString = Convert.ToBase64String(hashedBytes);

                return hashedString;
            }
            catch
            {
                return password;
            }
        }

        public static string GetDescription(this Enum value)
        {
            var descriptionAttribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return value.ToString();
        }

        public static List<ItemModel> Clone(this List<ItemModel> list)
        {
            var newList = new List<ItemModel>();
            list.ForEach(item =>
            {
                newList.Add((ItemModel)item.Clone(item.Id));
            });

            return newList;
        }

        public static T PickRandom<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).First();
        }
        
        public static List<T> Randomize<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public static List<T> GetList<T>(this T value)
        {
            return new[] { value }.ToList();
        }

        public static string ConcatString(this string currentString, string value, bool inTheEnd = false)
        {
            if (inTheEnd)
                return currentString + value;
            else
                return value + currentString;
        }

        public static string ModeratorPrefix(this string currentString, AccountAccessLevelEnum accLvEnum)
        {
            return accLvEnum switch
            {
                AccountAccessLevelEnum.Moderator => "[MOD]" + currentString,
                AccountAccessLevelEnum.GameMasterOne or
                AccountAccessLevelEnum.GameMasterTwo or
                AccountAccessLevelEnum.GameMasterThree => "[GM]" + currentString,
                AccountAccessLevelEnum.Administrator => "[ADM]" + currentString,
                _ => currentString,
            };
        }

        private static readonly string Key = "452874123659852365284512";

        public static string Encrypt(this string toEncrypt)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(toEncrypt));
        }

        public static string Base64Decrypt(this string toDecrypt)
        {
            try { return Encoding.ASCII.GetString(Convert.FromBase64String(toDecrypt)); }
            catch { return toDecrypt; }
        }

        /* Blocked for MudBlazor
        public static string AesEncrypt(this string toEncrypt)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new(cryptoStream))
                {
                    streamWriter.Write(toEncrypt);
                }

                array = memoryStream.ToArray();
            }

            return Convert.ToBase64String(array);
        }

        public static string AesDecrypt(this string toDecrypt)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(toDecrypt);

            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            return streamReader.ReadToEnd();
        }
        */
    }
}