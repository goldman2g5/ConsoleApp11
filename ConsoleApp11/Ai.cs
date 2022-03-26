using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Cosoleapp3;

public class Ai
{
    private Dictionary<string, List<Func<bool>>> _patternList = new Dictionary<string, List<Func<bool>>>();
    public List<Character> Allies;
    public List<Character> Enemies;
    public Character Subject;
    public static Skill skill;
    public static List<Character> target;

    public Ai(List<Character> allies, List<Character> enemies, Character subject)
    {
        Allies = allies;
        Enemies = enemies;
        Subject = subject;
    }

    public List<Func<bool>> GetPattern(string name)
    {
        return _patternList[name];
    }

    private bool DealDamage()
    {
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
        if (Allies.Any(x => x.Hp < x.MaxHp * 0.5))
        {
            target = new List<Character> {Allies.Where(x => x.Hp < x.MaxHp * 0.5).OrderBy(a => a.Hp).ToList()[0]};
            skill = Subject.Skills.Where(x => !x.UseOnAllies & x.Targets.Contains(Allies.IndexOf(target[0])))
                .OrderByDescending(x => x.Damage).ToList()[0];
            target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
            skill.Use(Subject, target);
            return true;
        }

        return false;
    }

    private bool Heal()
    {
        if (Enemies.Any(x => x.Hp < x.MaxHp * 0.5 & Subject.Skills.Any(a => a.UseOnAllies & a.Damage != 0)))
        {
            skill = Subject.Skills.Where(x => x.UseOnAllies & x.Damage != 0).OrderBy(a => a.Damage).ToList()[0];
            target = skill.Aoe
                ? Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).ToList()
                : new List<Character>
                    {Enemies.Where(x => skill.Targets.Contains(Enemies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
            skill.Use(Subject, target);
            return true;
        }

        return false;
    }

    private bool Control()
    {
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "stun")).ToList();
        if (skillList.Any())
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (Allies.Any(x => x.StatusList.All(a => a.Type != "stun") & skill.Targets.Contains(Allies.IndexOf(x))))
            {
                var targetList = Allies.Where(x =>
                    x.StatusList.All(a => a.Type != "stun") & skill.Targets.Contains(Allies.IndexOf(x))).ToList();
                target = new List<Character> {targetList[new Random().Next(0, targetList.Count)]};
                target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }

        return false;
    }

    private bool Mark()
    {
        var skillList = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "mark")).ToList();
        if (skillList.Any() & Enemies.Any(x => x.Skills.Any(a => a.MarkDamage)))
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (Allies.All(x => x.StatusList.All(a => a.Type != "mark") & skill.Targets.Contains(Allies.IndexOf(x))))
            {
                var targetList = Allies.Where(x =>
                    x.StatusList.All(a => a.Type != "mark") & skill.Targets.Contains(Allies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderBy(x => x.Hp).ToList()[0]};
                target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }

        return false;
    }

    private bool TargetMark()
    {
        var skillList = Subject.Skills.Where(x => x.MarkDamage).ToList();
        if (skillList.Any())
        {
            skill = skillList[new Random().Next(0, skillList.Count)];
            if (Allies.Any(x => x.StatusList.Any(a => a.Type == "mark") & skill.Targets.Contains(Allies.IndexOf(x))))
            {
                var targetList = Allies.Where(x =>
                    x.StatusList.Any(a => a.Type == "mark") & skill.Targets.Contains(Allies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderBy(x => x.Hp).ToList()[0]};
                target = skill.Aoe ? Allies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }

        return false;
    }

    private bool Buff()
    {
        var skillListA = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "agressivebuff") & x.UseOnAllies)
            .ToList();
        var skillListD = Subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "defensivebuff") & x.UseOnAllies)
            .ToList();
        if (skillListA.Any())
        {
            skill = skillListA[new Random().Next(0, skillListA.Count)];
            if (Enemies.Any(x =>
                    x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" &
                    skill.Targets.Contains(Enemies.IndexOf(x))))
            {
                var targetList = Enemies.Where(x =>
                    x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" &
                    skill.Targets.Contains(Enemies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderByDescending(x => x.Dmg).ToList()[0]};
                target = skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }

        if (skillListD.Any())
        {
            skill = skillListD[new Random().Next(0, skillListD.Count)];
            if (Enemies.Any(x =>
                    x.StatusList.Any(a => a.Type != "defensivebuff" & x.Role == "Tank") &
                    skill.Targets.Contains(Enemies.IndexOf(x))))
            {
                var targetList = Enemies.Where(x =>
                    x.StatusList.All(a => a.Type != "defensivebuff") & x.Role == "Tank" &
                    skill.Targets.Contains(Enemies.IndexOf(x))).ToList();
                target = new List<Character> {targetList.OrderByDescending(x => x.Hp).ToList()[0]};
                target = skill.Aoe ? Enemies.Where(x => skill.Targets.Contains(Allies.IndexOf(x))).ToList() : target;
                skill.Use(Subject, target);
                return true;
            }
        }

        return false;
    }

    // bool Buff()
    // {
    //     var skillListA = subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "agressivedebuff" & x.StatusList.Any(a => a.Type == "defensivedebuff") & x.UseOnAllies)).ToList();
    //     if (skillListA.Any())
    //     {
    //         skill = skillListA[new Random().Next(0, skillListA.Count)];
    //         if (enemies.Any(x => x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" & skill.Targets.Contains(enemies.IndexOf(x))))
    //         {
    //             var targetList = enemies.Where(x => x.StatusList.All(a => a.Type != "agressivebuff") & x.Role == "DD" & skill.Targets.Contains(enemies.IndexOf(x))).ToList();
    //             target = new List<Character> {targetList.OrderByDescending(x => x.Dmg).ToList()[0]};
    //             target = skill.Aoe ? enemies.Where(x => skill.Targets.Contains(allies.IndexOf(x))).ToList() : target;
    //             skill.Use(subject, target);
    //             return true;
    //         }
    //     }
    //
    //     return false;
    // 
    
    public void Act()
    {
        _patternList.Add("Skeleton Veteran", new List<Func<bool>>() {LastHit, Control, DealDamage});
        _patternList.Add("Skeleton Spearman", new List<Func<bool>>() {LastHit, DealDamage});
        _patternList.Add("Skeleton Banner Lord", new List<Func<bool>>() {Mark, Heal, LastHit, Buff, DealDamage});
        _patternList.Add("Skeleton Crossbowman", new List<Func<bool>>() {TargetMark, LastHit, DealDamage});
        foreach (Func<bool> i in _patternList[Subject.Name].Where(i => i()))
        {
            break;
        }
    }
}