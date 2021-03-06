﻿#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software,
// and you are welcome to redistribute it under certain conditions; See
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion

using System.Collections.Generic;
using System.Xml;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

[MoonSharpUserData]
public class Quest : IPrototypable
{
    public string Name { get; set; }

    public string Type
    {
        get { return Name; }
    }

    public string Description { get; set; }

    public List<QuestGoal> Goals { get; set; }

    public bool IsAccepted { get; set; }

    public bool IsCompleted { get; set; }

    public List<QuestReward> Rewards { get; set; }

    public List<string> RequiredQuests { get; set; }

    public void ReadXmlPrototype(XmlReader reader_parent)
    {
        Name = reader_parent.GetAttribute("Name");
        Goals = new List<QuestGoal>();
        Rewards = new List<QuestReward>();   
        RequiredQuests = new List<string>();

        XmlReader reader = reader_parent.ReadSubtree();

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "Description":
                    reader.Read();
                    Description = reader.ReadContentAsString();
                    break;
                case "PreRequiredCompletedQuests":
                    XmlReader subReader = reader.ReadSubtree();

                    while (subReader.Read())
                    {
                        if (subReader.Name == "PreRequiredCompletedQuest")
                        {
                            RequiredQuests.Add(subReader.GetAttribute("Name"));
                        }
                    }

                    break;
                case "Goals":
                    XmlReader goals_reader = reader.ReadSubtree();
                    while (goals_reader.Read())
                    {
                        if (goals_reader.Name == "Goal")
                        {
                            QuestGoal goal = new QuestGoal();
                            goal.ReadXmlPrototype(goals_reader);
                            Goals.Add(goal);
                        }
                    }

                    break;
                case "Rewards": 
                    XmlReader reward_reader = reader.ReadSubtree();
                    while (reward_reader.Read())
                    {
                        if (reward_reader.Name == "Reward")
                        {
                            QuestReward reward = new QuestReward();
                            reward.ReadXmlPrototype(reward_reader);
                            Rewards.Add(reward);
                        }
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Reads the prototype from the specified JObject.
    /// </summary>
    /// <param name="jsonProto">The JProperty containing the prototype.</param>
    public void ReadJsonPrototype(JProperty jsonProto)
    {
        Name = jsonProto.Name;
        JToken innerJson = jsonProto.Value;

        Goals = new List<QuestGoal>();
        Rewards = new List<QuestReward>();
        RequiredQuests = new List<string>();

        Description = PrototypeReader.ReadJson(Description, innerJson["Description"]);

        if (innerJson["RequiredQuests"] != null)
        {
            foreach (JToken token in innerJson["RequiredQuests"])
            {
                RequiredQuests.Add((string)token);
            }
        }

        if (innerJson["Goals"] != null)
        {
            foreach (JToken token in innerJson["Goals"])
            {
                QuestGoal goal = new QuestGoal();
                goal.ReadJsonPrototype(token);
                Goals.Add(goal);
            }
        }

        if (innerJson["Rewards"] != null)
        {
            foreach (JToken token in innerJson["Rewards"])
            {
                QuestReward reward = new QuestReward();
                reward.ReadJsonPrototype(token);
                Rewards.Add(reward);
            }
        }
    }
}