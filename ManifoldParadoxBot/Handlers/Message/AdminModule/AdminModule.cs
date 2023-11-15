﻿using System.Text;
using Discord;
using Discord.Commands;
using Google.Cloud.Firestore;
using ManifoldParadoxBot.Handlers.Message.AdminModule.Exceptions;

namespace ManifoldParadoxBot.Handlers.Message.AdminModule;

public class AdminModule : ModuleBase<SocketCommandContext>
{
    [RequireOwner]
    [Command("SetActivityAsync")]
    public Task SetActivityAsync(string type, string name)
    {
        Task resultTask;
        bool isValidEnumValue = Enum.TryParse(type, out ActivityType activityType);

        try
        {
            if (name.Length > 15)
                throw new InvalidActivityNameException();

            if (!isValidEnumValue)
                throw new InvalidActivityTypeException();

            resultTask = Context.Client.SetActivityAsync(new CustomActivity()
            {
                Name = name,
                Type = activityType,
            });
        }
        catch (InvalidActivityNameException)
        {
            resultTask = ReplyAsync($"Activity name '{name}' is too long, try using 10 or less characters instead.");
        }
        catch (InvalidActivityTypeException)
        {
            StringBuilder validActivityTypeOptions = new StringBuilder();
            ActivityType[] allEnumValues = (ActivityType[])Enum.GetValues(typeof(ActivityType));

            // Iterate through and print the enum values
            foreach (ActivityType enumValue in allEnumValues)
            {
                validActivityTypeOptions.Append($"[{enumValue}] ");
            }

            validActivityTypeOptions.Remove(validActivityTypeOptions.Length - 1, 1);

            resultTask =
                ReplyAsync(
                    $"{activityType} is not a valid Activity Type. Try one of these: {validActivityTypeOptions}.");
        }

        return resultTask;
    }
}