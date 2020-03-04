using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI friendName;
    public string friendId;

    public void SetFriendName(string name, string steamId)
    {
        friendName.text = name;
        friendId = steamId;
    }

    public void JoinFriend()
    {
        NetworkManagerMenu.instance.JoinFriend(friendId);
    }
}
