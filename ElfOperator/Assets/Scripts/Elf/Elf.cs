using UnityEngine;

public class Elf : MonoBehaviour
{
    private ElfManager _manager;
    
    void Start()
    {
        _manager = GameObject.Find("ElfManager").GetComponent<ElfManager>();
    }

    
    void Update()
    {
        
    }
}
