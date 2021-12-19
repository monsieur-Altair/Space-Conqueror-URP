using System;
using UnityEngine;

public class MyException:Exception
{
    public MyException(string message)
    {
        Debug.LogError(message);
    }
}