using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelect : Photon.MonoBehaviour
{
    //플레이어마다 왼쪽에 있는 스펠 표시
    [SerializeField]
    private Image spellImage1 = null;
    [SerializeField]
    private Image spellImage2 = null;

    //동기화용 함수
    public void ChangeSpellImage(string spellD, string spellF)
    {
        if (!spellD.Equals(string.Empty))
            spellImage1.sprite = Resources.Load<Sprite>("Spell/" + spellD);
        if (!spellF.Equals(string.Empty))
            spellImage2.sprite = Resources.Load<Sprite>("Spell/" + spellF);
    }
}

