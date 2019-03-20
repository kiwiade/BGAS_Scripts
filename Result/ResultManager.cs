using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : Singleton<ResultManager>
{
    public class ResultData
    {
        public bool isMine = false;
        public string championName;
        public string nickName;
        public int level;
        public int kill;
        public int death;
        public int assist;
        public int cs;
        public int[] items = new int[6];
        public int accessoryItem;

        public ResultData ClassCopy()
        {
            return (ResultData)this.MemberwiseClone();
        }
    }

    [HideInInspector]
    public List<ResultData> blueTeamResults = new List<ResultData>();
    [HideInInspector]
    public List<ResultData> redTeamResults = new List<ResultData>();

    public string result = "";

    public void ListReset()
    {
        blueTeamResults.Clear();
        redTeamResults.Clear();
    }

    public void ResultInput(ResultData result, string team)
    {
        ResultData newResult = result.ClassCopy();
        if(team.Equals("red"))
        {
            redTeamResults.Add(newResult);
        }
        else if(team.Equals("blue"))
        {
            blueTeamResults.Add(newResult);
        }
    }
}