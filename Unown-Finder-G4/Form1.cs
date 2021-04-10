using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unown_Finder_G4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public List<string> characters = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "R",
            "S",
            "T",
            "U",
            "V",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "W",
            "X",
            "Y",
            "Z",
            "!",
            "?"
        };
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            List<bool> characterCheck = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            int characterCount = 0;
            if (AtoJCheck.Checked)
            {
                for (int i = 0; i <= 9; i++)
                {
                    characterCheck[i] = true;
                    characterCount++;
                }
            }
            if (RtoVCheck.Checked)
            {
                for (int i = 15; i <= 21; i++)
                {
                    characterCheck[i] = true;
                    characterCount++;
                }
            }
            if (KtoQCheck.Checked)
            {
                for (int i = 10; i <= 14; i++)
                {
                    characterCheck[i] = true;
                    characterCount++;
                }
            }
            if (WtoZCheck.Checked)
            {
                for (int i = 22; i <= 25; i++)
                {
                    characterCheck[i] = true;
                    characterCount++;

                }
            }
            uint initial;
            try
            {
                initial = Convert.ToUInt32(InitialSeedTB.Text, 16);
            }
            catch
            {
                MessageBox.Show("Error: Seed has not been entered properly, please fix this if you want results.");
                return;
            }
            if (characterCount == 0)
            {
                MessageBox.Show("Error: You have not unlocked any letters.");
                return;
            }
           
            PokeRNG rng = new PokeRNG(initial);
            uint startingFrame, maxFrames;
            try
            {
                startingFrame = (uint)StartingFrameNUD.Value;
                maxFrames = (uint)AmountofFramesNUD.Value;
            }
            catch
            {
                MessageBox.Show("Error: Starting Frame and/or Frame Amount have not been entered properly, please fix this if you want results.");
                return;
            }
            List<decimal> minIVs = new List<decimal> { HPMinNUD.Value, AtkMinNUD.Value, DefMinNUD.Value, SpAMinNUD.Value, SpDMinNUD.Value, SpeMinNUD.Value };
            List<decimal> maxIVs = new List<decimal> { HPMaxNUD.Value, AtkMaxNUD.Value, DefMaxNUD.Value, SpAMaxNUD.Value, SpDMaxNUD.Value, SpeMaxNUD.Value };
            int delay = 0;

            if (DelayCheck.Checked)
            {
                try
                {
                    delay = (int)DelayNUD.Value;
                }
                catch
                {
                    MessageBox.Show("Error: Delay has not been entered properly, please fix this if you want it to impact the frames or uncheck the Delay box.");
                }
            }

            for (uint f = 1; f < startingFrame + delay; f++)
            {
                rng.nextUInt();
            }
            uint tid, sid, tsv, txor;
            try
            {
                tid = (uint)TIDNUD.Value;
                sid = (uint)SIDNUD.Value;
                txor = tid ^ sid;
                tsv = txor / 8;
            }
            catch
            {
                tsv = 8193;
                txor = 8193;
                MessageBox.Show("Error: TID/SID have not been entered properly, please fix this if you want shinies to be marked correctly.");
            }

            string selectedLetter;
            int selectedNature, selectedLetterTest;


            selectedLetter = FormCB.Text;
            selectedLetterTest = characters.IndexOf(selectedLetter);

            if (selectedLetterTest < 0 & FormCheck.Checked & FormCB.Text != "Any")
            {
                MessageBox.Show("Error: Chosen Form has not been entered properly, please fix this or uncheck the Form box if you want results.");
                return;
            }

            selectedNature = natures.IndexOf(NatureCB.Text);
            if (selectedNature < 0 & NatureCheck.Checked & NatureCB.Text != "Any")
            {
                MessageBox.Show("Error: Chosen Nature has not been entered properly, please fix this or uncheck the Nature box if you want results.");
                return;
            }

            for (int j = 0;  j < 6; j++)
            {
                if (maxIVs[j] < minIVs[j] | minIVs[j] > maxIVs[j])
                {
                    MessageBox.Show("Error: IV Range has not been entered properly.");
                    break;
                }
            }
            List<bool> seen = new List<bool> { ACheck.Checked, BCheck.Checked, CCheck.Checked, DCheck.Checked, ECheck.Checked, FCheck.Checked, GCheck.Checked, HCheck.Checked, ICheck.Checked, JCheck.Checked, KCheck.Checked, LCheck.Checked, MCheck.Checked, NCheck.Checked, OCheck.Checked, PCheck.Checked, QCheck.Checked, RCheck.Checked, SCheck.Checked, TCheck.Checked, UCheck.Checked, VCheck.Checked, WCheck.Checked, XCheck.Checked, YCheck.Checked, ZCheck.Checked };

            for (uint cnt = 0; cnt < maxFrames + delay; cnt++, rng.nextUInt())
            {
                int seenCount = 0;
                for (int i = 0; i <= 25; i++)
                {
                    if (characterCheck[i] && !seen[i])
                    {
                        seenCount++;
                    }
                }
                bool flag = true;
                uint seed = rng.seed;
                List<bool> seenGo = new List<bool>(seen);
                PokeRNG go = new PokeRNG(seed);
                ushort low = go.nextUShort();
                ushort high = go.nextUShort();
                ushort psv = (ushort)((low ^ high) / 8);
                bool shiny = tsv == psv;
                bool square = ((low ^ high) == txor) & shiny;
                uint pid = (uint)(high << 16) | low;
                ushort iv1 = go.nextUShort();
                ushort iv2 = go.nextUShort();
                List<uint> ivs = GetIVs(iv1, iv2);
                go.nextUInt();
                ushort letterRand = go.nextUShort();
                string mainLetter = characters[letterRand % (characterCount)];
                string qeLetter = characters[(letterRand % 2) + characterCount];
                string nature = natures[(int)(pid % 25)];
                bool radio = letterRand % 100 < 50;
                string radioLetters = "";
                letterRand = go.nextUShort();

                if (radio)
                {
                    for (seenCount = seenCount; seenCount > 0; seenCount--)
                    {
                        int letterValue = letterRand % seenCount;
                        int letterCheck = 0;
                        for (int i = 0; i <= 25; i++)
                        {
                            if (characterCheck[i] && !seenGo[i])
                            {
                                if (letterValue == letterCheck)
                                {
                                    radioLetters += characters[i];
                                    seenGo[i] = true;
                                    break;
                                }
                                letterCheck++;
                            }
                        }
                    }
                } else
                {
                    int letterValue = letterRand % characterCount;
                    int letterCheck = 0;
                    for (int i = 0; i <= 25; i++)
                    {
                        if (characterCheck[i])
                        {
                            if (letterValue == letterCheck)
                            {
                                radioLetters = characters[i];
                                break;
                            }
                            letterCheck++;
                        }
                    }
                }


                if (ShinyCheck.Checked & !(ShinyCB.Text == "Any"))
                {
                    if (ShinyCB.Text == "Star" & (!shiny | square))
                    {
                        flag = false;
                    } else if (ShinyCB.Text == "Square" & !square)
                    {
                        flag = false;
                    } else if (ShinyCB.Text == "Star/Square" & !shiny)
                    {
                        flag = false;
                    }
                }


                if (FormCheck.Checked & !(FormCB.Text == "Any") & !(selectedLetter == mainLetter) & !(selectedLetter == qeLetter))
                {
                    flag = false;
                }

                if (NatureCheck.Checked & !(NatureCB.Text == "Any") & !(selectedNature == pid%25))
                {
                    flag = false;
                }

                for (int i = 0; i < 6; i++)
                {
                    if (ivs[i] < minIVs[i] | ivs[i] > maxIVs[i])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    dataGridView1.Rows.Add(cnt + startingFrame, getCall(low), getChatot(low), pid.ToString("X"), square ? "Square" : shiny ? "Star" : "No", nature, ivs[0], ivs[1], ivs[2], ivs[3], ivs[4], ivs[5], mainLetter, radio ? "O" : "X", radioLetters, qeLetter, GetHPowerType(ivs), GetHPowerDamage(ivs));
                }
            }



        }
        public List<string> natures = new List<string> {
                "Hardy","Lonely","Brave","Adamant","Naughty",
                "Bold","Docile","Relaxed","Impish","Lax",
                "Timid","Hasty","Serious","Jolly","Naive",
                "Modest","Mild","Quiet","Bashful","Rash",
                "Calm","Gentle","Sassy","Careful","Quirky"};
        public List<string> hpowertypes = new List<string> { "Fighting", "Flying", "Poison",
                                                             "Ground", "Rock", "Bug",
                                                             "Ghost", "Steel", "Fire",
                                                             "Water", "Grass", "Electric",
                                                             "Psychic", "Ice", "Dragon",
                                                                        "Dark" };


        private List<uint> GetIVs(uint iv1, uint iv2)
        {
            uint hp = iv1 & 0x1f;
            uint atk = (iv1 >> 5) & 0x1f;
            uint defense = (iv1 >> 10) & 0x1f;
            uint spa = (iv2 >> 5) & 0x1f;
            uint spd = (iv2 >> 10) & 0x1f;
            uint spe = iv2 & 0x1f;
            return new List<uint> { hp, atk, defense, spa, spd, spe };
        }

        private string GetHPowerType(List<uint> ivs)
        {
            uint a, b, c, d, e, f;

            a = ivs[0] & 1;
            b = ivs[1] & 1;
            c = ivs[2] & 1;
            d = ivs[5] & 1;
            e = ivs[3] & 1;
            f = ivs[4] & 1;

            return hpowertypes[(int)(((a + 2 * b + 4 * c + 8 * d + 16 * e + 32 * f) * 15) / 63)];
        }

        private uint GetHPowerDamage(List<uint> ivs)
        {
            uint u, v, w, x, y, z;

            u = (ivs[0] >> 1) & 1;
            v = (ivs[1] >> 1) & 1;
            w = (ivs[2] >> 1) & 1;
            x = (ivs[5] >> 1) & 1;
            y = (ivs[3] >> 1) & 1;
            z = (ivs[4] >> 1) & 1;

            return ((u + 2 * v + 4 * w + 8 * x + 16 * y + 32 * z) * 40) / 63 + 30;

        }

        private string getChatot(uint seed)
        {
            return getPitch((byte)(((seed & 0x1fff) * 100) >> 13));
        }

        private string getPitch(byte result)
        {
            string pitch;
            if (result < 20)
            {
                pitch = "L ";
            }
            else if (result < 40)
            {
                pitch = "ML ";
            }
            else if (result < 60)
            {
                pitch = "M ";
            }
            else if (result < 80)
            {
                pitch = "MH ";
            }
            else
            {
                pitch = "H ";
            }

            return pitch + result.ToString();
        }
        private string getCall(uint seed)
        {
            byte call = (byte)(seed % 3);
            return call == 0 ? "E" : call == 1 ? "K" : "P";
        }
    }
}
