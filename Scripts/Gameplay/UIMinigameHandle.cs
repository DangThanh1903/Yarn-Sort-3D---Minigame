using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMinigameHandle : MonoBehaviour
{
    public static UIMinigameHandle Ins;
    void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public PopUpShop SpawnShop()
    {
        GameObject popup = Instantiate(GameManager.Ins.GameDataAssets.PopUpShop, transform);
        popup.transform.localScale = Vector3.one;
        PopUpShop popupShop = popup.GetComponent<PopUpShop>();
        popupShop.ActiveExitButton();
        popupShop.MoveToCoin();
        popup.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        popup.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        popup.transform.SetAsLastSibling();
        return popupShop;
    }
}
