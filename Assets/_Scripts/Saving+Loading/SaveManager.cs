using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class SaveManager
{
    public string saveFileName = "save.xml";

    private static readonly byte[] Key = Encoding.UTF8.GetBytes("a8F5kD2uWq9P0bXr7nVm4zCj6YtErU1Q");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("H7gT4vLxQ2mZ1pNc");

    public void SavePlayer(Transform playerTransform, Weapon weapon, bool isWeaponEquipped)
    {
        PlayerData data = new PlayerData
        {
            Transform = new TransformData
            {
                Position = new Vector3Data(playerTransform.position),
                Rotation = new Vector3Data(playerTransform.eulerAngles),
                Scale = new Vector3Data(playerTransform.localScale)
            },
            Health = GameManager.Instance.PlayerHealth,
            IsWeaponEquipped = isWeaponEquipped,
            WeaponPosition = weapon != null ? new Vector3Data(weapon.transform.position) : null,
            WeaponRotation = weapon != null ? new Vector3Data(weapon.transform.eulerAngles) : null
        };

        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (MemoryStream ms = new MemoryStream())
        {
            serializer.Serialize(ms, data);
            byte[] encrypted = Encrypt(ms.ToArray());
            File.WriteAllBytes(path, encrypted);
        }
        Debug.Log($"Player saved to {path}");
    }

    public PlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found at {path}");
            return null;
        }

        byte[] encryptedDate = File.ReadAllBytes(path);
        byte[] decryptedData = Decrypt(encryptedDate);

        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (MemoryStream ms = new MemoryStream(decryptedData))
        {
            return (PlayerData)serializer.Deserialize(ms);
        }    
    }

    public bool HasSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        return File.Exists(path);
    }

    private byte[] Encrypt(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    private byte[] Decrypt(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(new MemoryStream(data), aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cs.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}

