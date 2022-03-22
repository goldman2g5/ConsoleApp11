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
    // ВЫВОД ИНФЫ О СКИЛЛАХ ПО ЗАПРОСУ
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
            var rangedAttack = new Skill("Ranged Attack", 0.75, new List<int> {0, 1, 2}, new List<Status>());
            var shieldBash = new Skill("Shield Bash", 0.5, new List<int> {0, 1},
                new List<Status>() {Status.GetStatus("stun")});
            var cleave = new Skill("Cleave", 0.33, new List<int> {0, 1, 2}, new List<Status>() { }, aoe: true);
            var bleed = new Skill("Bleed", 0.5, new List<int>() {0, 1, 2, 3},
                new List<Status>() {Status.GetStatus("bleed")});
            var fortify = new Skill("fortify", 0, new List<int>() {0, 1, 2, 3},
                new List<Status>() {Status.GetStatus("buffArmor")}, true, true);
            var heal = new Skill("Heal", 1, new List<int> {0, 1, 2, 3}, new List<Status>(), true);
            var hero = new Character("Hero", 100, 70, 0, 5, 40, 0, 30,
                new List<Skill>() {attack, shieldBash, cleave, fortify});
            var obama = new Character("Obama", 100, 50, 0, 5, 10, 100, 29, new List<Skill>() {attack, bleed, heal});
            var joeBaiden = new Character("JoeBaiden", 80, 50, 0, 5, 10, 5, 28,
                new List<Skill>() {attack, bleed, heal});
            var enemy1 = new Character("Enemy1", 100, 50, 0, 5, 10, 5, 25,
                new List<Skill> {rangedAttack, shieldBash, heal});
            var enemy2 = new Character("Enemy2", 100, 50, 0, 5, 10, 5, 10,
                new List<Skill> {attack, cleave, lasthit, shieldBash, heal});
            var enemy3 = new Character("Enemy3", 100, 50, 0, 5, 10, 5, 10,
                new List<Skill> {attack, cleave, lasthit, shieldBash, heal});

            Game = new Game(new List<Character>() {hero, obama, joeBaiden},
                new List<Character>() {enemy1, enemy2, enemy3});
            Game.Start();
        }
    }
}