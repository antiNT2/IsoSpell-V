using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipWeaponSlotDisplay : MonoBehaviour
{
    [SerializeField]
    Image weaponIconDisplay;
    public int weaponId;

    public void SetWeaponId(int id)
    {
        weaponId = id;
        weaponIconDisplay.sprite = GameManager.instance.weaponDatabase.allWeapons[id].weaponIcon;
    }

    public void OnSelect(BaseEventData eventData)
    {
        //print(eventData.currentInputModule.);
       // if (eventData.currentInputModule != BaseInputModule.)
            eventData.currentInputModule.transform.parent.GetComponent<PlayerWeapon>().EquipWeapon(weaponId);
        GameManager.instance.ChangePlayerReadyState(eventData.currentInputModule.transform.parent.gameObject, true);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GameManager.instance.ChangePlayerReadyState(eventData.currentInputModule.transform.parent.gameObject);
    }
}
