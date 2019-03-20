using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonEventCode {
    // Selection Scene
    public static readonly byte TIMER_SHARE = 1;
    public static readonly byte SPELL_SHARE = 0;

    // InGame Scene
    public static readonly byte MINION_POOLSETTING = 20;

    public static readonly byte LOADING_COMPLETE = 33;
    public static readonly byte RELEASE_STARTING_WALL = 34;
    public static readonly byte SYNC_USER_VIEWID = 34;

    public static readonly byte PING_REDTEAM = 30;
    public static readonly byte PING_BLUETEAM = 40;

    public static readonly byte SYSTEM_MESSAGE_ALLTEAM = 150;
    public static readonly byte SYSTEM_MESSAGE_KILL = 160;
    public static readonly byte SYSTEM_MESSAGE_REDTEAM = 131;
    public static readonly byte SYSTEM_MESSAGE_BLUETEAM = 141;

    public static readonly byte SURRENDER_UIOPEN_REDTEAM = 136;
    public static readonly byte SURRENDER_UIOPEN_BLUETEAM = 146;
    public static readonly byte SURRENDER_VOTE_REDTEAM = 133;
    public static readonly byte SURRENDER_VOTE_BLUETEAM = 143;

    public static readonly byte GAME_ENDED = 151;
}
