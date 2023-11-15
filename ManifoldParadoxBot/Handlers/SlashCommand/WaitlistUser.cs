using Google.Cloud.Firestore;

namespace ManifoldParadoxBot.Handlers.SlashCommand;

[FirestoreData]
public class WaitlistUser
{
    [FirestoreProperty] public string Mention { get; set; }
    [FirestoreProperty] public string GlobalName { get; set; }
    [FirestoreProperty] public string UserName { get; set; }
    [FirestoreProperty] public string DateTime { get; set; }
}