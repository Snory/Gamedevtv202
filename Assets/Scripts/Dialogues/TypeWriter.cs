using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypeWriter : MonoBehaviour
{
    [SerializeField]
    private float _speedInSec;

    public void Type(string text, Action<string> writtingCallback, Action finishedCallback)
    {
        StartCoroutine(TypeText(text,writtingCallback, finishedCallback));
       
    }

    private IEnumerator TypeText(string text, Action<string> writtingCallback, Action finishedCallback)
    {
        string typedText = "";

        foreach(char letter in text.ToCharArray())
        {
            typedText += letter;
            writtingCallback(typedText);

            yield return new WaitForSeconds(_speedInSec);
        }

        finishedCallback();
    }

    public void OnCancel()
    {
        StopAllCoroutines();
    }
}
