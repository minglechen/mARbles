/// <summary>
/// Script for the fan to push marble away.
/// </summary>
public class Fan : Interferer
{
    /// <summary>
    /// Set the force direction such that it repels the player.
    /// </summary>
    public Fan()
    {
        ForceDirection = -1;
    }
}