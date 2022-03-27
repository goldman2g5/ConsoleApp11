using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Schema;

namespace Cosoleapp3;

public class Skill
{
    public string Name;
    public Double Damage;
    public bool UseOnAllies;
    public bool Aoe;
    public bool MarkDamage;
    public bool BuffSelf;
    public int Move;
    public List<int> Targets = new() {0, 1, 2, 3};
    public List<int> UsableFrom = new() {0, 1, 2, 3};
    public List<Status> StatusList = new();

    public Skill(string name, double damage = 0, List<int>? targets = null, List<Status>? statusList = null,
        List<int>? usablefrom = null, bool useonaliies = false, bool aoe = false, bool markdamage = false,
        bool buffself = false, int move = 0)
    {
        Damage = damage;
        Name = name;
        if (targets != null)
            Targets = targets;
        if (statusList != null)
            StatusList = statusList;
        if (usablefrom != null)
            UsableFrom = usablefrom;
        UseOnAllies = useonaliies;
        MarkDamage = markdamage;
        BuffSelf = buffself;
        Aoe = aoe;
        Move = move;
    }

    public void Use(Character subject, List<Character> targets)
    {
        foreach (var t in targets)
        {
            var target = t;
            int damageDealt;
            if (UseOnAllies)
            {
                damageDealt = Damage == 0 ? 0 : Convert.ToInt32(subject.Hp * 0.25 * Damage);
                target.Heal(damageDealt);
                Console.WriteLine(damageDealt != 0
                    ? $"{subject.Name} using {Name} healed {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}"
                    : $"{subject.Name} using {Name} to {target.Name}");
                Thread.Sleep(3000);
                foreach (var i in StatusList)
                {
                    if (BuffSelf)
                    {
                        Status.ApplyStatus(subject, i);
                        Console.WriteLine($"{subject.Name} {i.OnApply}");
                        continue;
                    }

                    Status.ApplyStatus(target, i);
                    Console.WriteLine($"{target.Name} {i.OnApply}");
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
                    if (target.StatusList.Any(x => x.Type == "guard"))
                    {
                        Console.WriteLine("guard");
                        target = (Program.Game.Allies.Contains(target) ? Program.Game.Allies : Program.Game.Enemies)
                            .Find(x =>
                                x.Skills.Any(a => a.StatusList.Any(b => b.Type == "guard")));
                    }

                    damageDealt = Convert.ToInt32(subject.Dmg * Damage * (1.0 - target.Armor));

                    if (MarkDamage & target.StatusList.Any(x => x.Type == "mark"))
                        damageDealt *= 2;

                    if (Misc.Roll(subject.Crit))
                    {
                        damageDealt *= 2;
                        Console.WriteLine("Critical Strike!");
                    }

                    target.TakeDamage(damageDealt);
                    Console.WriteLine(damageDealt != 0 ? $"{subject.Name} dealt {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}" : "");
                    Thread.Sleep(3000);
                    if (target.StatusList.Any(x => x.Type == "riposte"))
                    {
                        new Skill("riposte attack", 1, new List<int> {0, 1, 2, 3}, new List<Status>()).Use(target,
                            new List<Character>() {subject});
                    }

                    foreach (var i in StatusList)
                    {
                        if (BuffSelf)
                        {
                            Status.ApplyStatus(subject, i);
                            Console.WriteLine($"{subject.Name} {i.OnApply}");
                            continue;
                        }

                        Status.ApplyStatus(target, i);
                        Console.WriteLine($"{target.Name} {i.OnApply}");
                    }

                    if (Move == 0) continue;
                    {
                        Console.WriteLine($"{target.Name} is " + (Move > 0 ? "Pushed back " : "Pulled in ") + "by " + (Move > 0 ? $"{Move}" : $"{Move * -1}"));
                        ref var allies = ref Program.Game.Allies;
                        ref var enemies = ref Program.Game.Enemies;
                        var targetTeam = Program.Game.Allies.Contains(target) ? allies : enemies;
                        foreach (int i in Enumerable.Range(1, Move < 0 ? Move * -1 : Move ))
                        {
                            if (targetTeam.IndexOf(target) == 3 & Move > 0 ||
                                targetTeam.IndexOf(target) == 0 & Move < 0) { break; }
                            (targetTeam[targetTeam.IndexOf(target)], targetTeam[targetTeam.IndexOf(target) + (Move < 0 ? -1 : 1)]) = (targetTeam[targetTeam.IndexOf(target) + (Move < 0 ? -1 : 1)], targetTeam[targetTeam.IndexOf(target)]);
                        }
                    }
                    
                    
                }
            }
        }
    }

    public List<Character> GetTargets()
    {
        var allies = Program.Game.Allies;
        var enemies = Program.Game.Enemies;
        List<Character> targetTeam;
        Thread.Sleep(3000);
        targetTeam = UseOnAllies ? allies : enemies;
        targetTeam = targetTeam.Where(x => Targets.Contains(targetTeam.IndexOf(x))).ToList();
        if (Aoe)
        {
            return targetTeam;
        }

        Console.WriteLine($"Select a target:\n{Misc.GetCharsNames(targetTeam)}");
        return new List<Character> {targetTeam[Misc.VerfiedInput(targetTeam.Count)]};
    }

    public static string GetNames(List<Skill> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"{i + 1}: {ls[i].Name} ");
    }

    public string GetStatuses()
    {
        return StatusList.Aggregate("", (current, i) => current + (i.Name + ", "));
    }

    public static string GetInfo(List<Skill> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"\n{i + 1}: {ls[i].Name}" +
                                                                           (ls[i].Damage != 0
                                                                               ? $"\n{(ls[i].UseOnAllies ? "Heal" : "Damage")}: {ls[i].Damage * 100}%"
                                                                               : "") +
                                                                           $"\nTargets{(ls[i].UseOnAllies ? " Allies" : "")}: {(ls[i].Aoe ? "~" : "")}{ls[i].Targets.Aggregate("", (x, j) => x + $"{j + 1} ").Trim()}" +
                                                                           $"{(ls[i].StatusList.Any() ? $"\nStatus: {ls[i].GetStatuses()}" : "")}\n");
    }
}