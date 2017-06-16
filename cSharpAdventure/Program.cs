using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cSharpAdventure
{
    public class Player
    {
        public string Name { get; set; }
        public ObservableCollection<InventoryItem> Inventory { get; private set; }

        public Player()
        {
            Inventory = new ObservableCollection<InventoryItem>();
            Inventory.CollectionChanged += Inventory_CollectionChanged;
        }

        private void Inventory_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            foreach (InventoryItem item in e.NewItems)
            {
                Console.WriteLine("Received '{0}'!\n({1})", item.Name, item.Description);
            }

            Console.WriteLine();
            Console.ReadLine();
        }

        public bool HasItem(string name)
        {
            return Inventory.Any(item => item.Name.ToLower() == name.ToLower());
        }
    }
    public struct InventoryItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public abstract class GameScreen
    {
        protected IDictionary<string, Func<GameScreen>> MenuItems;

        public abstract GameScreen Run();

        protected void Write(string text)
        {
            Console.Write(text);
            Console.WriteLine();
            Console.WriteLine("[ENTER]");
            Console.ReadLine();
        }

        protected string Prompt(string text)
        {
            Console.WriteLine(text);
            var result = Console.ReadLine();
            Console.WriteLine();
            return result;
        }

        protected GameScreen Menu()
        {
            Console.WriteLine("What do you do?");

            var i = 0;
            foreach (var item in MenuItems)
            {
                i++;
                Console.WriteLine("[{0}] {1}", i, item.Key);
            }

            Console.WriteLine();
            Console.WriteLine("Selection?");
            var input = Console.ReadLine();

            int selection;
            if (int.TryParse(input, out selection))
            {
                if (selection > 0 && selection <= MenuItems.Count)
                {
                    return MenuItems.ElementAt(selection - 1).Value();
                }
            }

            return null;
        }
    }
    public class IntroScreen : GameScreen
    {
        private readonly Player _player;

        public IntroScreen(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
        {
            {"Ask the passenger a few rows in front of you.", () =>
                                    {
                                        MenuItems.Remove("Ask the passenger a few rows in front of you.");
                                        Write("You sit up so you can see over the seats and ask the man a few rows ahead if he noticed anything.\n He introduces himself, but has no recolection of anything until he was jolted awake\n by a noise or bump...");
                                        _player.Inventory.Add(new InventoryItem
                                                                    { Name = "Nakatomi Corp Business Card - Frank Reynolds",
                                                                      Description = "A business card for a middle manager at a large corporation. Nothing specifically stands out about this card."
                                                                    });
                                        return new IntroScreen2(player);
                                    } },
            {"Go to the next car to see what is happening yourself.", () => new IntroScreen2(player) }
        };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();
            if (!_player.HasItem("GREEN BAG")) return Intro();
            return Menu();
        }

        private GameScreen Intro()
        {
            var arr2 = new[]
{
          @"                             .---._",
          @"             .--(. '  .).--.      . .-.",
          @"         . ( ' _) .)` (   .)-. ( ) '-'",
          @"         ( ,  ).        `(' . _)",
          @"       (')  _________      '-'",
          @"       ____[_________]                                         ________",
          @"       \__/ | _ \  ||    ,;,;,,                               [________]",
          @"      _][__|(')/__||  ,;;;;;;;;,   __________   __________   _| LILI |_",
          @"      /             | |____      | |          | |  ___     | |      ____|",
          @"     (| .--.    .--.| |     ___  | |   |  |   | |      ____| |____      |",
          @"     /|/ .. \~~/ .. \_|_.-.__.-._|_|_.-:__:-._|_|_.-.__.-._|_|_.-.__.-._|",
          @"  +=/_|\ '' /~~\ '' /=+(o ) (o )+==(o ) (o )=+=(o ) (o )+==(o ) (o )=+=",
          @"  ='=='='--'==+='--'===+'-'=='-'==+=='-'+='-'===+='-'=='-'==+=='-'=+'-'jgs+",
        };

            Draw.RunDrawDown(arr2);
            Console.CursorVisible = true;
            Write("You jolt awake. Did the train just hit something? Was there a loud bang? Was it just a dream?\n You feel a headache coming on. Too bad that vodka bottle is already empty.");
            Write("You hear a commotion in the next train car. It's hard to tell what is happening but at least a few people are shouting.");
            Write("The door behind you opens, two police officers run past, entering the car where you heard shouting.\n As the door opens, you hear \"YOU WILL NEVER...\" The door slides closed behind them quieting the noise.");
            Write("Should I ask the passenger a few rows ahead of me what is happening? Or should I get up and go check it out?");


            //var bag = new InventoryItem { Name = "GREEN BAG", Description = "A general-purpose garbage bag." };
           // _player.Inventory.Add(bag);

            var name = string.Empty;
            int attempts = 0;
            while (string.IsNullOrEmpty(name))
            {
                attempts++;
                string prompt;
                if (attempts == 1)
                {
                    prompt = "You look around, and notice a nametag stuck to the coat across your lap. It reads \"Hello, My Name is...\" (enter it!):";
                    name = Prompt(prompt);
                } else
                {
                    name = "Dude";
                    prompt = string.Format("Ok nevermind, we'll call you {0}.", name);
                    Write(prompt);
                }
            }
            _player.Name = name;

            return Menu();
        }
    }

    public class IntroScreen2 : GameScreen
    {
        private readonly Player _player;

        public IntroScreen2(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
                                    {
                                        { "Open the door to the next train car.", () => new Car1(player) }
                                    };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();

                Write("You stand up and toss your coat back on the seat, and walk towards the door.\n It's opaque glass, so you cannot see what is happening in the next car. You don't see any movement or hear any yelling anymore.");
           
            return Menu();
        }
    }

    public class Car1 : GameScreen
    {
        private readonly Player _player;

        public Car1(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
                                        {
                                          { "You pass out from the sight of blood and hit your head.", () => null },
                                          { "Step forward into the train car and investigate.", () => new Car1p2(player) }
                                        };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();
            var arr2 = new[]
            {
          @"                        ______________",
         @"                       |\ ___________ /|",
         @"                       | |  /|,| |   | |",
         @"                       | | |,x,| |   | |",
         @"                       | | |,x,' |   | |",
         @"                       | | |,x   ,   | |",
         @"                       | | |/    |   | |",
         @"                       | |    /] ,   | |",
         @"                       | |   [/ ()   | |",
         @"                       | |       |   | |",
         @"                       | |       |   | |",
         @"                       | |       |   | |",
         @"                       | |      , '  | |",
         @"                       | |   , '     | |",
         @"                       |_|,'_________|_|",
         };
           Draw.RunDrawDown(arr2);
            Console.CursorVisible = true;

                Write("You are shocked when the door opens. The man who was sitting a few rows ahead of you gasps.\n The lights in this car are malfunctioning, but for those few moments they are on...");


            return Menu();
        }
    }

    public class Car1p2 : GameScreen
    {
        private readonly Player _player;

        public Car1p2(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
                                    {
                                        { "Step into the train car. Frank has your back.", () => new Car2(player) }
                                    };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();
                Write("\"Wait! Are you crazy?!?\" The man who was sitting a few rows ahead of you stands up.\n \"Don't go in there alone! I don't know what is happening, but we should probably stick together until things get back to normal.\"\n \"My name is Frank, what's yours?\"");

                Write($"\"Hey Frank, I'm {_player.Name}. My head hurts. I just want to get off this train...\"");
                Write($"\"Well {_player.Name}, this whole situation is strange... Let's be careful and find something to protect ourselves with.\"");           

            return Menu();
        }
    }
    public class Car2 : GameScreen
    {
        private readonly Player _player;

        public Car2(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
                                    {
                                        { "Open the bathroom door, see if there's anything useful as a weapon", () => new Car2p2(player) },
                                        { "Take the fire extinguisher, it's better than nothing. ", () =>
                                            { _player.Inventory.Add(new InventoryItem
                                                                    { Name = "Fire extinguisher",
                                                                      Description = "Standard size red fire extinguisher. You notice the inspection tag is 10 years expired. Hope there's no fires."
                                                                    });
                                                                    return this; }
                                        },
                                        { "Keep going to the next car, but take a look at each row of seats and see if there's anything lying around.", () => new Car2p2(player) }


                                    };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();
            Write("You turn the flashlight on your cell phone on and step into the car.\n All the seats are empty. No people. You call out, \"Hello, anybody?\"");
            Write("No response, only the sound of the train rumbling down the tracks.\n You look out the window, but with no moon it's very dark and hard to tell where you are.");
            Write("The restroom door for this train car is just to your left, a small fire extinguisher is hanging in its spot right by the door.\n Otherwise you don't notice anything odd about this train car.\n Except for all the blood everywhere. And already dry. Weird. ");

            if (_player.HasItem("Fire extinguisher"))
            {
                Write("You clutch the fire extinguisher like an awkward sized baseball bat as you move forward through the train car. Can't use the flashlight very well now.");
                MenuItems = new Dictionary<string, Func<GameScreen>> { { "Keep moving", () => new Car2p2(_player) } };
            }

            return Menu();
        }
    }
    public class Car2p2 : GameScreen
    {
        private readonly Player _player;

        public Car2p2(Player player)
        {
            MenuItems = new Dictionary<string, Func<GameScreen>>
                                    {
                                        { "Wake up from your nightmare.", () => null }
                                    };
            _player = player;
        }

        public override GameScreen Run()
        {
            Console.Clear();
            Write("Before you even have a chance to move a sense of fear washes over you. A giant lump in your throat begins to form, you can hardly swallow.\n Panic is setting in...");

            return Menu();
        }
    }
    public class Draw
    {
        static void ConsoleDraw(IEnumerable<string> lines, int x, int y)
        {
            if (x > Console.WindowWidth) return;
            if (y > Console.WindowHeight) return;

            var trimLeft = x < 0 ? -x : 0;
            int index = y;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            var linesToPrint =
                from line in lines
                let currentIndex = index++
                where currentIndex > 0 && currentIndex < Console.WindowHeight
                select new
                {
                    Text = new String(line.Skip(trimLeft).Take(Math.Min(Console.WindowWidth - x, line.Length - trimLeft)).ToArray()),
                    X = x,
                    Y = y++
                };

            Console.Clear();
            foreach (var line in linesToPrint)
            {
                Console.SetCursorPosition(line.X, line.Y);
                Console.Write(line.Text);
            }
        }
        public static void RunDrawDown(string[] arr)
        {
            Console.CursorVisible = false;
            var maxLength = arr.Aggregate(0, (max, line) => Math.Max(max, line.Length));
            var x = Console.BufferWidth / 2 - maxLength / 2;
            for (int y = -arr.Length; y < Console.WindowHeight + arr.Length; y++)
            {
                ConsoleDraw(arr, x, y);
                Thread.Sleep(100);
            }

        }
    }
    class Program
    {


        static void Main(string[] args)
        {
            var player = new Player();

            var intro = new IntroScreen(player);
            var nextScreen = intro.Run();

            while (nextScreen != null)
            {
                nextScreen = nextScreen.Run();
            }


        }
    }
}