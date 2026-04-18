using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElfManager : MonoBehaviour
{
    [SerializeField] private Sprite walkLeftSprite;
    [SerializeField] private Sprite walkRightSprite;
    [SerializeField] private Sprite jumpLeftSprite;
    [SerializeField] private Sprite jumpRightSprite;
    [SerializeField] private Sprite climbSprite;
    [SerializeField] private Sprite bundle2Sprite;
    [SerializeField] private Sprite bundle3Sprite;
    
    [SerializeField] private List<GameObject> buttons;
    
    
    // bool represents left or right to not have to create two seperate events
    public UnityEvent<bool> walk;
    public UnityEvent<bool> jump;
    public UnityEvent climb;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
