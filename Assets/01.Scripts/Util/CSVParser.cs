using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public static class CSVParser
{
    public static (List<DialogueEntry>, HashSet<SpeakerType>) ParseCSVForChapter(string filePath, int chapter)
    {
        List<DialogueEntry> dialogues = new List<DialogueEntry>();
        HashSet<SpeakerType> participants = new HashSet<SpeakerType>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            var matches = Regex.Matches(line, "(\"[^\"]*\"|[^,]+)");
            if (matches.Count >= 4 && int.TryParse(matches[1].Value.Trim('"'), out int chapterFromCSV) && chapterFromCSV == chapter)
            {
                var speakerType = (SpeakerType)Enum.Parse(typeof(SpeakerType), matches[2].Value.Trim('"'));
                DialogueEntry entry = new DialogueEntry
                {
                    speaker = speakerType,
                    dialogue = matches[3].Value.Trim('"')
                };
                dialogues.Add(entry);
                participants.Add(speakerType);
            }
        }

        return (dialogues, participants);
    }
}