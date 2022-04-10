using System.Collections.Immutable;

namespace Cosoleapp3;

public class Game
{
    public List<Character> Allies;
    public List<Character> Enemies;
    public static Character Subject;
    public static List<Character> TurnOrder = new List<Character>() { };

    public Game(List<Character> allies, List<Character> enemies)
    {
        Allies = allies;
        Enemies = enemies;
        foreach (var i in enemies)
        {
            i.IsAi = true;
        }
    }

    private List<Character> GetTurnOrder()
    {
        TurnOrder = new List<Character>() { };
        TurnOrder.AddRange(Allies);
        TurnOrder.AddRange(Enemies);
        TurnOrder = TurnOrder.OrderBy(x => x.Initiative).ToList();
        TurnOrder.Reverse();
        return TurnOrder;
    }

    public void ClearDead()
    {
        TurnOrder = TurnOrder.Where(x => !x.Dead).ToList();
        Allies = Allies.Where(x => !x.Dead).ToList();
        Enemies = Enemies.Where(x => !x.Dead).ToList();
    }

    public bool Start()
    {
        while (true)
        {
            Console.Clear();
            Thread.Sleep(1000);
            if (!TurnOrder.Any())
                TurnOrder = GetTurnOrder();

            Subject = TurnOrder[0];
            if (!Allies.Any() | !Enemies.Any()) return !Enemies.Any();
            if (Subject.Dead) Start();

            Console.WriteLine($"Turn Order: \n{Misc.GetCharsNames(TurnOrder)}\n");
            Console.WriteLine($"Acting: {Subject.Name}");
            Subject.ProcessStatuses();

            ClearDead();
            if (Subject.Stunned)
            {
                Subject.Stunned = false;
                TurnOrder.Remove(Subject);
                Start();
            }

            if (Subject.IsAi)
            {
                new Ai(Allies, Enemies, Subject).Act();
            }
            else
            {
                var skill = Subject.GetSkill();
                skill.Use(Subject, skill.GetTargets());
            }

            ClearDead();
            Thread.Sleep(5000);
            TurnOrder.Remove(Subject);
        }
    }
}