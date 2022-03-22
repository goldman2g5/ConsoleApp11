using System.Diagnostics;
using System.Security.Cryptography;

namespace Cosoleapp3;

public class Skill
{
    public string Name;
    public Double Damage;
    public bool UseOnAllies;
    public bool Aoe;
    public List<int> Targets;
    public List<Status> StatusList = new List<Status>();
    private static Dictionary<string, Skill> SkillList = new Dictionary<string, Skill>() {};

    public Skill(string name, Double damage, List<int> targets, List<Status> statusList, bool useonaliies = false, bool aoe = false)
    {
        Damage = damage;
        Name = name;
        UseOnAllies = useonaliies;
        StatusList = statusList;
        Targets = targets;
        UseOnAllies = useonaliies;
        Aoe = aoe;
    }

    public void Use(Character subject, List<Character> targets)
    {
        foreach (var target in targets)
        {
            int damageDealt;
            if (UseOnAllies)
            {
                damageDealt = Damage == 0 ? 0 : Convert.ToInt32(subject.Hp * 0.25 * Damage);
                target.Heal(damageDealt);
                Console.WriteLine(damageDealt != 0 ? $"{subject.Name} using {Name} healed {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}" : $"{subject.Name} using {Name} to {target.Name}");
                Thread.Sleep(3000);
                foreach (var i in StatusList)
                {
                    i.ApplyStatus(target, i);
                    Thread.Sleep(3000);
                    i.ApplyStatus(target, i);
                }
            }
            else
            {
                Console.WriteLine($"{subject.Name} used {Name} on {target.Name}");
                Thread.Sleep(3000);
                if (Misc.Roll(target.Dodge - subject.Acc))
                {
                    Console.WriteLine($"{target.Name} dodges");
                }
                else
                {
                    damageDealt = Convert.ToInt32(subject.Dmg * Damage * new Random().Next(75, 125) / 100);
                    if (Misc.Roll(subject.Crit))
                    {
                        damageDealt *= 2;
                        Console.WriteLine("Critical Strike!");
                    }
                    target.TakeDamage(damageDealt);
                    Console.WriteLine(damageDealt != 0 ? $"{subject.Name} dealt {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}" : $"{subject.Name} used {Name} on {target.Name}\n{target.Name}");
                    Thread.Sleep(3000);
                    foreach (var i in StatusList)
                    {
                        Console.Write($"{subject.Name} {i.OnApply}");
                        Thread.Sleep(3000);
                        i.ApplyStatus(target, i);
                    }
                }
            }
        }
    }

    public List<Character> GetTargets(Character subject)
    {
        var allies = Program.Game.Allies;
        var enemies = Program.Game.Enemies;
        List<Character> targetTeam;
        if (subject.IsAi) 
        {
            targetTeam = UseOnAllies ? enemies : allies;
            if (Aoe) { return targetTeam.Where(x => Targets.Contains(targetTeam.IndexOf(x))).ToList(); }
            return new List<Character> {targetTeam[new Random().Next(0, targetTeam.Count)]};
        }
        Thread.Sleep(3000);
        targetTeam = UseOnAllies ? allies : enemies;
        targetTeam = targetTeam.Where(x => Targets.Contains(targetTeam.IndexOf(x))).ToList();
        if (Aoe) { return targetTeam; }
        Console.WriteLine($"Select a target:\n{Misc.GetCharsNames(targetTeam)}");
        return new List<Character> {targetTeam[Misc.VerfiedInput(targetTeam.Count)]};
    }

    public static string GetNames(Character subj)
    {
        return Enumerable.Range(0, subj.Skills.Count).Aggregate("", (current, i) => current + $"{i + 1}: {subj.Skills[i].Name} ");
        
    }
    
    public string GetStatuses()
    {
        return StatusList.Aggregate("", (current, i) => current + (i.Name + ", "));
    }

    public static string GetInfo(List<Skill> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"\n{i + 1}: {ls[i].Name}" +
                                                                    (ls[i].Damage != 0 ? $"\n{(ls[i].UseOnAllies ? "Heal" : "Damage")}: {ls[i].Damage * 100}%" : "") + $"\nStatus: {ls[i].GetStatuses()}\n");
    }
}