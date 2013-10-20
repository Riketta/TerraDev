// DEVELOPED BY RIKETTA 2012-2013 http://vk.com/terradev https://github.com/Riketta/TerraDev

using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
 *  Для успешной компиляции проекта добавьте ссылку на Terraria.exe версии 1.2 либо 1.1.2.
 *  Возможно работает и с другими. Проверялось лично мною только на этих.
 *  Данный хак работает как с английской версией, так и с русской. Точнее - хак не привязан к языку клиента.
 *  Всем добра, за костыли просьба не пинать.
 *  По вопросам обращаться сюда - http://vk.com/terradev
 *  С репортами о ошибках - https://github.com/Riketta/TerraDev
 *  Рад предложениям и пожеланиям
 *  Удачи в модификации хака.
 *  Кстати, о ваших интересных модификациях так же сообщайте, возможно добавлю в основную ветку.
 *  И вас внесу в список авторов :)
*/

namespace TerraDev
{
    class Program : Terraria.Main
    {
        static void Main() // string[] args
        {
            string TDVer;

            try
            {
                InjectedMain InjMain = new InjectedMain();
                TDVer = InjMain.TDVer;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Title = "TerraDev v." + TDVer;
                Console.WriteLine("                              = TerraDev {0} =", TDVer);
                Console.WriteLine("                                 by Riketta");
                Console.WriteLine("");
                Console.Write("================================================================================");
                Console.WriteLine("                            http://vk.com/TerraDev");
                Console.WriteLine("                     https://github.com/Riketta/TerraDev");
                Console.WriteLine("================================================================================");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                InjMain.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex);
                Console.ResetColor();
                Console.ReadLine();
            }
        }
    }
}
