using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using WebSocketSharp;

public class HNKTwitchChatListener : MonoBehaviour {

    [SerializeField] private string channelName = "nayatorinko";
    private WebSocket _socket;
    
    private void Start() {
        ConnectToTwitchChat();
    }

    private void OnDestroy() {
        if (_socket == null) 
            return;
        
        _socket.Close();
        Debug.Log("Twitch Chat Listener Disconnected");
    }

    private void ConnectToTwitchChat() {
        _socket = new WebSocket("wss://irc-ws.chat.twitch.tv:443");
        _socket.OnOpen += (sender, e) => {
            Debug.Log("Connected to Twitch Chat!");
            _socket.Send("CAP REQ :twitch.tv/tags twitch.tv/commands");
            _socket.Send("NICK justinfan12345"); // No login required (anonymous)
            _socket.Send($"JOIN #{channelName}");
        };
        
        _socket.OnMessage += (sender, e) => {
            if (!e.Data.Contains("PRIVMSG"))
                return;
            
            var userData = e.Data.Split('!')[0].Substring(1);
            var message = e.Data.Split(new[] { "PRIVMSG #" + channelName + " :" }, StringSplitOptions.None)[1];
            ProcessUserData(userData);
            // Debug.Log($"{userData}: {message}");
        };

        _socket.OnError += (sender, e) => {
            Debug.LogError($"Twitch Chat Error: {e.Message}");
        };

        _socket.OnClose += (sender, e) => {
            Debug.Log("Twitch Chat Disconnected, attempting to reconnect...");
            StartCoroutine(Reconnect());
        };

        _socket.Connect();
    }

    private void ProcessUserData(string userData) {
        var data = userData.Split(';');
        var userDictionary = data.Select(property => property.Split('='))
            .Where(keyValue => keyValue.Length == 2)
            .ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);
        
        var chatter = new HNKTwitchChatter(userDictionary);
        var chatterJson = JsonUtility.ToJson(chatter);
        Debug.Log(chatterJson);
    }
    
    private IEnumerator Reconnect() {
        yield return new WaitForSeconds(5);
        ConnectToTwitchChat();
    }
}
