using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Cosoleapp3
{
    // #TODO
    // GENERAL
    // ДОПИСАТЬ ОГРАНИЧЕНИЯ ПРИМЕНЕНИЯ СКИЛОВ В ИИ => СТАТУС И СКИЛЛ ПЕРЕМЕЩЕНИЯ
    // ДОБАВИТЬ БРОНЮ В ФОРМУЛЫ РАССЧЕТА УРОНА
    // moveself


    // ИИ
    // ВЫБОР УРОВНЯ СЛОЖНОСТИ
    // ДЕЙТВИЕ САППОРТ - ЗАЩИТНЫЙ/АТАКУЮЩИЙ БАФ/ДЕБАФ ПО ТИПУ И КЛАССУ С РАЗВТВЛЕНИЕМ ЧЕРЕЗ РАНДОМ
    // СЕЙВ С ВОЗМОЖНОСТЯМИ ЗАСВИТЬ СОЮЗНИКА
    // ДОБАВИТЬ В ВЫБОР ЦЕЛИ УЧЕТ БРОНИ И УКЛОНЕНИЯ
    // ПЕРЕПИСАТЬ ТАРГЕТИНГ ИСПОЛЬЗУЯ THENBY
    // ИСПОЛЬЗОВАНИЕ СКИЛЛА ПЕРЕМЕЩЕНИЯ ПО ОЦЕНКИ ДОСТУПНЫХ СКИЛЛОВ

    // КОНТЕНТ
    // ВЫВОДИТЬ ХП ШАНС ПОПАДАНИЯ И УРОН ПРОСЛЕ ВЫБОРА СКИЛЛА
    // МАКСИМАЛЬНО ИГРАБЕЛЬНЫЙ ВЫВОД НА КОНСОЛЬ
    // МЕНЮ СТАРТА ИГРЫ
    // МЕНЮ ВЫБОРА ПЕРСОНАЖЕЙ
    // 3 ПОЛНОЦЕННЫХ БИТВЫ С ПЕРСОНАЖАМИ

    // КОД
    // УЛУЧШИТЬ ЛОГИКУ ПРИМЕНЕНИЯ НА СЕБЯ/СОЮЗНИКОВ/ВРАГОВ

    public class Program
    {
        public static Game Game;
        public static int Difficulty;

        static void Main(string[] args)
        {
            Status.GenerateStatuses();
            var move = new Skill("Move", isMoveSkill: true, useonself: true);

            
            var crush = new Skill("Crush", damage: 1, targets: new List<int>() {0, 1}, usablefrom: new List<int>() {0, 1});
            var cleave = new Skill("cleave", damage: 0.67, targets: new List<int>() {0, 1}, usablefrom: new List<int>() {0, 1}, aoe: true);
            var stunningblow = new Skill("StunningBlow", damage: 0.5, targets: new List<int>() {0, 1}, new List<Status>() {Status.GetStatus("stun")}, usablefrom: new List<int>() {0, 1});
            var bulwark = new Skill("Bulwark", useonaliies: true, aoe: true, statusList: new List<Status>() {Status.GetStatus("ArmorBuff")}); 
            var zealousheal = new Skill("ZealousHeal", damage: 0.5, useonaliies: true);
            var holylance = new Skill("holylance", damage: 1.15, targets: new List<int>() {1, 2 ,3}, usablefrom: new List<int>() {2, 3}, moveself: 2);
            var crusader = new Character("Crusader", 61, 19, 0, 0.05, 0.2, 5, 3, new List<Skill>() {crush, stunningblow, bulwark, holylance});
            
            var slice = new Skill("Slice", damage: 1, targets: new List<int>() {0, 1}, usablefrom: new List<int>() {0, 1, 2});
            var pistolShot = new Skill("pistolShot", damage: 0.85, targets: new List<int>() {1, 2 ,3}, usablefrom: new List<int>() {1, 2, 3});
            var duelistddvance = new Skill("Duelist's Advance", damage: 0.67, targets: new List<int>() {0, 1 ,2}, usablefrom: new List<int>() {1, 2, 3}, moveself: 1, statusList: new List<Status>{Status.GetStatus("Riposte")}, buffself: true);
            var glasscanon = new Skill("Point Blank Shot", damage: 1.5, targets: new List<int>() {0}, usablefrom: new List<int>() {0}, moveself: -1);
            var highwayman = new Character("HighWayman", 43, 16, 10, 0.3, 0.05,10, 7, new List<Skill>() {slice, pistolShot, duelistddvance, glasscanon});

            var noxiousnlast = new Skill("Noxious Blast", damage: 0.2, targets: new List<int>() {0, 1}, usablefrom: new List<int>() {0, 1, 2, 3}, statusList: new List<Status>{Status.GetStatus("blight")});
            var plaguegrenade = new Skill("Plague Grenade", damage: 0.1, targets: new List<int>() {2, 3}, aoe: true, usablefrom: new List<int>() {2, 3}, statusList: new List<Status>{Status.GetStatus("blight")});
            var blindinggas = new Skill("Blinding Gas", targets: new List<int>() {0, 1, 2, 3}, usablefrom: new List<int>() {2, 3}, statusList: new List<Status>{Status.GetStatus("stun")});
            var emboldeningvapours = new Skill("Emboldening Vapours", statusList: new List<Status>{Status.GetStatus("VaporsBuff")}, useonaliies: true);
            var plaguedoctor = new Character("PlagueDoctor", 38, 13, 0, 0.2, 0.05,5, 9, new List<Skill>() {noxiousnlast, plaguegrenade, blindinggas, emboldeningvapours});
            
            var judgement = new Skill("Judgement", damage: 0.75, usablefrom: new List<int>() {3, 4});
            var dazzlinglight = new Skill("Dazzling Light", damage: 0.25, targets: new List<int>() {0, 1, 2}, new List<Status>() {Status.GetStatus("stun")}, usablefrom: new List<int>() {1, 2 ,3});
            var divinegrace = new Skill("Divine Grace", damage: 1, useonaliies: true, usablefrom: new List<int>() {3, 4});
            var divinecomfort = new Skill("Divine Comfort", damage: 0.25, useonaliies: true, aoe: true, usablefrom: new List<int>() {3, 4});
            var vestal = new Character("Vestal", 44, 14, 0, 0.2, 0.05,5, 6, new List<Skill>() {judgement, dazzlinglight, divinecomfort, divinegrace});
            
            
            
            
            var spikedMace = new Skill("Spiked Mace", targets: new List<int> {0, 1}, statusList: new List<Status>() {Status.GetStatus("bleed")}, usablefrom: new List<int>() {0, 1});
            var shieldBash = new Skill("Shield Bash", 0.5, new List<int> {0, 1}, new List<Status>() {Status.GetStatus("stun")}, usablefrom: new List<int>() {0, 1, 2});
            var unholyguard = new Skill("Unholy Guard", 0, statusList: new List<Status>() {Status.GetStatus("Guard")}, useonaliies: true);
            var skeletonVeteran = new Character("Skeleton Veteran", 55, 25, 0, 0.05, 0.4, 10, 3,
                new List<Skill> {spikedMace, shieldBash, unholyguard}, role: "T");
            
            var spearCharge = new Skill("Spear Charge", 0.67, new List<int> {0, 1, 2}, usablefrom: new List<int>() {0, 1}, aoe: true);
            var spearStrike = new Skill("Spear Strike", 1, new List<int> {0, 1, 2}, usablefrom: new List<int>() {0, 1});
            var spearRiposte = new Skill("Riposte", 0.5, new List<int> {0, 1, 2, 4}, new List<Status>() {Status.GetStatus("Riposte")}, usablefrom: new List<int>() {0, 1},  buffself: true);
            var skeletonSpearman = new Character("Skeleton Spearman", 40, 24, 0, 0.15, 0.25, 10, 6,
                new List<Skill> {spearStrike, spearCharge, spearRiposte}, role: "D");
            
            var bannerstrike = new Skill("Unexpected attack", 1, new List<int> {0, 1 ,2 ,3}, new List<Status>() {Status.GetStatus("DodgeDeBuff")});
            var bannerlordrally = new Skill("Rally To The Flame", 0, new List<int> {0, 1, 2, 3}, new List<Status>() {Status.GetStatus("Rallybuff")}, aoe: true, useonaliies: true);
            var unholyheal = new Skill("Unholy Restoration", 1, new List<int> {0, 1, 2, 3}, new List<Status>(), useonaliies: true, aoe: true);
            var bannermark = new Skill("Mark for death", 0, new List<int> {0, 1, 2, 3}, new List<Status>() {Status.GetStatus("Mark")});
            var skeletonBannerLord = new Character("Skeleton Banner Lord", 28, 14, 0, 0.30,0.30, 5, 50,
                new List<Skill> {bannerstrike, bannerlordrally, unholyheal, bannermark});
            
            var crosbowbolt = new Skill("Crosbow Bolt", 1, new List<int> {0, 1 ,2 ,3}, new List<Status>() {}, markdamage: true, usablefrom: new List<int>() {3});
            var suppressingfire = new Skill("Suppressing Fire", 0.67, new List<int> {1 ,2 ,3}, new List<Status>() {}, aoe: true, usablefrom: new List<int>() {3});
            var skeletonArcher = new Character("Skeleton Crossbowman", 35, 16, 0, 0.10, 0.20, 15, 4,
                new List<Skill> {crosbowbolt, suppressingfire}, role: "D");

            // var charlist = new List<Character> {crusader, highwayman, plaguedoctor, vestal};
            // var allies = new List<Character>();
            // while (allies.Count != 4)
            // {
            //     Console.WriteLine($"Select Characters \n{Misc.GetCharsNamesWithInfo(charlist)}\n");
            //     Console.WriteLine($"{Misc.GetCharsNames(allies)}");
            //     var character = charlist[Misc.VerfiedInput(charlist.Count)];
            //     charlist.Remove(character);
            //     allies.Add(character);
            //     Thread.Sleep(1000);
            //     Console.Clear();
            //     Thread.Sleep(1000);
            // }
            
            // Console.WriteLine("Select a difficulty 0-100\nmore means hard1er");
            // Difficulty = Misc.VerfiedInput(100);

            Difficulty = 100;
            var allies = new List<Character> {crusader, highwayman, plaguedoctor, vestal};
            Game = new Game(allies,
                new List<Character>() {skeletonVeteran, skeletonSpearman, skeletonBannerLord, skeletonArcher});
            Console.WriteLine($"{(Game.Start()? "You Won" : "You Lost")}");
        }
    }
}