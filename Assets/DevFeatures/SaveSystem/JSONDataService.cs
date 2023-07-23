using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace DevFeatures.SaveSystem
{
    public class JsonDataService : IDataService
    {
        private const string KEY = "v04pUiWeHI6DRb0VEUP864Cw5UxeN1EtC9rhjmkYgPE=";
        private const string IV = "Hf16yvj6A49RcCR0iDMsmQ==";

        public bool SaveData<T>(string relativePath, T data, bool encrypted)
        {
            var path = Application.persistentDataPath + relativePath;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log($"Data exists ({relativePath}). Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log($"Writing file ({relativePath}) for the first time!");
                }
                using var stream = File.Create(path);
                if (encrypted)
                {
                    WriteEncryptedData(data, stream);
                }
                else
                {
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(data));
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        private void WriteEncryptedData<T>(T data, FileStream stream)
        {
            using var aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);
            using var cryptoTransform = aesProvider.CreateEncryptor();
            using var cryptoStream = new CryptoStream(
                stream,
                cryptoTransform,
                CryptoStreamMode.Write
            );
            cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
        }


        public T LoadData<T>(string relativePath, bool encrypted)
        {
            var path = Application.persistentDataPath + relativePath;

            if (!File.Exists(path))
            {
                Debug.LogError($"Cannot load file at {path}. File does not exist!");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try
            {
                T data;
                if (encrypted)
                {
                    data = ReadEncryptedData<T>(path);
                }
                else
                {
                    data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        private T ReadEncryptedData<T>(string path)
        {
            var fileBytes = File.ReadAllBytes(path);
            using var aesProvider = Aes.Create();

            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using var cryptoTransform = aesProvider.CreateDecryptor(
                aesProvider.Key,
                aesProvider.IV
            );
            using var decryptionStream = new MemoryStream(fileBytes);
            using var cryptoStream = new CryptoStream(
                decryptionStream,
                cryptoTransform,
                CryptoStreamMode.Read
            );
            using var reader = new StreamReader(cryptoStream);

            var result = reader.ReadToEnd();

            Debug.Log($"Decrypted result (if the following is not legible, probably wrong key or iv): {result}");
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
