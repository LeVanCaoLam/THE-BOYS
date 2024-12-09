
using System.Collections.Generic;
using UnityEngine;

public class Damezone : MonoBehaviour
{
    public Collider dameCollider;
    public int dame = 20;
    public List<Collider> listDame = new List<Collider>();
    public string targetTag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dameCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy") && !listDame.Contains(other))
        {
            listDame.Add(other);
            other.GetComponent<Enemy>().TakeDame(dame);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag) && !listDame.Contains(other))
        {
            listDame.Add(other);
            other.GetComponent<Enemy>().TakeDame(dame);
        }
    }
    public void beginDame()
    {
        listDame.Clear();
        dameCollider.enabled = true;
    }
    public void endDame()
    {
        listDame.Clear();
        dameCollider.enabled = false;
    }
}