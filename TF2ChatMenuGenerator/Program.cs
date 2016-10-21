using System;
using System.Collections.Generic;
using System.IO;

namespace TF2ChatMenuGenerator
{
    class Program
    {

        static void Main(string[] args)
        {
            string input;
            string totalMessage;

            emptyScreen();

            Console.Write("With what key do you want to open the chat menu selection?");
            input = Console.ReadKey().KeyChar.ToString();

            string activateKey = input;

            input = "";
            emptyScreen();

            Console.Write("How many 'menus' do you want? ");
            input = Console.ReadKey().KeyChar.ToString();

            int numberOfTabs;

            while (!int.TryParse(input, out numberOfTabs))
            {
                Console.WriteLine("\n\nThat is not a valid number... Try again.");
                Console.Write("How many 'menus' do you want? ");
                input = Console.ReadKey().KeyChar.ToString();
            }

            emptyScreen();

            List<Menu> menus = new List<Menu>();

            for (int i = 0; i < numberOfTabs; i++)
            {
                emptyScreen();
                Menu tempmenu = new Menu();
                tempmenu.activateKey = (i + 1).ToString();
                Console.WriteLine("This is menu #" + (i + 1) + " and it will use the " + (i + 1) + " key when you have pressed " + activateKey.ToUpper() + ".");
                Console.Write("\nNow give this menu a name. ");
                input = Console.ReadLine();
                tempmenu.title = input;

                Console.Write("\nDo you want this menu to be team chat or normal chat? '1' for regular chat and '2' for team chat. ");
                input = Console.ReadKey().KeyChar.ToString();

                while (true)
                {
                    if (input == "1")
                    {
                        tempmenu.chatMode = "say";
                        break;
                    }
                    else if (input == "2")
                    {
                        tempmenu.chatMode = "say_team";
                        break;
                    }
                    else
                    {
                        Console.Write("\n\nThat was not a valid answer. Choose either '1' for regular chat or '2' for team chat. ");
                        input = Console.ReadKey().KeyChar.ToString();
                    }
                }

                Console.Write("\n\nNow tell me how many binds you want this 'menu' to contain. ");
                input = Console.ReadKey().KeyChar.ToString();

                while (!int.TryParse(input, out numberOfTabs))
                {
                    Console.Write("\n\nThat is not a valid number. Please try again. ");
                    input = Console.ReadKey().KeyChar.ToString();
                }

                Console.Write("\n\nNow it's time to give me the messages.");

                for (int j = 0; j < numberOfTabs; j++)
                {
                    Console.WriteLine("\nWhat will be the message of the " + (j + 1) + " key? (You can paste with right click.)");
                    input = Console.ReadLine();
                    tempmenu.messages.Add(input);

                }
                menus.Add(tempmenu);
            }


            //For the creation.
            List<string> total = new List<string>();

            int biggestLength = 0;
            foreach (Menu menu in menus)
            {
                if (menu.messages.Count > biggestLength)
                {
                    biggestLength = menu.messages.Count;
                }
            }

            int a;
            if (numberOfTabs > biggestLength)
            {
                a = numberOfTabs;
            }
            else
            {
                a = biggestLength;
            }

            string resetKeys = "alias resetNumberKeys \"";
            for (int i = 0; i < a; i++)
            {
                resetKeys += "alias key_" + i + " slot" + i + "; ";
                total.Add("bind " + (i + 1) + " key_" + (i + 1));
            }

            resetKeys = resetKeys.Substring(0, resetKeys.Length - 2) + "\"";

            total.Add("");

            total.Add(resetKeys);
            
            total.Add("");

            total.Add("alias menuMessage_Title \"echo Key | Menu Title\"");
            total.Add("alias bindMessage_Title \"echo Key | Bind Text\"");
            total.Add("bind " + activateKey.ToUpper() + " menuBind");
            total.Add("");

            total.Add("con_filter_text_out \" \"");
            total.Add("alias startHud \"clear; developer 1; con_filter_enable 0; contimes 6; con_notifytime 100; separator_start\"");
            total.Add("alias stopHud  \"developer 0; con_filter_enable 0; con_notifytime 8; contimes 8\"");
            total.Add("alias separator_start \"echo [------------------------------------------------------------]\"");
            total.Add("alias separator \"con_filter_enable 0; echo [------------------------------------------------------------]\" //I would have liked to use \"con_filter_enabled 1\" here, but for some reason the next \"con_filter_enable 0\" won't work and the binds wouldn't show.");
            total.Add("stopHud");
            total.Add("");


            totalMessage = "alias menuMessage \"startHud; ";
            foreach (Menu menu in menus)
            {
                total.Add("alias menuMessage_message" + menu.activateKey + " \"echo " + menu.activateKey + ": " + menu.title + "\"");
                totalMessage += "menuMessage_message" + menu.activateKey + "; ";
            }

            totalMessage += "separator\"";
            total.Add(totalMessage);
            total.Add("");

            totalMessage = "alias menuBind \"menuMessage; ";
            foreach (Menu menu in menus)
            {
                totalMessage += "alias key_" + menu.activateKey + " textBind_" + menu.activateKey + "; ";
            }
            totalMessage = totalMessage.Substring(0, totalMessage.Length - 2) + "\"";
            total.Add(totalMessage);
            total.Add("");

            int count = 1;
            foreach (Menu menu in menus)
            {
                count = 1;
                totalMessage = "alias bindMessage_" + menu.activateKey + " \"startHud; bindMessage_Title; ";
                foreach (string text in menu.messages)
                {
                    total.Add("alias bindMessage_" + menu.activateKey + "_Message" + count + " \"echo " + count + ": " + text + "\"");
                    totalMessage += "bindMessage_" + menu.activateKey + "_Message" + count + "; ";
                    count++;
                }

                totalMessage += "separator\"";
                total.Add(totalMessage);
                total.Add("");

            }

            foreach (Menu menu in menus)
            {
                count = 1;
                totalMessage = "alias textBind_" + menu.activateKey + " \"bindMessage_" + menu.activateKey + "; ";
                foreach (string text in menu.messages)
                {
                    total.Add("alias textBind_" + menu.activateKey + "_" + count + " \"stopHud; resetNumberKeys; " + menu.chatMode + " " + text + "\"");
                    totalMessage += "alias key_" + count + " textBind_" + menu.activateKey + "_" + count + "; ";
                    count++;
                }
                totalMessage = totalMessage.Substring(0, totalMessage.Length - 2) + "\"";
                total.Add(totalMessage);
                total.Add("");
            }

            File.WriteAllLines(@"chatmenu.cfg", total);

            emptyScreen();

            Console.WriteLine("You're almost done.\nYou will also need \"exec chatmenu\" in your autoexec.cfg.");
            Console.Write("\nDo you want to generate an empty autoexec.cfg with \"exec chatmenu\" in it? y/n: ");

            input = Console.ReadKey().KeyChar.ToString().ToLower();

            if (input == "y")
            {
                string[] autoexec = { "exec chatmenu" };
                File.WriteAllLines(@"autoexec.cfg", autoexec);
            }

        }

        public static void emptyScreen()
        {
            Console.Clear();
            Console.WriteLine("------------------------------");
            Console.WriteLine("Tvde1's Chat Menu Generator");
            Console.WriteLine("------------------------------");
            Console.WriteLine();
        }
    }

    class Menu
    {
        public string title { get; set; }
        public string chatMode { get; set; }
        public string activateKey { get; set; }
        public List<string> messages = new List<string>();

    }
}
