using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class countToFunnyScript : MonoBehaviour
{

    //public stuff
    public KMAudio Audio;
    public List<KMSelectable> Buttons;
    public TextMesh[] Text;
    public KMBombModule Module;

    //private stuff
    private bool solved;
    private List<int> solution = new List<int> { };
    private List<int> numbers = new List<int> { };
    private List<int> users;
    private List<int> selected = new List<int> { };
    private readonly List<string> usernames = new List<string> { "Arizona Ranger", "Decateron Shanpe", "SolidPepper", "Protogen God", "Lasagna Lover", "Lasagna Hater", "Hyperboloid", "worst expert ever :(", "Cyberpunk Tesla", "Left-Wing Media Agent", "Big Thonker", "what", "Kanye Not East", "Atlanta", "void" };
    private readonly List<string> hex = new List<string> { "002ABA", "3F3FbF", "EAEBEC", "F0D149", "3F3FbF", "EAEBEC", "EAEBEC", "577D26", "607D8B", "FF8AFF", "ff0000", "ff0000", "002ABA", "F0D149", "000000" };
    private readonly List<int> pings = new List<int> { 234, 136, 614, 731, 633, 366, 394, 243, 998, 407, 818, 956, 808, 864, -1 };
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
            Buttons[i].OnHighlight += delegate { if (!solved) { Buttons[x].GetComponent<MeshRenderer>().material.color = new Color32(52, 57, 62, 255); } };
            Buttons[i].OnHighlightEnded += delegate { Buttons[x].GetComponent<MeshRenderer>().material.color = new Color32(44, 47, 51, 255); };
            Buttons[i].OnInteract += delegate
            {
                if (!solved)
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
                        input %= 10000;
                    }
                    if (!solved)
                    {
                        Text[0].text = numbers.Join("\n") + "\n" + input;
                        Text[1].text = "<b>ONLINE-3</b>\n\n" + users.Select(y => (y == users[selected[numbers.Count()]] ? ">" : " ") + "<color='#" + hex[y] + "'>" + usernames[y].Substring(0, new int[] { 8, usernames[y].Length }.Min()) + (usernames[y].Length > 8 ? "..." : "") + "</color>").Join("\n");
                    }
                    else
                    {
                        Text[0].text = numbers.Join("\n") + "\n<color='#00ff00'>Poggers!</color>";
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
        Text[0].text = numbers.First() + "\n0";
        Text[1].text = "<b>ONLINE-3</b>\n\n" + users.Select(x => (x == users[selected[1]] ? ">" : " ") + "<color='#" + hex[x] + "'>" + usernames[x].Substring(0, new int[] { 8, usernames[x].Length }.Min()) + (usernames[x].Length > 8 ? "..." : "") + "</color>").Join("\n");
    }

    private void GenerateSolution()
    {
        regen:
        solution = new List<int> { };
        numbers = new List<int> { };
        selected = new List<int> { };
        int n = Rnd.Range(1000, 5000);
        numbers.Add(n);
        solution.Add(n);
        selected.Add(-1);
        users = Enumerable.Range(0, usernames.Count()).ToList().Shuffle().Take(3).ToList();
        Debug.LogFormat("[Count to 69420 #{0}] Online: {1}.", _moduleID, users.Select(x => usernames[x]).Join(", "));
        List<int> latest = new List<int> { 0, 0, 0 };
        for (int i = 0; i < 5; i++)
        {
            selected.Add(Rnd.Range(0, 3));
            if (users[selected.Last()] == 14)
            {
                solution.Add(0);
            }
            else
            {
                while (solution.Count() < selected.Count())
                {
                    n++;
                    List<int> candidates = new List<int> { };
                    for (int j = 0; j < 3; j++)
                        if (latest[j] != n - 1 && Likes(users[j], n))
                            candidates.Add(j);
                    if (candidates.Count() >= 1)
                    {
                        //pog
                        latest[candidates.OrderBy(x => n % pings[users[x]]).First()] = n;
                        if (candidates.OrderBy(x => n % pings[users[x]]).First() == selected.Last())
                            solution.Add(n);
                    }
                    else
                    {
                        //less pog
                        for (int j = 0; j < 3; j++)
                            if (latest[j] != n - 1 && !Likes(users[j], n + 1) && users[j] != 14)
                                candidates.Add(j);
                        if (candidates.Count() >= 1)
                        {
                            //still pog
                            latest[candidates.OrderBy(x => n % pings[users[x]]).First()] = n;
                            if (candidates.OrderBy(x => n % pings[users[x]]).First() == selected.Last())
                                solution.Add(n);
                        }
                        else
                        {
                            //unpog
                            for (int j = 0; j < 3; j++)
                                if (latest[j] != n - 1 && users[j] != 14)
                                    candidates.Add(j);
                            latest[candidates.OrderBy(x => n % pings[users[x]]).First()] = n;
                            if (candidates.OrderBy(x => n % pings[users[x]]).First() == selected.Last())
                                solution.Add(n);
                        }
                    }
                    Debug.LogFormat("[Count to 69420 #{0}] {1}", _moduleID, (solution.Contains(n) ? "!!! " : "") + usernames[users[latest.IndexOf(n)]] + ": " + n);
                }
            }
        }
        if(n >= 10000)
        {
            Debug.LogFormat("[Count to 69420 #{0}] The number crossed 10000, which is great, but not supported :(", _moduleID);
            goto regen;
        }
        Debug.LogFormat("[Count to 69420 #{0}] Answers are: {1}.", _moduleID, solution.First() + "; " + Enumerable.Range(1, 5).Select(x => usernames[users[selected[x]]] + ": " + solution[x]).Join("; "));
    }

    private void CheckSolve()
    {
        if (solution[numbers.Count()] == input)
        {
            Debug.LogFormat("[Count to 69420 #{0}] You submitted {1}, which is correct!", _moduleID, input);
            numbers.Add(input);
            input = 0;
        }
        else
        {
            Debug.LogFormat("[Count to 69420 #{0}] You submitted {1} but I expected {2}. Your message will be deleted, but you will still get a strike!", _moduleID, input, solution[numbers.Count()]);
            Module.HandleStrike();
        }
        if (numbers.Count() == solution.Count())
        {
            Debug.LogFormat("[Count to 69420 #{0}] Module solved!", _moduleID);
            Module.HandlePass();
            solved = true;
            foreach (var button in Buttons)
            {
                button.GetComponent<MeshRenderer>().material.color = new Color32(44, 47, 51, 255);
                button.GetComponentInChildren<TextMesh>().text = "";
            }
        }
    }

    private bool IsPrime(int n)
    {
        bool good = true;
        for (int i = 2; i < n && good; i++)
            if (n % i == 0)
                good = false;
        return good;
    }

    private bool IsSquare(int n)
    {
        bool good = false;
        for (int i = 0; i * i <= n; i++)
            good = i * i == n;
        return good;
    }

    private bool Likes(int user, int n)
    {
        switch (user)
        {
            case 0:
                return (n % 2 == 0);
            case 1:
                return (n % 2 == 1);
            case 2:
                return (new int[] { 6, 9 }.Contains(n % 10));
            case 3:
                return (IsPrime(n));
            case 4:
                return (n % 5 != 0);
            case 5:
                return (n % 5 == 0);
            case 6:
                return (n.ToString()[0] == n.ToString()[3] && n.ToString()[1] == n.ToString()[2]);
            case 7:
                return (IsSquare(n));
            case 8:
                return (!n.ToString().Contains('0'));
            case 9:
                return (new int[] { 1234, 2345, 3456, 4567, 5678, 6789 }.Contains(n));
            case 10:
                return (n.ToString().Contains('0') && n.ToString().Contains('5'));
            case 11:
                return (!(n.ToString().Contains('0') || n.ToString().Contains('3') || n.ToString().Contains('6') || n.ToString().Contains('9')));
            case 12:
                return (!(n.ToString().Contains('7') || n.ToString().Contains('8') || n.ToString().Contains('9')));
            case 13:
                return (n % 51 == 0);
            case 14:
                return false;
        }
        return false;
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "'!{0} enter 69' to enter that number.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        yield return null;
        command = command.ToLowerInvariant();
        if (Regex.IsMatch(command, @"^enter\s(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)$"))
        {
            MatchCollection matches = Regex.Matches(command, @"(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)?(0|1|2|3|4|5|6|7|8|9)");
            foreach (Match match in matches)
            {
                Debug.Log(match.ToString());
                string subcmd = match.ToString();
                subcmd = (int.Parse(subcmd)).ToString("0000");
                for (int i = 0; i < 4; i++)
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
        yield return true;
        while (!solved)
        {
            string cmd = solution[numbers.Count()].ToString("0000");
            for (int i = 0; i < 4; i++)
            {
                Buttons[cmd[i] - '0'].OnInteract();
                yield return true;
            }
            Buttons[10].OnInteract();
            yield return true;
        }
    }
}
