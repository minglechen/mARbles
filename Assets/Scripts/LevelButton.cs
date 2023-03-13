using Menu;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Script that handles button highlighting in menus when button is selected.
/// </summary>
[RequireComponent(typeof(Image))]
public class LevelButton : MonoBehaviour
{
    public GameObject text;
    Image m_Image;
    private ARMenu _menu;

    public ARMenu Menu
    {
        set => _menu = value;
    }
    
    private Text _textComponent;
    private string _text;
    private bool _isHighlighted;
    

    void Start()
    {
        m_Image = GetComponent<Image>();
        _textComponent = text.GetComponent<Text>();
        _textComponent.text = _text;
        m_Image.enabled = _isHighlighted;
    }

    /// <summary>
    /// Method that shows or hides the image that highlights the button.
    /// </summary>
    public void HighlightButton(bool s)
    {
        if (s) _menu.ClearLevelButtonHighlights(this);
        if (m_Image)
        {
            m_Image.enabled = s;
        }
        _isHighlighted = s;
    }
    
    public void SetText(string s)
    {
        if (_textComponent)
        {
            _textComponent.text = s;
        }
        else
        {
            _text = s;
        }
    }
}