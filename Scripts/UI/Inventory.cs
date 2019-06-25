using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    bool showInventory = false;
    public Rect inventoryRect = new Rect(Screen.width / 2, Screen.height / 2, 400, 400);
    public GameObject EmptyObject;
    public int InventorySize = 6;
    public GameObject[] invItems;
    public GameObject[] QuickItems;

    List<KeyValuePair<int, GameObject>> items = new List<KeyValuePair<int, GameObject>>();
    List<KeyValuePair<int, int>> itemCount = new List<KeyValuePair<int, int>>();

    void Awake()
    {
        InitializeInventory();
    }

    void Update()
    {
        if (Input.GetButtonUp("I_Key"))
        {
            showInventory = (showInventory) ? false : true;
        }
    }

    void OnGUI()
    {
        if (showInventory)
        {
            inventoryRect = GUI.Window(0, inventoryRect, InventoryGUI, "Inventory");
        }
    }

    void InventoryGUI(int ID)
    {
        GUILayout.BeginArea(new Rect(0, 50, 400, 350));

        GUILayout.BeginHorizontal();
        GUILayout.Button(itemCount[0].Value.ToString() + " " + invItems[0].name, GUILayout.Height(75));
        GUILayout.Button(itemCount[1].Value.ToString() + " " + invItems[1].name, GUILayout.Height(75));
        GUILayout.Button(itemCount[2].Value.ToString() + " " + invItems[2].name, GUILayout.Height(75));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Button(itemCount[3].Value.ToString() + " " + invItems[3].name, GUILayout.Height(75));
        GUILayout.Button(itemCount[4].Value.ToString() + " " + invItems[4].name, GUILayout.Height(75));
        GUILayout.Button(itemCount[5].Value.ToString() + " " + invItems[5].name, GUILayout.Height(75));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Button(QuickItems[0].name, GUILayout.Height(50));
        GUILayout.Button(QuickItems[1].name, GUILayout.Height(50));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    void InitializeInventory()
    {
        invItems = new GameObject[InventorySize];
        for(int i = 0; i < InventorySize; i++)
        {
            invItems[i] = EmptyObject;
            items.Add(new KeyValuePair<int, GameObject>(i, invItems[i]));
            itemCount.Add(new KeyValuePair<int, int>(i, 0));

            if (i < QuickItems.Length)
                QuickItems[i] = invItems[i];
        }
    }

    void AddToInventory(int HowMany, GameObject NewItem)
    {
        for(int i = 0; i < invItems.Length; i++)
        {
            if (invItems[i].name != "Empty")
            {
                if (invItems[i].name == NewItem.name)
                {
                    int val = itemCount[i].Value + HowMany;
                    itemCount[i] = new KeyValuePair<int, int>(itemCount[i].Key, val);
                }
                break;
            }
            else
            {
                int val = itemCount[i].Value + HowMany;
                invItems[i] = NewItem;
                items.Add(new KeyValuePair<int, GameObject>(i, NewItem));
                itemCount.Add(new KeyValuePair<int, int>(i, val));
                break;
            }
        }
    }

    void RemoveFromInventory(int HowMany, GameObject Item)
    {
        for (int i = 0; i < items.Capacity; i++)
        {
            if (invItems[i].name != "Empty")
            {
                if (invItems[i].name == Item.name)
                {
                    int val = itemCount[i].Value - HowMany;
                    itemCount[i] = new KeyValuePair<int, int>(itemCount[i].Key, val);
                    if (itemCount[i].Value <= 0)
                    {
                        invItems[i] = EmptyObject;
                        items[i] = new KeyValuePair<int, GameObject>(i, EmptyObject);
                        itemCount[i] = new KeyValuePair<int, int>(itemCount[i].Key, 0);
                    }
                    break;
                }
            }
        }
    }

    void SetQuickItem(GameObject NewItem, int QuickInput)
    {
        if (QuickItems[QuickInput].name != NewItem.name)
            if (QuickInput < QuickItems.Length)
                QuickItems[QuickInput] = NewItem;
    }
}
