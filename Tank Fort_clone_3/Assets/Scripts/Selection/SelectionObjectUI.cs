using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionObjectUI : SelectionObject
{
    public void OnClick()
    {
        FindObjectOfType<SelectionBoardUI>().SelectionObjectClicked(this);
    }

}
