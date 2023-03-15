using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that handles button highlighting in menus when button is selected.
/// </summary>
[RequireComponent(typeof(Image))]
public class LevelButton : MonoBehaviour
{
    [Tooltip("The text component of the level button.")]
    [SerializeField]
    private GameObject text;
    
    /// <summary>
    /// The ARMenu this button is in. (set only)
    /// </summary>
    public ARMenu Menu
    {
        set => _menu = value;
    }
    
    private Image _image;
    private ARMenu _menu;
    private Text _textComponent;
    private string _text;
    private bool _isHighlighted;
    
    /// <summary>
    /// Method that shows or hides the image that highlights the button.
    /// </summary>
    public void HighlightButton(bool s)
    {
        if (s) _menu.ClearLevelButtonHighlights(this);
        if (_image)
        {
            _image.enabled = s;
        }
        _isHighlighted = s;
    }
    
    /// <summary>
    /// Sets the text to display on the button.
    /// </summary>
    /// <param name="s">The text to set.</param>
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
    
    void Start()
    {
        _image = GetComponent<Image>();
        _textComponent = text.GetComponent<Text>();
        _textComponent.text = _text;
        _image.enabled = _isHighlighted;
    }
}