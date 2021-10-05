using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;

public class SavegameManager_Interfaces : MonoBehaviour
{
    public List<ISerializable> _objToSaveList;

    private void Awake()
    {
        _objToSaveList=new List<ISerializable>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //TODO: Serialize the JsonString
            var jSaveGame = new JObject();

            for (var i = 0; i < _objToSaveList.Count; i++)
            {
                jSaveGame.Add(_objToSaveList[i].GetJsonKey(),_objToSaveList[i].Serialize());
            }

            var filePath = Application.persistentDataPath + "/save_multiple_go_interfaces.sav";
            //Working
            // var sw = new StreamWriter(filePath);
            // print("Saving to: " + filePath);
            // sw.WriteLine(jSaveGame.ToString());
            // sw.Close();

            byte[] encryptedSavegame= Encrypt(jSaveGame.ToString());
            File.WriteAllBytes(filePath,encryptedSavegame);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var filePath = Application.persistentDataPath + "/save_multiple_go_interfaces.sav";
            print("Loading from: " + filePath);
     
            //Working
            // var sr = new StreamReader(filePath);
            // var jsonString = sr.ReadToEnd();
            // sr.Close();
         
            byte[] decryptedSavegame= File.ReadAllBytes(filePath);
            string jsonString = Decrypt(decryptedSavegame);
         
            print(jsonString);
            //TODO: Deserialize the JsonString
            JObject jSavegame = JObject.Parse(jsonString);
         
            for (int i = 0; i < _objToSaveList.Count; i++)
            {
                ISerializable curElement = _objToSaveList[i];
                string elementJsonString = jSavegame[_objToSaveList[i].GetJsonKey()].ToString();
                curElement.Deserialize(elementJsonString);
            }
        }
    }
    
    private readonly byte[] _key =
        {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};


    private readonly byte[] _initializationVector =
        {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

    private byte[] Encrypt(string message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform encryptor = aes.CreateEncryptor(_key, _initializationVector);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        StreamWriter sw = new StreamWriter(cryptoStream);

        sw.WriteLine(message);

        sw.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }

    private string Decrypt(byte[] message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform decrypter = aes.CreateDecryptor(_key, _initializationVector);

        MemoryStream memoryStream = new MemoryStream(message);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(cryptoStream);

        string decryptedMessage = sr.ReadToEnd();

        memoryStream.Close();
        cryptoStream.Close();
        sr.Close();
      
        return decryptedMessage;
    }
}
