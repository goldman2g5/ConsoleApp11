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
    public Func<Character, double> StatAffected;
    private static Dictionary<string, Status> StatusList = new();

    private Status(string name, int duration, Action<Character> fn, string type, string onapply, bool isInstant = false, Func<Character, double>? affectstat = null)
    {
        Name = name;
        Fn = fn;
        Duration = duration;
        Type = type;
        IsInstant = isInstant;
        OnApply = onapply;
        if (affectstat != null)
            StatAffected = affectstat;
    }

    public static void GenerateStatuses()
    {
        
        double Dodge(Character obj)
        {
            return obj.Dodge;
        }
        
        double Dmg(Character obj)
        {
            return obj.Dmg;
        }
        
        void Stun(Character obj)
        {
            obj.Stunned = true;
            Console.WriteLine($"{obj.Name} is stunned, skipping turn");
            Thread.Sleep(3000);
        }
        
        void Bleed(Character obj)
        {
            int damage = Convert.ToInt32(obj.MaxHp * 0.2);
            obj.TakeDamage(damage);
            Console.WriteLine($"{obj.Name} bleeding for {damage}");
            Thread.Sleep(3000);
        }
        
        void Blight(Character obj)
        {
            int damage = Convert.ToInt32(obj.MaxHp * 0.1);
            obj.TakeDamage(damage);
            Console.WriteLine($"{obj.Name} blighted for {damage}");
            Thread.Sleep(3000);
        }
        
        void Rallybuff(Character obj)
        {
            obj.Initiative += 10;
            obj.Crit += 25;
        }
        
        void VaporsBuff(Character obj)
        {
            obj.Initiative += 3;
            obj.Dmg += Convert.ToInt32(obj.Dmg * 0.75);
        }
        
        void Mark(Character obj) => Console.WriteLine($"{obj.Name} is marked");

        void Riposte(Character obj) => Console.WriteLine($"{obj.Name} is riposting");
        
        void Guard(Character obj) => Console.WriteLine($"{obj.Name} is riposting");
        
        void ArmorBuff(Character obj) => obj.Armor += 0.2;
        
        void DodgeDeBuff(Character obj) => obj.Dodge -= 0.2;

        AddStatus("stun", 1, Stun, "stun", "is stunned");
        AddStatus("bleed", 2, Bleed, "damage", "is bleeding");
        AddStatus("blight", 5, Blight, "damage", "is blighted");
        AddStatus("Mark", 2, Mark, "mark", "is marked");
        AddStatus("Riposte", 3, Riposte, "riposte", "is riposting");
        AddStatus("Guard", 3, Guard, "guard", "is guarded");
        AddStatus("Rallybuff", 3, Rallybuff, "abuff", "is empowered", true);
        AddStatus("ArmorBuff", 3, ArmorBuff, "dbuff", "is fortyfied", true);
        AddStatus("DodgeDeBuff", 3, DodgeDeBuff, "ddbuff", "is crippled", true, Dodge);
        AddStatus("VaporsBuff", 3, VaporsBuff, "abuff", "is empowered", true);
    }

    private static void AddStatus(string name, int duration, Action<Character> fn, string type, string onapply, bool isinstant = false, Func<Character, double>? affectsatat = null)
    {
        var statusToAdd = new Status(name, duration, fn, type, onapply, isinstant, affectsatat);
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
            status.Fn(obj);
        if (obj.StatusList.Contains(status)) 
            obj.StatusList.Remove(status);
        
        obj.StatusList.Add(status);
    }
}