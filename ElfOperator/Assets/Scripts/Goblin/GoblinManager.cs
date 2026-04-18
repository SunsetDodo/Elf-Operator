using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public struct Operation
{
    public bool isEnabled;
    public string name;
    public Sprite sprite;
    public int count;
    public UnityEvent onUse;
}

public class GoblinManager : MonoBehaviour
{
    
    [SerializeField] private float singleDigitFontSize;
    [SerializeField] private float doubleDigitFontSize;

    [SerializeField] private Operation nullOperation;
    [SerializeField] private Operation walkLeft;
    [SerializeField] private Operation walkRight;
    [SerializeField] private Operation jumpLeft;
    [SerializeField] private Operation jumpRight;
    [SerializeField] private Operation climbDown;
    [SerializeField] private Operation climbUp;
    [SerializeField] private Operation bundle2;
    [SerializeField] private Operation bundle3;
    
    [SerializeField] private List<GameObject> buttons;
    private Dictionary<GameObject, Operation> _inventory;
    
    
    public void ProcessButton(GameObject button)
    {
        if (!_inventory.ContainsKey(button)) return;
        var op =  _inventory[button];
        op.count--;
        op.onUse.Invoke();

        if (op.count == 0)
        {
            button.GetComponentInChildren<DragNDrop>().enabled = false;
        }
        _inventory[button] = op;
        
        UpdateHUD();
    }

    public void Start()
    {
        var inventory = new SortedDictionary<string, int>
        {
            ["walk_right"] = -1,
            ["walk_left"] = 2,
            ["jump_right"] = 5,
            ["jump_left"] = 2,
            ["bundle_2"] = 1,
            ["climb_up"] = -1,
            
        };
        InitButtons();
        InitInventory(inventory);
        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        Debug.Log(_inventory.Count);
        foreach (var (go, op) in _inventory)
        {
            go.SetActive(op.isEnabled);
            if (!op.isEnabled) continue;
            
            var sr =  go.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = op.sprite;
            
            var tmp = go.GetComponentInChildren<TextMeshPro>();
            tmp.text = op.count < 0 ?  "FREE" : "x" + op.count;
            tmp.fontSize = op.count is >= 0 and < 9 ? singleDigitFontSize : doubleDigitFontSize;
        }
    }

    private void InitButtons()
    {
        foreach (var button in buttons)
        {
            button.SetActive(false);
            button.GetComponentInChildren<DragNDrop>().enabled = true;
        }
    }
    
    private void InitInventory(SortedDictionary<string, int> inventory)
    {
        var i = 0;
        foreach (var (opStr, count) in inventory)
        {
            Debug.Log(opStr + " " + count);
            switch (opStr)
            {
                case "walk_left":
                    walkLeft.count = count;
                    walkLeft.isEnabled = true;
                    _inventory[buttons[i++]] = walkLeft;
                    break;
                case "walk_right":
                    walkRight.count = count;
                    walkRight.isEnabled = true;
                    _inventory[buttons[i++]] = walkRight;
                    break;
                case "jump_left":
                    jumpLeft.count = count;
                    jumpLeft.isEnabled = true;
                    _inventory[buttons[i++]] = jumpLeft;
                    break;
                case "jump_right":
                    jumpRight.count = count;
                    jumpRight.isEnabled = true;
                    _inventory[buttons[i++]] = jumpRight;
                    break;
                case "climb_down":
                    climbDown.count = count;
                    climbDown.isEnabled = true;
                    _inventory[buttons[i++]] = climbDown;
                    break;
                case "climb_up":
                    climbUp.count = count;
                    climbUp.isEnabled = true;
                    _inventory[buttons[i++]] = climbUp;
                    break;
                case "bundle_2":
                    bundle2.count = count;
                    bundle2.isEnabled = true;
                    _inventory[buttons[i++]] = bundle2;
                    break;
                case "bundle_3":
                    bundle3.count = count;
                    bundle3.isEnabled = true;
                    _inventory[buttons[i++]] = bundle3;
                    break;
            }
        }
    }
}
