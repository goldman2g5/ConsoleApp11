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

    private Status(string name, int duration, Action<Character> fn, string type, string onapply, bool isInstant = false)
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
        
        void Rallybuff(Character obj)
        {
            obj.Initiative += 10;
            obj.Crit += 25;
        }
        
        void ArmorBuff(Character obj)
        {
            obj.Armor += 20;
        }
        
        AddStatus("stun", 1, Stun, "stun", "is stunned");
        AddStatus("bleed", 2, Bleed, "damage", "is bleeding");
        AddStatus("Mark", 2, Mark, "mark", "is marked");
        AddStatus("Rallybuff", 3, Rallybuff, "agressivebuff", "is empowered", true);
        AddStatus("ArmorBuff", 3, ArmorBuff, "defensivebuff", "is fortyfied", true);
    }

    private static void AddStatus(string name, int duration, Action<Character> fn, string type, string onapply, bool isinstant = false)
    {
        Status statusToAdd = new Status(name, duration, fn, type, onapply, isinstant);
        StatusList.Add(statusToAdd.Name, statusToAdd);
    }

    public static Status GetStatus(string name)
    {
        return StatusList[name];
    }
    
    public static void ApplyStatus(Character obj, Status status)
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
}