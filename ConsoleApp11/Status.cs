using System.Threading.Channels;

namespace Cosoleapp3;

public class Status
{
    public string Name;
    public Action<Character> Fn;
    public int Duration;
    public bool IsInstant;
    public string Type;
    public string OnApply;
    private static Dictionary<string, Status> StatusList = new Dictionary<string, Status>();

    public Status(string name, int duration, Action<Character> fn, string type, string onapply, bool isInstant = false)
    {
        Name = name;
        Fn = fn;
        Duration = duration;
        Type = type;
        IsInstant = isInstant;
        
        OnApply = onapply;
    }

    public static void GenerateStatuses()
    {
        void Stun(Character obj)
        {
            obj.Stunned = true;
            Console.WriteLine($"{obj.Name} is stunned, skipping turn");
            Thread.Sleep(3000);
        }
        
        void Bleed(Character obj)
        {
            int damage = Convert.ToInt32(obj.MaxHp * 0.15);
            obj.TakeDamage(damage);
            Console.WriteLine($"{obj.Name} bleeding for {damage}");
            Thread.Sleep(3000);
        }
        
        void Mark(Character obj)
        {
            Console.WriteLine($"{obj.Name} is marked");
        }

        void ArmorBuff(Character obj)
        {
            obj.Armor += 20;
        }
        
        void DodgeBuff(Character obj)
        {
            obj.Dodge += 25;
        }
        
        void AccBuff(Character obj)
        {
            obj.Acc += 10;
        }
        
        void DamageBuff(Character obj)
        {
            obj.Dmg += obj.Dmg / 100 * 20;
        }
        
        void CritBuff(Character obj)
        {
            obj.Crit += 20;
        }
        
        void InitBuff(Character obj)
        {
            obj.Initiative += 10;
        }
        
        void ArmorDeBuff(Character obj)
        {
            obj.Armor -= 20;
        }
        
        void DodgeDeBuff(Character obj)
        {
            obj.Armor -= 25;
        }
        
        void AccDeBuff(Character obj)
        {
            obj.Armor -= 25;
        }
        
        void DamageDeBuff(Character obj)
        {
            obj.Armor -= 20;
        }
        
        void CritDeBuff(Character obj)
        {
            obj.Crit -= 25;
        }
        
        void InitDeBuff(Character obj)
        {
            obj.Initiative -= 20;
        }

        AddStatus("stun", 1, Stun, "stun", "is stunned");
        AddStatus("bleed", 2, Bleed, "damage", "is bleeding");
        AddStatus("Mark", 2, Mark, "mark", "is marked");
        AddStatus("Armor buff", 3, ArmorBuff, "defensivebuff", "is fortified", true);
        AddStatus("Dodge buff", 3, DodgeBuff, "defensivebuff", "is evading", true);
        AddStatus("Acc buff", 3, AccBuff, "agressivebuff", "is more accurate", true);
        AddStatus("Damage buff", 3, DamageBuff, "agressivebuff", "is empowerd", true);
        AddStatus("Crit buff", 3, CritBuff, "agressivebuff", "is crit buffed", true);
        AddStatus("Init buff", 3, InitBuff, "agressivebuff", "is hasted", true);
        AddStatus("Armor debuff", 3, ArmorDeBuff, "agressivedebuff", "gets armor reduced", true);
        AddStatus("Dodge debuff", 3, DodgeDeBuff, "agressivedebuff", "is crippled", true);
        AddStatus("Acc debuff", 3, AccDeBuff, "defensivedebuff", "is blinded", true);
        AddStatus("Damage debuff", 3, DamageDeBuff, "defensivedebuff", "is weakend", true);
        AddStatus("Crit debuff", 3, CritDeBuff, "defensivedebuff", "is unlucky", true);
        AddStatus("Init debuff", 3, InitDeBuff, "defensivedebuff", "is slowed ", true);


    }
    
    public void ApplyStatus(Character obj, Status status)
    {
        obj.Dmg = obj.MaxDmg;
        obj.Acc = obj.MaxAcc;
        obj.Dodge = obj.MaxDodge;
        obj.Initiative = obj.MaxInitiative;
        obj.Crit = obj.MaxCrit;
        obj.Armor = obj.MaxArmor;
        if (status.IsInstant)
        {
            status.Fn(obj);
        }
        if (obj.StatusList.Contains(status))
        {
            obj.StatusList.Remove(status);
        }
        obj.StatusList.Add(status);
    }

    public static void AddStatus(string name, int duration, Action<Character> fn, string type, string onapply, bool isinstant = false)
    {
        Status statusToAdd = new Status(name, duration, fn, type, onapply, isinstant);
        StatusList.Add(statusToAdd.Name, statusToAdd);
    }

    public static Status GetStatus(string name)
    {
        return StatusList[name];
    }
}