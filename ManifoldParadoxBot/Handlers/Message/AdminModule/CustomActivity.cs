using Discord;

namespace ManifoldParadoxBot.Handlers.Message.AdminModule;

public class CustomActivity : IActivity
{
    public string Name { get; set; }
    public ActivityType Type { get; set; }
    public ActivityProperties Flags { get; set; }
    public string Details { get; set; }
}