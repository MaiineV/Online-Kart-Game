using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Text _actualLap;
    [SerializeField] GameObject _tabScreen;
    [SerializeField] GameObject _winScreen;
    [SerializeField] List<Text> _nicksPlayer;

    private void Start()
    {
        StartCoroutine(WaitPhoton());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            _tabScreen.SetActive(true);
        else if(Input.GetKeyUp(KeyCode.Tab))
            _tabScreen.SetActive(false);
    }

    public void ChangeLapCounter(int actualLap)
    {
        _actualLap.text = actualLap.ToString() + " / 3";
    }

    public void ChangeScoreboard(List<Player> orderNick)
    {
        for (int i = 0; i < orderNick.Count; i++)
        {
            _nicksPlayer[i].text = orderNick[i].NickName;
        }
    }

    public void WinScreen()
    {
        _winScreen.SetActive(true);
    }

    IEnumerator WaitPhoton()
    {
        yield return new WaitForSeconds(2f);
        PlayersVar.instance.ReciveCanvas(this);
    }
}