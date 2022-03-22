namespace Cosoleapp3;

public static class Ai
{
    public static void Act(List<Character> allies, List<Character> enemies, Character subject)
    {
        Skill skill;
        List<Character> target;
        var def = new List<Func<bool>>() {Control, LastHit, Heal, DealDamage};
        foreach (var i in def) { if (i()) {break;} }

        bool DealDamage()
        {
            var skillList = subject.Skills.Where(x => !x.UseOnAllies & x.Damage != 0).ToList();
            skill = skillList[new Random().Next(0, skillList.Count)];
            target = skill.Aoe
                ? allies.Where(x => skill.Targets.Contains(allies.IndexOf(x))).ToList()
                : new List<Character> {allies.Where(x => skill.Targets.Contains(allies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
            skill.Use(subject, target);
            return true;
        }
        
        bool LastHit()
        {
            if (allies.Any(x => x.Hp < x.MaxHp * 0.5))
            {
                target = new List<Character> {allies.Where(x => x.Hp < x.MaxHp * 0.5).OrderBy(a => a.Hp).ToList()[0]};
                skill = subject.Skills.Where(x => !x.UseOnAllies & x.Targets.Contains(allies.IndexOf(target[0])) ).OrderByDescending(x => x.Damage).ToList()[0];
                target = skill.Aoe ? allies.Where(x => skill.Targets.Contains(allies.IndexOf(x))).ToList() : target;
                skill.Use(subject, target);
                return true;
            }

            return false;
        }

        bool Heal()
        {
            if (enemies.Any(x => x.Hp < x.MaxHp * 0.5 & subject.Skills.Any(a => a.UseOnAllies & a.Damage != 0)))
            {
                skill = subject.Skills.Where(x => x.UseOnAllies & x.Damage != 0).OrderBy(a => a.Damage).ToList()[0];
                target = skill.Aoe
                    ? enemies.Where(x => skill.Targets.Contains(enemies.IndexOf(x))).ToList()
                    : new List<Character> {enemies.Where(x => skill.Targets.Contains(enemies.IndexOf(x))).OrderBy(x => x.Hp).ToList()[0]};
                skill.Use(subject, target);
                return true;
            }

            return false;
        }
        
        bool Control()
        {
            var skillList = subject.Skills.Where(x => x.StatusList.Any(a => a.Type == "stun")).ToList();
            if (skillList.Any())
            {
                skill = skillList[new Random().Next(0, skillList.Count)];
                if (allies.Any(x => x.StatusList.All(a => a.Type != "stun") & skill.Targets.Contains(allies.IndexOf(x))))
                {
                    var targetList = allies.Where(x => x.StatusList.All(a => a.Type != "stun") & skill.Targets.Contains(allies.IndexOf(x))).ToList();
                    target = new List<Character> {targetList[new Random().Next(0, targetList.Count)]};
                    target = skill.Aoe ? allies.Where(x => skill.Targets.Contains(allies.IndexOf(x))).ToList() : target;
                    skill.Use(subject, target);
                    return true;
                }
            }

            return false;
        }
    }
    }

