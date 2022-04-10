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

    private void RandomAction()
    {
        var skillList = Subject.Skills.Where(x => x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        skill = skillList[new Random().Next(0, skillList.Count)];
        var targetTeam = skill.UseOnAllies ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList();
        skill.Use(Subject, skill.Aoe ? targetTeam : new List<Character> {targetTeam[new Random().Next(0, targetTeam.Count)]});
    }

    private bool DealDamage()
    {
        var skillList = Subject.Skills.Where(x => !x.UseOnAllies & x.Damage != 0 & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (!skillList.Any()) return false;
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            skill.Use(Subject, skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList()
                : new List<Character>
                    {Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).OrderBy(x => x.Hp * (1.0 - x.Armor) * (1.0 - (x.Dodge + Subject.Acc * 0.01))).ToList()[0]});
            return true;
        }

    }

    private bool TargetLowHp()
    {
        var targetList = Allies.Where(x => x.Hp < x.MaxHp * 0.5).OrderBy(a => a.Hp * (1.0 - a.Armor) * (1.0 - (a.Dodge + Subject.Acc * 0.01))).ToList();
        var skillList = Subject.Skills.Where(a => !a.UseOnAllies & a.Damage != 0 & a.UsableFrom.Contains(Enemies.IndexOf(Subject))).OrderByDescending(a => a.Damage).ToList();
        if (!targetList.Any()) return false;
        {
            foreach (var i in targetList)
            {
                var skillListT = skillList.Where(x => x.Targets.Contains(Allies.IndexOf(i))).ToList();
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
    
    private bool LastHit()
    {
        foreach (var i in Allies)
        {
            foreach (var j in Subject.Skills.Where(j => (i.Hp * (1.0 - i.Armor)) - (Subject.Dmg * j.Damage) < 0 & j.UsableFrom.Contains(Enemies.IndexOf(Subject)) & j.Targets.Contains(Allies.IndexOf(i))).OrderByDescending(x => x.Damage))
            {
                j.Use(Subject, new List<Character>() {i});
                return true;
            }
        }
        return false;
    }

    private bool Control()
    {
        var skillList = Subject.Skills.Where(x =>
            x.StatusList.Any(a => a.Type == "stun") & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (!skillList.Any()) return false;
        {
            foreach (var i in Allies
                         .Where(x => x.StatusList.All(a => a.Type != "stun"))
                         .OrderByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "stun")))
                         .ThenByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "guard")))
                         .ThenByDescending(x => x.Skills.Any(a => a.UseOnAllies & a.Damage != 0)).ToList())
            {
                var skillListT = skillList.Where(x => x.Targets.Contains(Allies.IndexOf(i))).ToList();
                if (!skillListT.Any()) continue;
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
            if (skillList.Any() & Enemies.Any(x => x.Hp < x.MaxHp * 0.5))
            {
                skill = skillList.OrderBy(a => a.Damage).ToList()[0];
                target = new List<Character> {Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
                skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
                return true;
            }

            return false;
    }
    
    private bool Guard()
    {
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "guard") & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (skillList.Any() & Enemies.Any(x => x.Hp < x.MaxHp * 0.5))
        {
            skill = skillList.OrderBy(a => a.Damage).ToList()[0];
            target = new List<Character> {Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
            skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
            return true;
        }
        return false;
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
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "riposte") & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        if (!skillList.Any() & Subject.StatusList.All(x => x.Type != "riposte")) return false;
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
            foreach (int move in new [] {-3, -2, -1, 1, 2 ,3 })
            {
                var enemies  = Program.Game.Enemies;
                foreach (int _ in Enumerable.Range(1, move < 0 ? move * -1 : move ))
                {
                    if (enemies.IndexOf(Subject) == 3 & move > 0 ||
                        enemies.IndexOf(Subject) == 0 & move < 0) { break; }
                    (enemies[enemies.IndexOf(Subject)], enemies[enemies.IndexOf(Subject) + (move < 0 ? -1 : 1)]) = (enemies[enemies.IndexOf(Subject) + (move < 0 ? -1 : 1)], enemies[enemies.IndexOf(Subject)]);
                }
                tempList.Add(enemies);
            }
            foreach (var i in tempList)
            {
                foreach (var j in i)
                {
                    Console.WriteLine(j.Name);
                }
                Console.WriteLine();
            }
            Console.WriteLine(tempList.Count);
            Program.Game.Enemies = tempList[0];
            Console.WriteLine($"{Subject.Name} moved to position {Program.Game.Enemies.IndexOf(Subject) + 1}");
            Console.ReadKey();
            return true;
        }
    }

    private bool Buff()
    {
        var skillList = Subject.Skills.Where(x => (x.StatusList.Any(a => a.Type == "abuff") || x.StatusList.Any(a => a.Type == "dbuff")) & x.UsableFrom.Contains(Enemies.IndexOf(Subject)) & x.UseOnAllies).ToList();
        if (!skillList.Any()) return false;
        {
            foreach (var i in skillList)
            {
                skill = i;
                var targetList = Enemies.Where(x => x.StatusList.All(a => i.StatusList.Any(b => b.Type != a.Type)) & skill.Targets.Contains(Enemies.IndexOf(x))).ToList();
                targetList = i.StatusList.Any(x => x.Type == "abuff")
                    ? targetList.OrderByDescending(x => x.Skills.Any(a => a.MarkDamage)).ThenByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "riposte"))).ThenByDescending(x => x.Role == "D").ThenByDescending(x => x.Role == "T").ToList()
                    : targetList.OrderByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "guard"))).ThenByDescending(x => x.Role == "D").ThenByDescending(x => x.Role == "T").ToList();
                if (targetList.Any())
                {
                    skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : new List<Character> {targetList[0]});
                } 
                return true;
            }

            return false;
        }
    }
    
    private bool DeBuff()
    {
        var skillList = Subject.Skills.Where(x => (x.StatusList.Any(a => a.Type == "adbuff") || x.StatusList.Any(a => a.Type == "ddbuff")) & x.UsableFrom.Contains(Enemies.IndexOf(Subject))).ToList();
        Console.WriteLine(skillList.Any());
        if (!skillList.Any()) return false;
        {
            foreach (var i in skillList)
            {
                skill = i;
                var targetList = Allies.Where(x => x.StatusList.All(a => i.StatusList.Any(b => b.Type != a.Type)) & skill.Targets.Contains(Allies.IndexOf(x))).OrderByDescending(x => i.StatusList[0].StatAffected(x)).ToList();
                if (targetList.Any())
                {
                    skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : new List<Character> {targetList[0]});
                } 
                return true;
            }

            return false;
        }
    }
    
    // private bool Save()
    // {
    //     var saveTargets = Enemies.Where(x => Allies.Any(a => a.Skills.Any(b => x.Hp <= a.Dmg * b.Damage & b.UsableFrom.Contains(Enemies.IndexOf(a)) & b.Targets.Contains(Enemies.IndexOf(x)) & !b.UseOnAllies))).OrderByDescending(x => x.Skills.Any(a => a.StatusList.Any(b => b.Type == "guard"))).ThenByDescending(a => a.Skills.Any(x => x.Damage != 0 & x.UseOnAllies)).ThenByDescending(x => x.Role == "D").ThenByDescending(x => x.Role == "T").ToList();
    //     Console.WriteLine(saveTargets.Any());
    //
    //     bool Guard()
    //     {
    //         var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "guard") & x.UsableFrom.Contains(Enemies.IndexOf(Subject)) & x.UseOnAllies).ToList();
    //         if (!skillList.Any()) return false;
    //         skill.Use(Subject, target);
    //         return true;
    //     }
    //
    //     bool HealS()
    //     {
    //         var skillList = Subject.Skills.Where(x => x.Damage != 0 & x.UsableFrom.Contains(Enemies.IndexOf(Subject)) & x.UseOnAllies).ToList();
    //         if (!skillList.Any()) return false;
    //         skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
    //         return true;
    //     }
    //     
    //     bool BuffS()
    //     {
    //         var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "dbuff") & x.UseOnAllies).ToList();
    //         if (!skillList.Any()) return false;
    //         skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
    //         return true;
    //     }
    //
    //     bool Stun()
    //     {
    //         var targetList = Allies.Where(x => Enemies.Any(a => x.Skills.Any(b =>
    //             a.GetEhp() - x.Dmg * b.Damage >= 0 & b.UsableFrom.Contains(Enemies.IndexOf(a)) &
    //             b.Targets.Contains(Enemies.IndexOf(a)) & !b.UseOnAllies))).ToList();
    //         targetList = targetList.Where(x => Game.TurnOrder.Contains(x))
    //             .OrderByDescending(x => Game.TurnOrder.IndexOf(x) - Game.TurnOrder.IndexOf(saveTargets[0]))
    //             .Union(targetList.Where(x => !Game.TurnOrder.Contains(x)).OrderByDescending(x => x.Initiative))
    //             .ToList();
    //         target = new List<Character>() {targetList[0]};
    //
    //     var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "stun") & x.UsableFrom.Contains(Enemies.IndexOf(Subject)) & x.Targets.Contains(Enemies.IndexOf(target[0]))).ToList();
    //         if (!skillList.Any()) return false;
    //         skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
    //         return true;
    //     }
    //     
    //     bool Kill()
    //     {
    //         var targetList = Allies.Where(x => Enemies.Any(a => x.Skills.Any(b => a.GetEhp() - x.Dmg * b.Damage >= 0 & b.UsableFrom.Contains(Enemies.IndexOf(a)) & b.Targets.Contains(Enemies.IndexOf(a)) & !b.UseOnAllies))).ToList();
    //         targetList = targetList.Where(x => Game.TurnOrder.Contains(x)).OrderByDescending(x => Game.TurnOrder.IndexOf(x) - Game.TurnOrder.IndexOf(saveTargets[0])).Union(targetList.Where(x => !Game.TurnOrder.Contains(x)).OrderByDescending(x => x.Initiative)).ToList();
    //         foreach (var i in targetList)
    //         {
    //             var skillList = Subject.Skills.Where(x => target[0].GetEhp() - Subject.Dmg * x.Damage <= 0 & x.UsableFrom.Contains(Enemies.IndexOf(Subject)) & x.Targets.Contains(Enemies.IndexOf(i))).ToList();
    //             if (!skillList.Any()) continue;
    //             {
    //                 skill.Use(Subject, skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList() : target);
    //                 return true;
    //             }
    //         }
    //
    //         return false;
    //     }
    //     
    //     Console.WriteLine("save");
    //     foreach (var i in saveTargets)
    //     {
    //         target = new List<Character>() {i};
    //         foreach (var j in new List<Func<bool>> {Kill, Guard, Stun, HealS, BuffS}.Where(j => j()))
    //         { 
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    public void Act()
    {
        _patternList.Add("Skeleton Veteran", new List<Func<bool>> {Guard, LastHit, TargetLowHp, Control, DealDamage});
        _patternList.Add("Skeleton Spearman", new List<Func<bool>> {LastHit, TargetLowHp, Riposte, DealDamage});
        _patternList.Add("Skeleton Banner Lord", new List<Func<bool>> {DeBuff, LastHit, Heal, Mark, Buff, TargetLowHp, DealDamage});
        _patternList.Add("Skeleton Crossbowman", new List<Func<bool>> {LastHit, TargetMark, TargetLowHp, DealDamage});
        if (Misc.Roll(Program.Difficulty))
        {
            foreach (Func<bool> _ in _patternList[Subject.Name].Where(i => i()))
                break;
        }
        else
        {
            RandomAction();
        }
    }
}