using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
public interface ISerializable
{
    JObject Serialize();
    void Deserialize(string jsonString);
    string GetJsonKey();
}
