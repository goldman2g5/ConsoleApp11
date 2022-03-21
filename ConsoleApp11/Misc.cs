namespace Cosoleapp3;

public class Misc
{
    public static int VerfiedInput(int limit)
    {
        Console.Write(">> ");
        string input = Console.ReadLine();
        int intInput;
        while (!int.TryParse(input, out intInput) || Convert.ToInt32(input) > limit || Convert.ToInt32(input) < 1)
        {
            // if (input == -1)
            // {
            //     Console.WriteLine($"Battlefield: \n Allies:\n {Misc.GetCharsNamesWithInfo(Program.Game.Allies)}\n Enemies:\n {Misc.GetCharsNamesWithInfo(Program.Game.Enemies)}\n");
            // }
            Console.WriteLine(input);
            Console.Write(">> ");
            input = Console.ReadLine();
        }
        return Convert.ToInt32(intInput - 1);
    }

    public static bool Roll(int chance)
    {
        return new Random().Next(100) <= chance;
    }
    
    public static string GetCharsNames(List<Character> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"{i + 1}: {ls[i].Name}   ");
    }
    
    public static string GetCharsNamesWithInfo(List<Character> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"\n{i + 1}: {ls[i].Name}" +
                                                                           $"\nHp: {ls[i].Hp}/{ls[i].MaxHp}" +
                                                                           $"\nDMG: {ls[i].Dmg} ACC: {ls[i].Acc} CRT: {ls[i].Crit}%" +
                                                                           $"\nARM: {ls[i].Armor}% DDG: {ls[i].Dodge}%" +
                                                                           $"\nINIT: {ls[i].Initiative}" +
                                                                           $"\nSkills: {Skill.GetNames(ls[i])}  " +
                                                                           $"\nStatus: {ls[i].GetStatuses()}\n");
    }

}