using TMPro;
using UnityEngine;

public class AuthView : ViewBase
{
    
    public ViewButtonBase login;


    public TMP_InputField UserName;
    public TMP_InputField Password;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        login.OnClick += (e) => 
        {
            Debug.Log(UserName.text);
            Debug.Log(Password.text);

            
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
