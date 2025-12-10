using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class LoginManager : MonoBehaviour
{

    /// <summary>
    /// This File Manage Login Button event and check Login condition
    /// </summary>
    [SerializeField] private TMP_InputField userNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text msgText;

    private void Start()
    {
        loginButton.onClick.RemoveListener(LoginButtonCLick);
        loginButton.onClick.AddListener(LoginButtonCLick);
    }
    public void LoginButtonCLick()
    {
        if(string.IsNullOrEmpty(userNameInputField.text) || string.IsNullOrEmpty(passwordInputField.text))
        {
            msgText.text = "Both Input filed must be not Empty for login";
        }
        else
        {
            SceneManager.LoadScene("Humanoid");
        }
    }
}
