// Models/User.cs
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserPreference> Preferences { get; set; }
}
