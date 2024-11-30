namespace SpaceBattle.Lib;
using Google.Protobuf.Collections;
using System.Collections.Generic;
public class ProtobufMapperStrategy{

   public static object Run(MapField<string, string> protoMap){

    Dictionary<string, object> newDict = new();

    foreach(string key in protoMap.Keys)
    {
        newDict[key] = (object) protoMap[key];
    }

    return newDict;
   } 
}