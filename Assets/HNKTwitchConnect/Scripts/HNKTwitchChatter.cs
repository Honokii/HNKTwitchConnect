using System;
using System.Collections.Generic;

[Serializable]
public class HNKTwitchChatter {
    public string userId;
    public string color;
    public string displayName;
    public bool firstMessage;
    public bool mod;
    public bool subscriber;

    public HNKTwitchChatter(Dictionary<string, string> userDictionary) {
        userId = userDictionary["user-id"];
        color = userDictionary["color"];
        displayName = userDictionary["display-name"];
        firstMessage = userDictionary["first-msg"] == "1";
        mod = userDictionary["mod"] == "1";
        subscriber = userDictionary["subscriber"] == "1";
    }
}