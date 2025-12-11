using System.Net.Mail;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
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
        string userN = userNameInputField.text;
        if (string.IsNullOrEmpty(userN) || string.IsNullOrEmpty(passwordInputField.text))
        {
            msgText.text = "Both Input filed must be not Empty for login";
            return;
        }

        if (!IsValidGmail(userN))
        {
            msgText.text = "Unvalid GmailId";
            return;
        }
        else
        {
            msgText.text = "Valid Login";
            Invoke("LoadHumonoidSceen",1.0f);
        }
    }
    public bool IsValidGmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
    }
    void LoadHumonoidSceen()
    {
        SceneManager.LoadScene("Humanoid");
    }
    
}
