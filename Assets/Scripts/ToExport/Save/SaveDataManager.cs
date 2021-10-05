using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.Serialization;


[Serializable]
public class RunnerLevelData : ISerializable
{
    public string levelName = "";
    public int levelNumber = 0;
    public bool hasBeenPlayed = false;
    public bool locked = false;
    public int starNumber = 0;
    public int localStarNumber = 0;

    public int star1Points;
    public int star2Points;
    public int star3Points;
    
    public int currentPoints;
    public int maxPoints;
    
    
    public JObject Serialize()
    {
        var jsonString = JsonUtility.ToJson(this);
        var returnVal = JObject.Parse(jsonString);
        return returnVal;
    }

    public void Deserialize(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }

    public string GetJsonKey()
    {
        return levelName;
    }

    public void CheckCurrentPoints()
    {
        if (maxPoints <= currentPoints)
            maxPoints = currentPoints;
        
        //Global star number
        if (maxPoints < star1Points)
            starNumber = 0;
        else if (maxPoints >= star1Points && maxPoints < star2Points)
        {
            if(starNumber<=1)
                starNumber = 1;
        }
        else if (maxPoints >= star2Points && maxPoints < star3Points)
        {
            if(starNumber<=2)
                starNumber = 2;
        }
        else if (maxPoints >= star3Points)
        {
            if(starNumber<=3)
                starNumber = 3;
        }
        
        //Local star Number
        if (currentPoints < star1Points)
            localStarNumber = 0;
        else if (currentPoints >= star1Points && currentPoints < star2Points)
        {
            if(localStarNumber<=1)
                localStarNumber = 1;
        }
        else if (currentPoints >= star2Points && currentPoints < star3Points)
        {
            if(localStarNumber<=2)
                localStarNumber = 2;
        }
        else if (currentPoints >= star3Points)
        {
            if(localStarNumber<=3)
                localStarNumber = 3;
        }
    }
}


[Serializable]
public class GameData : ISerializable
{
    public string gameName;
    public bool gameHasPlayed;

    public bool musicLevel;
    public bool sfxLevel;

    public int gender;
    // public bool hasBeenPlayed;

    public JObject Serialize()
    {
        var jsonString = JsonUtility.ToJson(this);
        var returnVal = JObject.Parse(jsonString);
        return returnVal;
    }

    public void Deserialize(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }

    public string GetJsonKey()
    {
        return gameName;
    }
}


public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager _instance;

    public List<RunnerLevelData> levels = new List<RunnerLevelData>();
    public GameData gameData;

    public Dictionary<string, RunnerLevelData> kittenDictionary = new Dictionary<string, RunnerLevelData>();

    public List<ISerializable> _objToSaveList = new List<ISerializable>();

    #region Encryption

    private readonly byte[] _key =
        {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

    private readonly byte[] _initializationVector =
        {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

    #endregion

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != null)
            Destroy(this);

        //Add basic game data to the saving list
        _objToSaveList.Add(gameData);

        //Add levels to the saving list
        for (var i = 0; i < levels.Count; i++) _objToSaveList.Add(levels[i]);

        //Add indexes and keys to the levels dictionary
        for (var i = 0; i < levels.Count; i++) kittenDictionary.Add(levels[i].levelName, levels[i]);
    }

    #region Getters and Setters

    public RunnerLevelData GetLevelData(string _key)
    {
        RunnerLevelData temp = null;
        if (kittenDictionary.TryGetValue(_key, out temp))
        {
            return temp;
        }

        print("Error in dictionary searching");
        return null;
    }

    public int LevelDataSize()
    {
        return levels.Count;
    }

    // public void GameIsPlayed()
    // {
    //     gameData.gameHasPlayed = true;
    //     SaveData();
    // }

    public bool GetGamePlayed()
    {
        return gameData.gameHasPlayed;
    }

    #endregion

    #region Save Data Utils

    public void SaveData()
    {
        var jSaveGame = new JObject();

        for (var i = 0; i < _objToSaveList.Count; i++)
            jSaveGame.Add(_objToSaveList[i].GetJsonKey(), _objToSaveList[i].Serialize());

        var filePath = Application.persistentDataPath + "/runner2d.inart";
        print("Saving from: " + filePath);

        var encryptedSavegame = Encrypt(jSaveGame.ToString());
        File.WriteAllBytes(filePath, encryptedSavegame);
    }

    public void LoadData()
    {
        var filePath = Application.persistentDataPath + "/runner2d.inart";
        print("Loading from: " + filePath);

        var decryptedSavegame = File.ReadAllBytes(filePath);
        var jsonString = Decrypt(decryptedSavegame);

        print(jsonString);
        //TODO: Deserialize the JsonString
        var jSavegame = JObject.Parse(jsonString);

        for (var i = 0; i < _objToSaveList.Count; i++)
        {
            var curElement = _objToSaveList[i];
            var elementJsonString = jSavegame[_objToSaveList[i].GetJsonKey()].ToString();
            curElement.Deserialize(elementJsonString);
        }
    }

    #endregion

    #region Encryption Methods

    private byte[] Encrypt(string message)
    {
        var aes = new AesManaged();
        var encryptor = aes.CreateEncryptor(_key, _initializationVector);

        var memoryStream = new MemoryStream();
        var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        var sw = new StreamWriter(cryptoStream);

        sw.WriteLine(message);

        sw.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }

    private string Decrypt(byte[] message)
    {
        var aes = new AesManaged();
        var decrypter = aes.CreateDecryptor(_key, _initializationVector);

        var memoryStream = new MemoryStream(message);
        var cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Read);
        var sr = new StreamReader(cryptoStream);

        var decryptedMessage = sr.ReadToEnd();

        memoryStream.Close();
        cryptoStream.Close();
        sr.Close();

        return decryptedMessage;
    }

    #endregion
}
