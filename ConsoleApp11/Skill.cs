using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Schema;

namespace Cosoleapp3;

public class Skill
{
    public string Name;
    public Double Damage;
    public bool UseOnAllies;
    public bool UseOnSelf;
    public bool IsMoveSkill;
    public bool Aoe;
    public bool MarkDamage;
    public bool BuffSelf;
    public int Move;
    public int MoveSelf;
    public List<int> Targets = new() {0, 1, 2, 3};
    public List<int> UsableFrom = new() {0, 1, 2, 3};
    public List<Status> StatusList = new();

    public Skill(string name, double damage = 0, List<int>? targets = null, List<Status>? statusList = null,
        List<int>? usablefrom = null, bool useonaliies = false, bool useonself = false, bool aoe = false, bool markdamage = false,
        bool buffself = false, bool isMoveSkill = false, int move = 0, int moveself = 0)
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
        UseOnSelf = useonself;
        MarkDamage = markdamage;
        BuffSelf = buffself;
        Aoe = aoe;
        Move = move;
        MoveSelf = moveself;
        IsMoveSkill = isMoveSkill;
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
                    ? $"{subject.Name} using {Name} healed {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}/{target.MaxHp}"
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

            if (UseOnSelf)
            {
                if (IsMoveSkill)
                {
                    Console.WriteLine($"What position to move: 1 - {Program.Game.Allies.Count}");
                    Move = Misc.VerfiedInput(Program.Game.Allies.Count);
                    ref var allies = ref Program.Game.Allies;
                    foreach (int i in Enumerable.Range(1, Move < 0 ? Move * -1 : Move ))
                    {
                        if (allies.IndexOf(target) == 3 & Move > 0 ||
                            allies.IndexOf(target) == 0 & Move < 0) { break; }
                        (allies[allies.IndexOf(target)], allies[allies.IndexOf(target) + (Move < 0 ? -1 : 1)]) = (allies[allies.IndexOf(target) + (Move < 0 ? -1 : 1)], allies[allies.IndexOf(target)]);
                    }
                    
                }
            }

            if (!(!UseOnAllies & !UseOnSelf)) continue;
            {
                Console.WriteLine($"{subject.Name} used {Name} on {target.Name}");
                Thread.Sleep(3000);

                if (Misc.Roll(Convert.ToInt32(target.Dodge * 100 - subject.Acc)))
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
                    Console.Write(damageDealt != 0 ? $"{subject.Name} dealt {damageDealt} to {target.Name}\n{target.Name} Hp: {target.Hp}/{target.MaxHp}\n" : "");
                    Thread.Sleep(3000);
                    if (target.StatusList.Any(x => x.Type == "riposte") & Name != "riposte attack")
                    {
                        new Skill("riposte attack", 0.67).Use(target, new List<Character>() {subject});
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
                    
                    if (MoveSelf == 0) continue;
                    {
                        Console.WriteLine($"{subject.Name} is " + (MoveSelf > 0 ? "Moves back " : "Moves forward ") + "by " + (MoveSelf > 0 ? $"{MoveSelf}" : $"{MoveSelf * -1}"));
                        ref var allies = ref Program.Game.Allies;
                        ref var enemies = ref Program.Game.Enemies;
                        var targetTeam = Program.Game.Allies.Contains(subject) ? allies : enemies;
                        foreach (int i in Enumerable.Range(1, MoveSelf < 0 ? MoveSelf * -1 : MoveSelf ))
                        {
                            if (targetTeam.IndexOf(subject) == 3 & Move > 0 ||
                                targetTeam.IndexOf(subject) == 0 & Move < 0) { break; }
                            (targetTeam[targetTeam.IndexOf(subject)], targetTeam[targetTeam.IndexOf(subject) + (Move < 0 ? -1 : 1)]) = (targetTeam[targetTeam.IndexOf(subject) + (Move < 0 ? -1 : 1)], targetTeam[targetTeam.IndexOf(subject)]);
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
            return targetTeam;

        if (IsMoveSkill)
            return new List<Character> {Game.Subject};

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