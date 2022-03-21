using System.Collections.Immutable;

namespace Cosoleapp3;

public class Game
{
    public List<Character> Allies;
    public List<Character> Enemies;
    private static List<Character> TurnOrder = new List<Character>() {};

    public Game(List<Character> allies, List<Character> enemies)
    {
        Allies = allies;
        Enemies = enemies;
        foreach (var i in enemies) { i.IsAi = true; }
    }

    private List<Character> GetTurnOrder()
    {
        TurnOrder = new List<Character>() {};
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
        while (Allies.Any() & Enemies.Any()) {
            TurnOrder = GetTurnOrder();
            foreach (var subject in TurnOrder)
            {
                if (!Allies.Any() | !Enemies.Any()) { break; }
                if (subject.Dead) {continue;}
                Console.WriteLine($"Turn Order: \n{Misc.GetCharsNames(TurnOrder)}\n");
                Console.WriteLine($"Acting: {subject.Name}");
                subject.ProcessStatuses();
                ClearDead();
                if (subject.Stunned)
                {
                    subject.Stunned = false;
                    continue;
                }

                if (subject.IsAi)
                {
                    Ai.Act(Allies, Enemies, subject);
                }
                else
                {
                    var skill = subject.GetSkill();
                    skill.Use(subject, skill.GetTargets(subject));
                }
                ClearDead();
                Thread.Sleep(5000);
                Console.Clear();
            }
        };

        bool battleWon = Allies.Any();
        Console.WriteLine($"{(battleWon ? "You Won" : "You Lost")}");
        return battleWon;
    }
}