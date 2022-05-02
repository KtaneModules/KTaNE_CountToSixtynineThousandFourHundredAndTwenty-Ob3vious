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
    private readonly List<string> usernames = new List<string> { "Lord Kabewm", "Obvious", "GhostSalt", "Rdzanu", "MásQuéÉlite", "AnAverageArceus", "BomberJack", "meh", "Danielstigman", "tandyCake", "Asmir", "Eltrick", "Shadow Meow", "Cooldoom5" };
    private readonly List<string> hex = new List<string> { "002ABA", "9080c0", "EAEBEC", "FFC000", "7700FF", "F0D149", "00BFBA", "577D26", "1F1E33", "FF8AFF", "00FFFF", "308BBE", "9400D3", "000000" };
    private readonly List<int> pings = new List<int> { 234, 136, 614, 731, 633, 366, 394, 243, 998, 407, 818, 956, 808, -1 };
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
                        Text[0].text = numbers.Select(y => y == 0 ? "   0" : y.ToString()).Join("\n") + "\n" + Enumerable.Repeat(" ", 4 - input.ToString().Length).Join("") + input;
                        Text[1].text = "<b>ONLINE-3</b>\n\n" + users.Select(y => (y == users[selected[numbers.Count()]] ? ">" : " ") + "<color='#" + hex[y] + "'>" + usernames[y].Substring(0, new int[] { 9, usernames[y].Length }.Min()) + (usernames[y].Length > 9 ? ".." : "") + "</color>").Join("\n");
                    }
                    else
                    {
                        Text[0].text = numbers.Select(y => y == 0 ? "   0" : y.ToString()).Join("\n") + "\n<color='#00ff00'>Poggers!</color>";
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
        Text[0].text = numbers.First() + "\n   0";
        Text[1].text = "<b>ONLINE-3</b>\n\n" + users.Select(x => (x == users[selected[1]] ? ">" : " ") + "<color='#" + hex[x] + "'>" + usernames[x].Substring(0, new int[] { 9, usernames[x].Length }.Min()) + (usernames[x].Length > 9 ? ".." : "") + "</color>").Join("\n");
    }

    private void GenerateSolution()
    {
        regen:
        solution = new List<int> { };
        numbers = new List<int> { };
        selected = new List<int> { };
        int n = Rnd.Range(1000, 10000);
        numbers.Add(n);
        solution.Add(n);
        selected.Add(-1);
        users = Enumerable.Range(0, usernames.Count()).ToList().Shuffle().Take(3).ToList();
        Debug.LogFormat("[Count to 69420 #{0}] Online: {1}.", _moduleID, users.Select(x => usernames[x]).Join(", "));
        List<int> latest = new List<int> { 0, 0, 0 };
        for (int i = 0; i < 5; i++)
        {
            selected.Add(Rnd.Range(0, 3));
            if (users[selected.Last()] == 13)
                solution.Add(0);
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
                            if (latest[j] != n - 1 && !Likes(users[j], n + 1) && users[j] != 13)
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
                                if (latest[j] != n - 1 && users[j] != 13)
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
        else if (n - numbers.First() > 25)
        {
            Debug.LogFormat("[Count to 69420 #{0}] We got more than 100 numbers further, which is great, but let's have some mercy on the player", _moduleID);
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

    private int DistinctPrimeCount(int n)
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

    private bool IsPower(int n)
    {
        bool good = false;
        for (int i = 2; i * i <= n; i++)
            for (int j = 1; j <= n; j *= i)
                good |= j == n;
        return good;
    }

    private int TernaryTwos(int n)
    {
        int i = 0;
        while (n > 0)
        {
            if (n % 3 == 2)
                i++;
            n /= 3;
        }
        return i;
    }

    private bool Likes(int user, int n)
    {
        switch (user)
        {
            case 0: //Kabewm
                return (((n + 8) % 9) % 2 == 1);
            case 1: //Obvi
                return (n % 2 == 1);
            case 2: //Ghost
                return (n % 16 >= 8);
            case 3: //zanu
                return (n.ToString().Count(x => "47".Contains(x)) % 2 == 1);
            case 4: //MasQue
                return (TernaryTwos(n) % 2 == 0);
            case 5: //Arc
                return (DistinctPrimeCount(n) % 2 == 1);
            case 6: //Jack
                return (n.ToString().Distinct().Count() <= 2);
            case 7: //meh
                return (IsPower(n));
            case 8: //Dan
                return (n.ToString().All(x => "02468".Contains(x)) || n.ToString().All(x => "13579".Contains(x)));
            case 9: //Danny
                return (Math.Abs(n.ToString()[0] - n.ToString()[1]) == Math.Abs(n.ToString()[2] - n.ToString()[3]));
            case 10: //Asmir
                return (Enumerable.Range(0, 4).All(x => x % 2 != (n.ToString()[x] - '0') / 5) || Enumerable.Range(0, 4).All(x => x % 2 == (n.ToString()[x] - '0') / 5));
            case 11: //Eltrick
                return (n.ToString().Count(x => "069".Contains(x)) % 2 == 0);
            case 12: //Shadow
                return (n % 10 == 7 || n % 7 == 0);
            case 13: //void
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
        while (!solved)
        {
            string cmd = solution[numbers.Count()].ToString("0000");
            for (int i = 0; i < 4; i++)
            {
                Buttons[cmd[i] - '0'].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            Buttons[10].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
