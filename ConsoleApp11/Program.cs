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
    // СТАТУС И СКИЛЛ ПЕРЕМЕЩЕНИЯ
    // ДОБВАИТЬ КЛАССЫ СТРОКОЙ В КОНСТРУКТОР ПЕРСОНАЖА
    // ДОБАВИТЬ БРОНЮ В ФОРМУЛЫ РАССЧЕТА УРОНА
    // ДОБАВИТЬ МОДИФИКАТОР ТОЧНОИСТИ К СКИЛЛАМ И ВО ВСЕ ФОРМУЛЫ


    // ИИ
    // ВЫБОР УРОВНЯ СЛОЖНОСТИ
    // ДЕЙТВИЕ САППОРТ - ЗАЩИТНЫЙ/АТАКУЮЩИЙ БАФ/ДЕБАФ ПО ТИПУ И КЛАССУ С РАЗВТВЛЕНИЕМ ЧЕРЕЗ РАНДОМ
    // СОЗДАТЬ КЛАСС ЭКЗЕМПЛЯР ПРИОРИТЕТА ДЕЙТСВИЙ, ПЕРЕДОВАТЬ ЕГО ПРИ СОЗДАНИИ ПЕРСОНАЖА
    // ДОБАВИТЬ В ВЫБОР ЦЕЛИ УЧЕТ БРОНИ И УКЛОНЕНИЯ

    // КОНТЕНТ
    // ВЫВОДИТЬ ХП ШАНС ПОПАДАНИЯ И УРОН ПРОСЛЕ ВЫБОРА СКИЛЛА
    // МАКСИМАЛЬНО ИГРАБЕЛЬНЫЙ ВЫВОД НА КОНСОЛЬ
    // МЕНЮ СТАРТА ИГРЫ
    // МЕНЮ ВЫБОРА ПЕРСОНАЖЕЙ
    // 3 ПОЛНОЦЕННЫХ БИТВЫ С ПЕРСОНАЖАМИ

    // КОД
    // 

    public class Program
    {
        public static Game Game;

        static void Main(string[] args)
        {
            Status.GenerateStatuses();
            var attack = new Skill("Attack", 1, new List<int> {0}, new List<Status>());
            var lasthit = new Skill("Lasthit", 2, new List<int> {0, 1}, new List<Status>());
            var rangedAttack = new Skill("Ranged Attack", 0.75, new List<int> {0, 1, 2}, new List<Status>(), markdamage: true);
            var sniperMark = new Skill("Sniper Mark", 0, new List<int> {0, 1, 2, 3}, new List<Status>() {Status.GetStatus("Mark")});
            var shieldBash = new Skill("Shield Bash", 0.5, new List<int> {0, 1},
                new List<Status>() {Status.GetStatus("stun")});
            var cleave = new Skill("Cleave", 0.33, new List<int> {0, 1, 2}, new List<Status>() { }, aoe: true);
            var bleed = new Skill("Bleed", 0.5, new List<int>() {0, 1, 2, 3},
                new List<Status>() {Status.GetStatus("bleed")});
            var fortify = new Skill("fortify", 0, new List<int>() {0, 1, 2, 3},
                new List<Status>() {Status.GetStatus("Armor buff")}, true, true);
            var heal = new Skill("Heal", 1, new List<int> {0, 1, 2, 3}, new List<Status>(), true);
            var alacrity = new Skill("Alacrity", 0, new List<int> {0, 1, 2, 3}, 
                new List<Status>() { Status.GetStatus("Damage buff"), Status.GetStatus("Crit buff"), Status.GetStatus("Acc buff"), Status.GetStatus("Init buff")}, true);
            var hero = new Character("Hero", 100, 70, 0, 5, 40, 0, 30,
                new List<Skill>() {attack, shieldBash, cleave, fortify, sniperMark, rangedAttack, alacrity});
            var obama = new Character("Obama", 100, 50, 0, 5, 10, 10, 6, 
                new List<Skill>() {attack, bleed, heal, sniperMark, rangedAttack});
            var joeBaiden = new Character("JoeBaiden", 80, 50, 0, 5, 10, 5, 7,
                new List<Skill>() {attack, bleed, heal, sniperMark, rangedAttack});
            var enemy1 = new Character("enemy1", 100, 50, 0, 5, 10, 5, 25,
                new List<Skill> {shieldBash, heal, sniperMark}, role: "Tank");
            var enemy2 = new Character("enemy2", 100, 50, 0, 5, 10, 5, 10,
                new List<Skill> {rangedAttack, attack, cleave, lasthit, shieldBash, heal}, role: "DD");
            var enemy3 = new Character("enemy3", 100, 50, 0, 5, 10, 5, 50,
                new List<Skill> {attack, alacrity, heal});

            Game = new Game(new List<Character>() {hero, obama, joeBaiden},
                new List<Character>() {enemy1, enemy2, enemy3});
            Game.Start();
        }
    }
}