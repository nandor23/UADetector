using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers;

public class BotParser : IBotParser
{
    private const string ResourceName = "Regexes.Resources.bots.json";
    private static readonly IEnumerable<Bot> Bots;
    private static readonly Regex CombinedRegex;


    static BotParser()
    {
        (Bots, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Bot>(ResourceName);
    }

    public static bool IsBot(string userAgent)
    {
        return CombinedRegex.IsMatch(userAgent);
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result)
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            foreach (var bot in Bots)
            {
                var match = bot.Regex.Match(userAgent);

                if (match.Success)
                {
                    result = new BotInfo
                    {
                        Name = bot.Name,
                        Category = bot.Category,
                        Url = bot.Url,
                        Producer = string.IsNullOrEmpty(bot.Producer?.Name) && string.IsNullOrEmpty(bot.Producer?.Url)
                            ? null
                            : new ProducerInfo { Name = bot.Producer?.Name, Url = bot.Producer?.Url, },
                    };

                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
