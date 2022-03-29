using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Cosoleapp3;

public class Ai
{
    private Dictionary<string, List<Func<bool>>> _patternList = new();
    private List<Character> Allies;
    private List<Character> Enemies;
    private Character Subject;
    private static Skill skill;
    private static List<Character> target;

    public Ai(List<Character> allies, List<Character> enemies, Character subject)
    {
        Allies = allies;
        Enemies = enemies;
        Subject = subject;
    }

    private bool DealDamage()
    {
        Console.WriteLine("DealDamage");
        var skillList = Subject.Skills.Where(x => !x.UseOnAllies & x.Damage != 0).ToList();
        skill = skillList[new Random().Next(0, skillList.Count)];
        target = skill.Aoe
            ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList()
            : new List<Character>
                {Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
        skill.Use(Subject, target);
        return true;
    }

    private bool LastHit()
    {
        Console.WriteLine("Lasthit");
        var targetList = Allies.Where(x => x.Hp < x.MaxHp * 0.5).OrderBy(a => a.Hp).ToList();
        var skillList = Subject.Skills.Where(a => !a.UseOnAllies & a.Damage != 0).OrderByDescending(a => a.Damage).ToList();
        List<Skill> skillListT;
        if (!targetList.Any()) return false;
        {
            foreach (var i in targetList)
            {
                skillListT = skillList.Where(x => x.Targets.Contains(Allies.IndexOf(i))).ToList();
                if (!skillListT.Any()) continue;
                {
                    skill = skillList[0];
                    skillListT[0].Use(Subject, skill.Aoe ? Allies.Where(x => skillList[0].Targets.Contains(Allies.IndexOf(x))).ToList() : new List<Character>() {i});
                    return true;
                }
            }
            return false;
        }
    }

    private bool Control()
    {
        var skillList = Subject.Skills.Where(x =>
            x.StatusList.Any(a => a.Type == "stun") & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        List<Skill> skillListT;
        if (!skillList.Any()) return false;
        {
            foreach (var i in Allies
                         .Where(x => x.StatusList.All(a => a.Type != "stun"))
                         .OrderByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "stun")))
                         .ThenByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "guard")))
                         .ThenByDescending(x => x.Skills.Any(a => a.UseOnAllies & a.Damage != 0)).ToList())
            {
                skillListT = skillList.Where(x => x.Targets.Contains(Allies.IndexOf(i))).ToList();
                if (skillListT.Any())
                {
                    skill = skillListT[0];
                    skill.Use(Subject, skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : new List<Character> {i});
                    return true;
                }
            }
        }
        return false;
    }
    
    private bool Heal()
    {
        var skillList = Subject.Skills.Where(x => x.UseOnAllies & x.Damage != 0 & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).OrderBy(a => a.Damage).ToList();
        if (!skillList.Any() & !Enemies.Any(x => x.Hp < x.MaxHp * 0.5)) return false;
        {
            target = new List<Character> {Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
            skill = skillList.Where(x => skill.Targets.Contains(Allies.IndexOf(target[0]))).OrderBy(a => a.Damage).ToList()[0];
            skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
            return true;
        }
    }

    private bool Mark()
    {
        Console.WriteLine("Mark");
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "mark") & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (!(skillList.Any() & Enemies.Any(x => x.Skills.Any(a => a.MarkDamage)))) return false;
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (!Allies.All(x => x.StatusList.All(a => a.Type != "mark")))
                return false;
            {
                var targetList = Allies.Where(x =>
                    x.StatusList.All(a => a.Type != "mark") & skill.Targets.Contains(Allies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderBy(x => x.Hp).ToList()[0]};
                target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }
        
    }

    private bool TargetMark()
    {
        var skillList = Subject.Skills.Where(x => x.MarkDamage & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (!skillList.Any()) return false;
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (!Allies.Any(x => x.StatusList.Any(a => a.Type == "mark") & skill.Targets.Contains(Allies.IndexOf(x))))
                return false;
            {
                var targetList = Allies.Where(x =>
                    x.StatusList.Any(a => a.Type == "mark") & skill.Targets.Contains(Allies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderBy(x => x.Hp).ToList()[0]};
                target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }
    }

    private bool Riposte()
    {
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "riposte")).ToList();
        if (!skillList.Any()) return false;
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (Subject.StatusList.Any(a => a.Type == "riposte")) return false;
            target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : new List<Character> {Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
            skill.Use(Subject, target);
            return true;
        }
    }

    private bool Move()
    {
        if (Subject.BestPositon.Contains(Enemies.IndexOf(Subject))) return false;
        {
            List<List<Character>> tempList = new();
            foreach (var move in new [] {-3, -2, -1, 1, 2 ,3 })
            {
                Console.WriteLine(move);
                var enemies  = Program.Game.Enemies;
                foreach (var _ in Enumerable.Range(1, move < 0 ? move * -1 : move ))
                {
                    if (enemies.IndexOf(Subject) == 3 & move > 0 ||
                        enemies.IndexOf(Subject) == 0 & move < 0) { break; }
                    (enemies[enemies.IndexOf(Subject)], enemies[enemies.IndexOf(Subject) + (move < 0 ? -1 : 1)]) = (enemies[enemies.IndexOf(Subject) + (move < 0 ? -1 : 1)], enemies[enemies.IndexOf(Subject)]);
                }
                tempList.Add(enemies);
                foreach (var i in enemies)
                {
                    Console.WriteLine(i.Name);
                }

                Console.WriteLine();
            }
            Program.Game.Enemies = tempList.OrderBy(x => Subject.BestPositon.Contains(x.IndexOf(Subject))).ToList()[0];
            Console.WriteLine($"{Subject.Name} moved to position {Program.Game.Enemies.IndexOf(Subject) + 1}");
            return true;
        }
    }

        // private bool Buff()
    // {
    //     Console.WriteLine("Buff");
    //     var skillListA = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "agressivebuff") & x.UseOnAllies)
    //         .ToList();
    //     var skillListD = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "defensivebuff") & x.UseOnAllies)
    //         .ToList();
    //     if (skillListA.Any())
    //     {
    //         skill = skillListA[new Random().Next(0, skillListA.Count)];
    //         if (Enemies.Any(x =>
    //                 x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" &
    //                 skill.Targets.Contains(Enemies.IndexOf(x))))
    //         {
    //             var targetList = Enemies.Where(x =>
    //                 x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" &
    //                 skill.Targets.Contains(Enemies.IndexOf(x))).ToList();
    //             target = new List<Character> {targetList.OrderByDescending(x => x.Dmg).ToList()[0]};
    //             target = skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
    //             skill.Use(Subject, target);
    //             return true;
    //         }
    //     }
    //
    //     if (!skillListD.Any()) return false;
    //     {
    //         skill = skillListD[new Random().Next(0, skillListD.Count)];
    //         if (!Enemies.Any(x =>
    //                 x.StatusList.Any(a => a.Type != "defensivebuff" & x.Role == "Tank") &
    //                 skill.Targets.Contains(Enemies.IndexOf(x)))) return false;
    //         {
    //             var targetList = Enemies.Where(x =>
    //                 x.StatusList.All(a => a.Type != "defensivebuff") & x.Role == "Tank" &
    //                 skill.Targets.Contains(Enemies.IndexOf(x))).ToList();
    //             target = new List<Character> {targetList.OrderByDescending(x => x.Hp).ToList()[0]};
    //             target = skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
    //             skill.Use(Subject, target);
    //             return true;
    //         }
    //     }
    //
    // }
    
    public void Act()
    {
        _patternList.Add("Skeleton Veteran", new List<Func<bool>>() {LastHit, Control, DealDamage});
        _patternList.Add("Skeleton Spearman", new List<Func<bool>>() {LastHit, Riposte, DealDamage});
        _patternList.Add("Skeleton Banner Lord", new List<Func<bool>>() {Mark, Heal, LastHit, DealDamage});
        _patternList.Add("Skeleton Crossbowman", new List<Func<bool>>() {TargetMark, LastHit, DealDamage});
        foreach (Func<bool> _ in _patternList[Subject.Name].Where(i => i()))
            break;
    }
}