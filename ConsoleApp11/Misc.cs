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
            switch (input)
            {
                case "info":
                    Console.WriteLine($"Battlefield: \n Allies:\n {GetCharsNamesWithLessInfo(Program.Game.Allies)}\n Enemies:\n {GetCharsNamesWithLessInfo(Program.Game.Enemies)}\n");
                    break;
                case "moreinfo":
                    Console.WriteLine($"Battlefield: \n Allies:\n {GetCharsNamesWithInfo(Program.Game.Allies)}\n Enemies:\n {GetCharsNamesWithInfo(Program.Game.Enemies)}\n");
                    break;
                case "skillinfo":
                    Console.WriteLine($"Select a skill:\n{Skill.GetNames(Game.Subject.Skills)}\n{Skill.GetInfo(Game.Subject.Skills)}");
                    break;
            }

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
    
    public static string GetCharsNamesWithLessInfo(List<Character> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"\n{i + 1}: {ls[i].Name}" +
                                                                           $"\nHp: {ls[i].Hp}/{ls[i].MaxHp}" +
                                                                           $"\nSkills: {Skill.GetNames(ls[i].Skills)}  " +
                                                                           (ls[i].GetStatuses().Any() ? $"\nStatus: {ls[i].GetStatuses()}\n" : "\n"));
    }
    
    public static string GetCharsNamesWithInfo(List<Character> ls)
    {
        return Enumerable.Range(0, ls.Count).Aggregate("", (current, i) => current + $"\n{i + 1}: {ls[i].Name}" +
                                                                           $"\nHp: {ls[i].Hp}/{ls[i].MaxHp}" +
                                                                           $"\nDMG: {ls[i].Dmg} ACC: {ls[i].Acc} CRT: {ls[i].Crit}%" +
                                                                           $"\nARM: {ls[i].Armor}% DDG: {ls[i].Dodge}%" +
                                                                           $"\nINIT: {ls[i].Initiative}" +
                                                                           $"\nSkills: {Skill.GetNames(ls[i].Skills)}  " +
                                                                           (ls[i].GetStatuses().Any() ? $"\nStatus: {ls[i].GetStatuses()}\n" : "\n"));
    }

}