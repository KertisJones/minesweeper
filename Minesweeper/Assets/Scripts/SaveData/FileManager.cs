using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            /*Debug.Log("Original " + a_FileContents);
            Debug.Log("Encrypted " + EncryptDecrypt(a_FileContents));
            Debug.Log("Decrypted " + EncryptDecrypt(EncryptDecrypt(a_FileContents)));*/
            File.WriteAllText(fullPath, EncryptDecrypt(a_FileContents));
            //File.WriteAllText(fullPath, a_FileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            if (File.Exists(fullPath))
            {
                result = EncryptDecrypt(File.ReadAllText(fullPath));
                return true;
            }
            else
            {
                result = "";
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }

    private static string EncryptDecrypt(string data)
    {
        string x = "3874521";
        string y = "5748864";
        string z = "8642156";

        StringBuilder sb = new StringBuilder("", data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            sb.Append((char)(data[i] ^ y[i % y.Length]));
        }
        return sb.ToString();
    }
}