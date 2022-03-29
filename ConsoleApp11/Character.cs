using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Cosoleapp3;

public class Character
{
    public string Name;
    public int Hp;
    public int MaxHp;
    public int Dmg;
    public int MaxDmg;
    public int Acc;
    public int MaxAcc;
    public int Dodge;
    public int MaxDodge;
    public int Crit;
    public int MaxCrit;
    public double Armor;
    public double MaxArmor;
    public int Initiative;
    public int MaxInitiative;
    public bool Stunned = false;
    public bool Dead = false;
    public bool IsAi;
    public string Role;
    public List<int> BestPositon;
    public List<Skill> Skills;
    public List<Status> StatusList = new();

    public Character(string name, int hp, int dmg, int acc, int dodge, double armor, int crit, int initiative, List<Skill> skills, string role = "", List<Func<bool>> pattern = null)
    {
        Hp = hp;
        MaxHp = hp;
        Dmg = dmg;
        MaxDmg = dmg;
        Acc = acc;
        MaxAcc = acc;
        Dodge = dodge;
        MaxDodge = dodge;
        Initiative = initiative;
        MaxInitiative = initiative;
        Crit = crit;
        MaxCrit = crit;
        Armor = armor;
        MaxArmor = armor;
        Skills = skills;
        Name = name;
        Role = role;
        BestPositon = Enumerable.Range(0, 3).Where(x => Skills.All(a => a.UsableFrom.Contains(x))).ToList();
    }
    public Skill GetSkill()
    {
        var usableSkills = Skills.Where(x => x.UsableFrom.Contains(Program.Game.Allies.IndexOf(this))).ToList();
        if (IsAi) return usableSkills[new Random().Next(Skills.Count)];
        Console.WriteLine($"Select a skill:\n{Skill.GetNames(usableSkills)}");
        return usableSkills[Misc.VerfiedInput(usableSkills.Count)];
    }
    
    public void ProcessStatuses()
    {
        List<Status> temp = new List<Status>(StatusList);
        foreach (var i in StatusList)
        {
            if (!i.IsInstant)
            {
                i.Fn(this);
                Console.WriteLine($"Turns remaining: {i.Duration - 1}");
            }
            i.Duration -= 1;
            Thread.Sleep(3000);
            if (i.Duration <= 0) { temp.Remove(i); }
        }
        StatusList = temp;
        Dmg = MaxDmg;
        Acc = MaxAcc;
        Dodge = MaxDodge;
        Initiative = MaxInitiative;
        Crit = MaxCrit;
        Armor = MaxArmor;
        foreach (var i in StatusList.Where(i => i.IsInstant))
        {
            i.Fn(this);
        } 
    }

    public void TakeDamage(int dmg)
    {
        Hp = Hp - dmg <= 0 ? 0 : Hp - dmg;
        Dead = Hp == 0;
        if (!Dead) return;
        Console.WriteLine($"{Name} is dead");
        Thread.Sleep(3000);
    }
    
    public void Heal(int dmg)
    {
        Hp = Hp + dmg > MaxHp ? MaxHp : Hp + dmg;
        Dead = Hp == 0;
        if (!Dead) return;
        Console.WriteLine($"{Name} is dead");
        Thread.Sleep(3000);
    }
    
    public string GetStatuses()
    {
        return StatusList.Aggregate("", (current, i) => current + (i.Name + " ")).Trim();
    }
}