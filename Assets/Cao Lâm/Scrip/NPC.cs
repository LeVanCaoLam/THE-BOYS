using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    public GameObject NPCpanel;
    public TextMeshProUGUI NPCcontent;
    //public string[] content;//chuoi hoi thoai
    public List<HoiThoai> hoiThoais = new List<HoiThoai>();
    private Coroutine Coroutine;//de tu dong chay
    public Button skip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NPCpanel.SetActive(false);
        NPCcontent.text = "";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NPCpanel.SetActive(true);
            //chay chu
            Coroutine = StartCoroutine(ReadContent());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //truong hop buoc ra khoi vung chon thi stop doan hoi thoai 
        if (other.gameObject.CompareTag("Player"))
        {
            NPCpanel.SetActive(false);
            StopCoroutine(Coroutine);
        }
    }
    private IEnumerator ReadContent()//IEnumerator dung de thuc hien mot viec gi do
    {
        foreach (var item in hoiThoais)
        {
            if (item.doiTuong == "Player")
            {
                NPCcontent.color = Color.red;
            }
            else
            {
                NPCcontent.color = Color.green;
            }
            NPCcontent.text = "";
            foreach (var content in item.noiDung)
            {
                NPCcontent.text += content;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        NPCpanel.SetActive(false);
    }
    public void End()
    {
        //NPCpanel.SetActive(false);
        StopCoroutine(Coroutine);
    }
    [Serializable]
    public class HoiThoai
    {
        public string doiTuong;


        public string noiDung;
    }
}