using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class CountToFunnyScript : MonoBehaviour
{
    private struct User
    {
        public string Name { get; private set; }
        public string HexCode { get; private set; }
        public int Ping { get; private set; }
        public Preference Likes { get; private set; }

        public User(string name, string hexCode, int ping, Preference likes)
        {
            Name = name;
            HexCode = hexCode;
            Ping = ping;
            Likes = likes;
        }

        public delegate bool Preference(int number);
    }


    //public stuff
    public KMAudio Audio;
    public List<KMSelectable> Buttons;
    public TextMesh[] Text;
    public KMBombModule Module;

    //private stuff
    private bool _solved;
    private List<int> _solution = new List<int> { };
    private List<int> _numbers = new List<int> { };
    private List<int> _selected = new List<int> { };

    private List<User> _chosenUsers;


    private static readonly User[] _users =
    {
        new User("Rdzanu", "FFC000", 731, number => number.ToString().Count(x => "47".Contains(x)) % 2 == 1),
        new User("AnAverageArceus", "F0D149", 366, number => DistinctPrimeCount(number) % 2 == 1),
        new User("meh", "577D26", 243, number => IsPerfectPower(number)),
        new User("BomberJack", "00BFBA", 394, number => number.ToString().Distinct().Count() <= 2),
        new User("Asmir", "00FFFF", 818, number => Enumerable.Range(0, number.ToString().Length - 1).All(x => LowHighParityOdd(number.ToString(), x, x + 1))),
        new User("Eltrick", "308BBE", 956, number => number.ToString().Count(x => "069".Contains(x)) % 2 == 0),
        new User("gwendolyn", "0080FF", 597, number => Enumerable.Range(0, number.ToString().Length).Any(x => IsStrictlyAscending(RemoveDigit(number.ToString(), x)))),
        new User("GhostSalt", "EAEBEC", 614, number => number % 16 >= 8),
        new User("Lord Kabewm", "002ABA", 234, number => ((number + 8) % 9 + 1) % 2 == 0),
        new User("Cooldoom5", "000001", 248, number => IsDecomposableInto(number.ToString(), LucasNumbersUpTo(number).Select(x => x.ToString()).ToArray())),
        new User("Danielstigman", "1F1E33", 727, number => number.ToString().All(x => number.ToString().All(y => y % 2 == x % 2))),
        new User("Obvious", "9080C0", 383, number => number % 2 == 1),
        new User("MásQuéÉlite", "7700FF", 633, number => BaseDigitCount(number, 3, 2) % 2 == 0),
        new User("Shadow Meow", "9400D3", 808, number => number % 10 == 7 || number % 7 == 0),
        new User("tandyCake", "FF8AFF", 407, number => DigitDifference(number.ToString(), 0, 1) == DigitDifference(number.ToString(), number.ToString().Length - 2, number.ToString().Length - 1)),
        new User("Dicey", "ff40D0", 679, number => number % 3 == 0),
    };

    private int input;

    //logging
    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        for (int i = 0; i < 11; i++)
        {
            int x = i;
            Buttons[i].OnHighlight += delegate { if (!_solved) { Buttons[x].GetComponent<MeshRenderer>().material.color = new Color32(64, 66, 73, 255); } };
            Buttons[i].OnHighlightEnded += delegate { Buttons[x].GetComponent<MeshRenderer>().material.color = new Color32(49, 51, 56, 255); };
            Buttons[i].OnInteract += delegate
            {
                if (!_solved)
                {
                    Audio.PlaySoundAtTransform("Tap", Module.transform);
                    if (x == 10)
                    {
                        CheckSolve();
                    }
                    else
                    {
                        input *= 10;
                        input += x;
                        input %= 100000;
                    }
                    if (!_solved)
                    {
                        Text[0].text = _numbers.Select(y => y == 0 ? "    0" : Enumerable.Repeat(" ", 5 - y.ToString().Length).Join("") + y.ToString()).Join("\n") + "\n" + Enumerable.Repeat(" ", 5 - input.ToString().Length).Join("") + input;
                        Text[1].text = "<b>ONLINE-3</b>\n\n" + _chosenUsers.Select(y => (y.Name == _chosenUsers[_selected[_numbers.Count()]].Name ? ">" : " ") + "<color='#" + y.HexCode + "'>" + y.Name.Substring(0, new int[] { 9, y.Name.Length }.Min()) + (y.Name.Length > 9 ? ".." : "") + "</color>").Join("\n");
                    }
                    else
                    {
                        Text[0].text = _numbers.Select(y => y == 0 ? "    0" : Enumerable.Repeat(" ", 5 - y.ToString().Length).Join("") + y.ToString()).Join("\n") + "\n<color='#" + (_solution.Any(y => y.ToString().Contains("727")) ? "135A8F" : "60E060") + "'>" + (_solution.Any(y => y.ToString().Contains("727")) ? "WYSI" : "Poggers!") + "</color>";
                        Text[1].text = "<b>ONLINE-0</b>";
                    }
                }
                return false;
            };
        }
    }

    void Start()
    {
        GenerateSolution();
        Text[0].text = Enumerable.Repeat(" ", 5 - _numbers.First().ToString().Length).Join("") + _numbers.First();
        Text[1].text = "<b>ONLINE-3</b>\n\n" + _chosenUsers.Select(x => (x.Name == _chosenUsers[_selected[1]].Name ? ">" : " ") + "<color='#" + x.HexCode + "'>" + x.Name.Substring(0, new int[] { 9, x.Name.Length }.Min()) + (x.Name.Length > 9 ? ".." : "") + "</color>").Join("\n");
    }

    private void GenerateSolution()
    {
        while (true)
        {
            _solution = new List<int> { };
            _numbers = new List<int> { };
            _selected = new List<int> { };

            int n = Rnd.Range(10, 69420);

            _numbers.Add(n);
            _solution.Add(n);
            _selected.Add(-1);

            _chosenUsers = _users.ToList().Shuffle().Take(3).ToList();
            Debug.LogFormat("[Count to 69420 #{0}] Online: {1}.", _moduleID, _chosenUsers.Select(x => x.Name).Join(", "));

            List<int> latest = new List<int> { 0, 0, 0 };
            for (int i = 0; i < 5; i++)
            {
                _selected.Add(Rnd.Range(0, 3));
                while (_solution.Count() < _selected.Count())
                {
                    n++;
                    List<int> candidates = new List<int> { };
                    for (int j = 0; j < 3; j++)
                        if (latest[j] != n - 1 && _chosenUsers[j].Likes(n))
                            candidates.Add(j);
                    if (candidates.Count() >= 1)
                    {
                        latest[candidates.OrderBy(x => n % _chosenUsers[x].Ping).First()] = n;
                        if (candidates.OrderBy(x => n % _chosenUsers[x].Ping).First() == _selected.Last())
                            _solution.Add(n);
                    }
                    else
                    {
                        for (int j = 0; j < 3; j++)
                            if (latest[j] != n - 1 && !_chosenUsers[j].Likes(n + 1))
                                candidates.Add(j);
                        if (candidates.Count() >= 1)
                        {
                            latest[candidates.OrderBy(x => n % _chosenUsers[x].Ping).First()] = n;
                            if (candidates.OrderBy(x => n % _chosenUsers[x].Ping).First() == _selected.Last())
                                _solution.Add(n);
                        }
                        else
                        {
                            for (int j = 0; j < 3; j++)
                                if (latest[j] != n - 1)
                                    candidates.Add(j);
                            latest[candidates.OrderBy(x => n % _chosenUsers[x].Ping).First()] = n;
                            if (candidates.OrderBy(x => n % _chosenUsers[x].Ping).First() == _selected.Last())
                                _solution.Add(n);
                        }
                    }
                    Debug.LogFormat("[Count to 69420 #{0}] {1}", _moduleID, (_solution.Contains(n) ? "!!! " : "") + _chosenUsers[latest.IndexOf(n)].Name + ": " + n);
                }
            }
            if (n > 69420)
            {
                Debug.LogFormat("[Count to 69420 #{0}] The number crossed 69420, which is amazing, but we don't have to count further do we?", _moduleID);
                continue;
            }
            if (n - _numbers.First() > 25)
            {
                Debug.LogFormat("[Count to 69420 #{0}] We got more than 25 numbers further, which is great, but let's have some mercy on the player", _moduleID);
                continue;
            }

            Debug.LogFormat("[Count to 69420 #{0}] Answers are: {1}.", _moduleID, _solution.First() + "; " + Enumerable.Range(1, 5).Select(x => _chosenUsers[_selected[x]].Name + ": " + _solution[x]).Join("; "));
            break;
        }
    }

    private void CheckSolve()
    {
        if (_solution[_numbers.Count()] == input)
        {
            Debug.LogFormat("[Count to 69420 #{0}] You submitted {1}, which is correct!", _moduleID, input);
            _numbers.Add(input);
            input = 0;
        }
        else
        {
            Debug.LogFormat("[Count to 69420 #{0}] You submitted {1} but I expected {2}. Your message will be deleted, but you will still get a strike!", _moduleID, input, _solution[_numbers.Count()]);
            Module.HandleStrike();
        }
        if (_numbers.Count() == _solution.Count())
        {
            Debug.LogFormat("[Count to 69420 #{0}] Module solved!", _moduleID);
            Module.HandlePass();
            _solved = true;
            foreach (var button in Buttons)
            {
                button.GetComponent<MeshRenderer>().material.color = new Color32(49, 51, 56, 255);
                button.GetComponentInChildren<TextMesh>().text = "";
            }
        }
    }

    private static int DistinctPrimeCount(int n)
    {
        int j = 0;
        for (int i = 2; i <= n; i++)
            if (n % i == 0)
            {
                j++;
                while (n % i == 0)
                    n /= i;
            }
        return j;
    }

    private static bool IsPerfectPower(int n)
    {
        bool good = false;
        for (int i = 2; i * i <= n; i++)
            for (int j = 1; j <= n; j *= i)
                good |= j == n;
        return good;
    }

    private static int BaseDigitCount(int n, int b, int d)
    {
        int i = 0;
        while (n > 0)
        {
            if (n % b == d)
                i++;
            n /= b;
        }
        return i;
    }

    private static int DigitDifference(string n, int a, int b)
    {
        return Math.Abs(n[a] - n[b]);
    }

    private static bool LowHighParityOdd(string n, int a, int b)
    {
        return (n[a] < '5') ^ (n[b] < '5');
    }

    private static bool IsStrictlyAscending(string n)
    {
        for (int i = 0; i < n.Length - 1; i++)
            if (n[i + 1] <= n[i])
                return false;

        return true;
    }

    private static string RemoveDigit(string n, int d)
    {
        return n.Substring(0, d) + n.Substring(d + 1);
    }

    public static int[] LucasNumbersUpTo(int upper)
    {
        List<int> numbers = new List<int> { 2 };
        int sumOfLastTwo = 1;
        do
        {
            numbers.Add(sumOfLastTwo);
            sumOfLastTwo = numbers[numbers.Count - 1] + numbers[numbers.Count - 2];
        }
        while (sumOfLastTwo <= upper);

        return numbers.ToArray();
    }

    public static bool IsDecomposableInto(string n, string[] parts)
    {
        Queue<string> compositions = new Queue<string>();
        compositions.Enqueue("");

        while (compositions.Count > 0)
        {
            string item = compositions.Dequeue();
            foreach (string part in parts)
            {
                string newComposition = item + part;
                if (newComposition == n)
                    return true;

                if (newComposition.Length < n.Length && PartialEquals(newComposition, n))
                    compositions.Enqueue(newComposition);
            }
        }

        return false;
    }

    private static bool PartialEquals(string s1, string s2)
    {
        for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            if (s1[i] != s2[i])
                return false;

        return true;
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "'!{0} enter 69' to enter that number.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        yield return null;
        command = command.ToLowerInvariant();
        if (Regex.IsMatch(command, @"^enter\s\d{1,5}$"))
        {
            MatchCollection matches = Regex.Matches(command, @"\s(\d+)$");
            foreach (Match match in matches)
            {
                Debug.Log(match.ToString());
                string subcmd = match.ToString();
                subcmd = (int.Parse(subcmd)).ToString("00000");
                for (int i = 0; i < 5; i++)
                {
                    Buttons[subcmd[i] - '0'].OnInteract();
                    yield return null;
                }
                Buttons[10].OnInteract();
                yield return null;
            }
            yield return "solve";
        }
        else
            yield return "sendtochaterror Invalid command.";
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!_solved)
        {
            string cmd = _solution[_numbers.Count()].ToString("00000");
            for (int i = 0; i < 5; i++)
            {
                Buttons[cmd[i] - '0'].OnInteract();
                yield return new WaitForSeconds(0.05f);
            }
            Buttons[10].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
