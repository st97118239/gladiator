using UnityEngine;

[CreateAssetMenu(menuName = "TutorialInfo")]
public class TutorialInfo : ScriptableObject
{
    public string text;
    public TutorialInfo nextPanel;
    public int id;
}
