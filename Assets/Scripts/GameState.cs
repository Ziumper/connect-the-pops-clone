using System;
using System.Collections.Generic;

[Serializable]
public class GameState 
{
    public int SelectedResult;
    public Node First;
    public Node Last;
    public Node Previous;

    public bool IsActive()
    {
        return First != null && Last != null && First != Last;
    }
}
