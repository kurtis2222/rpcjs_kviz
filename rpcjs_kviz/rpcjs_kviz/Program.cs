using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace rpcjs_kviz
{
    class Form1 : Form
    {
        public const string game_name = "Rossz PC Játékok Sorozat Kvíz";

        public const string file_qstn = "kerdesek.dat";
        public const string file_pics = "kepek.dat";
        const string file_score = "pontok.dat";
        const int MAX_ANS = 4;
        const int MAX_SCORE = 10;

        const int orig_w = 640;
        const int orig_h = 480;
        const int half_w = 320;
        const int half_h = 240;
        float sc_x, sc_y;

        Label lb_title;
        Label lb_sep;
        //Főmenü
        Button bt_easy, bt_med, bt_hard, bt_rnd, bt_score, bt_quit;
        //Csaló mód
        Label lb_cheat;
        Button bt_cheat;
        //Pontlista
        Label lb_pos, lb_names, lb_score;
        byte score_pos;
        string[] pl_names = new string[MAX_SCORE];
        ushort[] pl_scores = new ushort[MAX_SCORE];
        //Pontlista input
        Label lb_scinput;
        TextBox tb_scinput;
        //Betű
        Font ft_qt, ft_bt;
        //Kvíz elemek
        Label lb_qsep;
        Label lb_qstn;
        Label lb_qnum;
        Label lb_pts, lb_ptsnum;
        Label lb_wrong;
        Control ct_img;
        Button[] bt_ans;
        Button bt_menu;
        //Kvízkezelés
        byte count_qstn;
        byte count_imgs;
        int rnd_min, rnd_max;
        BinaryReader br_qstn, br_pics;
        BinaryReader br_score;
        BinaryWriter bw_score;
        Random rnd;
        uint qnum, len;
        byte[] list_qstn;
        string tmpstr;
        string[] tmp;
        byte imgid;
        byte answer;
        byte pl_count, pl_qstn;
        byte[] imgdata;
        //Csalás
        byte cheat = 0;
        //Befejező képernyő
        Control ct_end;
        Control ct_cheat;
        Label lb_end;
        //Pontok
        ushort points;
        ushort pts_mul;

        public Form1()
        {
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            //Főbb formázások
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
            Text = game_name;
            BackColor = Color.DarkBlue;
            ForeColor = Color.White;
            Application.ThreadException += Application_ThreadException;
            sc_x = (float)Width / orig_w;
            sc_y = (float)Height / orig_h;
            ft_bt = new Font(FontFamily.GenericSerif, sc_y * 12, FontStyle.Bold);
            ft_qt = new Font(FontFamily.GenericSerif, sc_y * 10, FontStyle.Bold);
            //Fejléc felirat
            lb_title = new Label();
            lb_title.Text = game_name;
            lb_title.Font = new Font(Font.FontFamily, sc_y * 24, FontStyle.Bold);
            lb_title.Bounds = ScaleElement(0, 0, orig_w, 64);
            lb_title.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lb_title);
            lb_sep = new Label();
            lb_sep.BorderStyle = BorderStyle.Fixed3D;
            lb_sep.Bounds = ScaleElement(0, 64, orig_w, 1);
            Controls.Add(lb_sep);
            //Főmenü
            bt_easy = new Button();
            bt_easy.Text = "Könnyű";
            bt_easy.Font = ft_bt;
            bt_easy.BackColor = Color.Green;
            bt_easy.Bounds = ScaleElement(half_w - 64, half_h - 80, 128, 32);
            bt_easy.Click += mainmenu_Click;
            Controls.Add(bt_easy);
            bt_med = new Button();
            bt_med.Text = "Közepes";
            bt_med.Font = ft_bt;
            bt_med.BackColor = Color.Orange;
            bt_med.Bounds = ScaleElement(half_w - 64, half_h - 48, 128, 32);
            bt_med.Click += mainmenu_Click;
            Controls.Add(bt_med);
            bt_hard = new Button();
            bt_hard.Text = "Nehéz";
            bt_hard.Font = ft_bt;
            bt_hard.BackColor = Color.Red;
            bt_hard.Bounds = ScaleElement(half_w - 64, half_h - 16, 128, 32);
            bt_hard.Click += mainmenu_Click;
            Controls.Add(bt_hard);
            bt_rnd = new Button();
            bt_rnd.Text = "Vakvéletlen";
            bt_rnd.Font = ft_bt;
            bt_rnd.BackColor = Color.Blue;
            bt_rnd.Bounds = ScaleElement(half_w - 64, half_h + 16, 128, 32);
            bt_rnd.Click += mainmenu_Click;
            Controls.Add(bt_rnd);
            bt_score = new Button();
            bt_score.Text = "Pontlista";
            bt_score.Font = ft_bt;
            bt_score.BackColor = Color.Olive;
            bt_score.Bounds = ScaleElement(half_w - 64, half_h + 48, 128, 32);
            bt_score.Click += mainmenu_Click;
            Controls.Add(bt_score);
            bt_quit = new Button();
            bt_quit.Text = "Kilépés";
            bt_quit.Font = ft_bt;
            bt_quit.BackColor = Color.Gray;
            bt_quit.Bounds = ScaleElement(half_w - 64, half_h + 80, 128, 32);
            bt_quit.Click += mainmenu_Click;
            Controls.Add(bt_quit);
            //Csaló mód
            lb_cheat = new Label();
            lb_cheat.Text = "Csaló mód engedélyezve!";
            lb_cheat.Font = ft_bt;
            lb_cheat.ForeColor = Color.Green;
            lb_cheat.Bounds = ScaleElement(0, orig_h - 24, 220, 24);
            lb_cheat.Hide();
            Controls.Add(lb_cheat);
            bt_cheat = new Button();
            bt_cheat.Text = "Folytat";
            bt_cheat.Font = ft_bt;
            bt_cheat.Bounds = ScaleElement(220, orig_h - 24, 128, 24);
            bt_cheat.TabStop = false;
            bt_cheat.Hide();
            Controls.Add(bt_cheat);
            //Pontlista
            lb_pos = new Label();
            lb_pos.Text = "\n1\n2\n3\n4\n5\n6\n7\n8\n9\n10";
            lb_pos.Font = ft_bt;
            lb_pos.Bounds = ScaleElement(half_w - 288, 128, 32, orig_h - 280);
            lb_pos.Hide();
            Controls.Add(lb_pos);
            lb_names = new Label();
            lb_names.Text = "Név\n";
            lb_names.Font = ft_bt;
            lb_names.Bounds = ScaleElement(half_w - 256, 128, 384, orig_h - 280);
            lb_names.Hide();
            Controls.Add(lb_names);
            lb_score = new Label();
            lb_score.Text = "Pontszám\n";
            lb_score.TextAlign = ContentAlignment.TopRight;
            lb_score.Font = ft_bt;
            lb_score.Bounds = ScaleElement(half_w + 128, 128, 128, orig_h - 280);
            lb_score.Hide();
            Controls.Add(lb_score);
            if (File.Exists(file_score))
            {
                br_score = new BinaryReader(new FileStream(file_score, FileMode.Open), Encoding.Default);
                byte idx = 0;
                char tmp;
                tmpstr = null;
                while (br_score.PeekChar() > -1)
                {
                    tmp = br_score.ReadChar();
                    if (tmp == 0x7C)
                    {
                        points = br_score.ReadUInt16();
                        lb_names.Text += tmpstr + "\n";
                        pl_names[idx] = tmpstr;
                        lb_score.Text += points.ToString() + "\n";
                        pl_scores[idx] = points;
                        idx++;
                        tmpstr = null;
                        if (idx == MAX_SCORE) break;
                        continue;
                    }
                    tmpstr += tmp;
                }
                br_score.Close();
            }
            points = 0;
            //Pontlista input
            lb_scinput = new Label();
            lb_scinput.BackColor = Color.DarkCyan;
            lb_scinput.TextAlign = ContentAlignment.MiddleCenter;
            lb_scinput.Font = ft_bt;
            lb_scinput.Bounds = ScaleElement(half_w - 128, half_h, 256, 24);
            lb_scinput.Hide();
            Controls.Add(lb_scinput);
            tb_scinput = new TextBox();
            tb_scinput.Font = ft_bt;
            tb_scinput.Bounds = ScaleElement(half_w - 128, half_h + 24, 256, 24);
            tb_scinput.MaxLength = 32;
            tb_scinput.KeyUp += tb_scinput_KeyUp;
            tb_scinput.KeyPress += tb_scinput_KeyPress;
            tb_scinput.Hide();
            Controls.Add(tb_scinput);
            //Kvíz felület
            lb_qstn = new Label();
            lb_qstn.UseMnemonic = false;
            lb_qstn.TextAlign = ContentAlignment.MiddleLeft;
            lb_qstn.Font = ft_bt;
            lb_qstn.Bounds = ScaleElement(0, 64, orig_w, 96);
            lb_qstn.Hide();
            Controls.Add(lb_qstn);
            lb_qsep = new Label();
            lb_qsep.BorderStyle = BorderStyle.Fixed3D;
            lb_qsep.Bounds = ScaleElement(0, 160, orig_w, 1);
            lb_qsep.Hide();
            Controls.Add(lb_qsep);
            lb_qnum = new Label();
            lb_qnum.Font = ft_bt;
            lb_qnum.TextAlign = ContentAlignment.MiddleRight;
            lb_qnum.Bounds = ScaleElement(orig_w - 96, 160, 96, 24);
            lb_qnum.Hide();
            Controls.Add(lb_qnum);
            lb_pts = new Label();
            lb_pts.Text = "Pontszám:";
            lb_pts.Font = ft_bt;
            lb_pts.Bounds = ScaleElement(0, 160, 88, 24);
            lb_pts.Hide();
            Controls.Add(lb_pts);
            lb_ptsnum = new Label();
            lb_ptsnum.Font = ft_bt;
            lb_ptsnum.Bounds = ScaleElement(88, 160, 64, 24);
            lb_ptsnum.Hide();
            Controls.Add(lb_ptsnum);
            ct_img = new Control();
            ct_img.Bounds = new Rectangle(0, (int)(192 * sc_y),
                (int)(256 * sc_y), (int)(256 * sc_y));
            ct_img.BackgroundImageLayout = ImageLayout.Zoom;
            ct_img.TabStop = false;
            ct_img.Hide();
            Controls.Add(ct_img);
            lb_wrong = new Label();
            lb_wrong.Bounds = ScaleElement(orig_w - 320, 192, 304, 32);
            lb_wrong.Text = "Rossz válasz";
            lb_wrong.Font = ft_bt;
            lb_wrong.ForeColor = Color.Red;
            lb_wrong.Hide();
            Controls.Add(lb_wrong);
            bt_ans = new Button[MAX_ANS];
            for (int i = 0; i < MAX_ANS; i++)
            {
                bt_ans[i] = new Button();
                bt_ans[i].UseMnemonic = false;
                bt_ans[i].Font = ft_qt;
                bt_ans[i].Bounds = ScaleElement(orig_w - 320, 224 + 48 * i, 304, 32);
                bt_ans[i].BackColor = Color.DarkCyan;
                bt_ans[i].TextAlign = ContentAlignment.MiddleRight;
                bt_ans[i].Click += new EventHandler(bt_ans_Click);
                bt_ans[i].Hide();
                Controls.Add(bt_ans[i]);
            }
            bt_ans[0].Select();
            bt_menu = new Button();
            bt_menu.Text = "Vissza a menübe";
            bt_menu.Font = ft_bt;
            bt_menu.Bounds = ScaleElement(orig_w - 256, orig_h - 32, 256, 32);
            bt_menu.TabStop = false;
            bt_menu.Click += mainmenu_Click;
            bt_menu.Hide();
            //Befejező képernyő
            ct_end = new Control();
            ct_end.Bounds = ScaleElement(half_w - 240, half_h - 164, 480, 160);
            ct_end.BackgroundImageLayout = ImageLayout.Zoom;
            ct_end.Hide();
            Controls.Add(ct_end);
            lb_end = new Label();
            lb_end.TextAlign = ContentAlignment.MiddleCenter;
            lb_end.Font = ft_bt;
            lb_end.Bounds = ScaleElement(half_w - 256, half_h, 512, 34);
            lb_end.Hide();
            Controls.Add(lb_end);
            ct_cheat = new Control();
            ct_cheat.Bounds = ScaleElement(half_w - 160, half_h + 38, 320, 160);
            ct_cheat.BackgroundImageLayout = ImageLayout.Zoom;
            ct_cheat.Hide();
            Controls.Add(ct_cheat);
            //Kérdések számának megállapítása
            Controls.Add(bt_menu);
            br_qstn = new BinaryReader(new FileStream(file_qstn, FileMode.Open), Encoding.Default);
            count_qstn = br_qstn.ReadByte();
            br_pics = new BinaryReader(new FileStream(file_pics, FileMode.Open), Encoding.Default);
            count_imgs = br_pics.ReadByte();
            //Random inicializálása
            rnd = new Random();
        }

        void tb_scinput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            lb_scinput.Hide();
            tb_scinput.Hide();
            int i;
            for (i = 9; i > score_pos; i--)
            {
                pl_names[i] = pl_names[i - 1];
                pl_scores[i] = pl_scores[i - 1];
            }
            pl_names[score_pos] = tb_scinput.Text;
            pl_scores[score_pos] = points;
            lb_names.Text = "Név\n";
            lb_score.Text = "Pontok\n";
            bw_score = new BinaryWriter(new FileStream(file_score, FileMode.Create), Encoding.Default);
            for (i = 0; i < MAX_SCORE; i++)
            {
                if (pl_scores[i] == 0) break;
                lb_names.Text += pl_names[i] + "\n";
                lb_score.Text += pl_scores[i] + "\n";
                bw_score.Write(Encoding.Default.GetBytes(pl_names[i]));
                bw_score.Write('|');
                bw_score.Write(pl_scores[i]);
            }
            bw_score.Close();
            lb_scinput.Text = null;
            tb_scinput.Text = null;
            points = 0;
            LoadScore();
        }

        void tb_scinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '\b') e.Handled = true;
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Source + " - " + e.Exception.Message, game_name,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (bt_easy.Visible && !lb_cheat.Visible)
            {
                if (keyData == Keys.M)
                    cheat++;
                else if (cheat > 0 && keyData == Keys.I)
                    cheat++;
                else if (cheat > 1 && keyData == Keys.L)
                {
                    cheat++;
                    lb_cheat.Show();
                    bt_cheat.Show();
                    bt_cheat.Click -= bt_cheat_Click;
                    bt_cheat.Click += bt_cheat_Click;
                }
                else
                    cheat = 0;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void bt_cheat_Click(object sender, EventArgs e)
        {
            if (lb_wrong.Visible)
            {
                pts_mul = 0;
                lb_ptsnum.Text = pts_mul.ToString();
                for (int i = 0; i < 4; i++)
                    bt_ans[i].BackColor = Color.DarkCyan;
                EnableQuiz(true);
                LoadQuestion();
            }
        }

        void bt_ans_Click(object sender, EventArgs e)
        {
            if (sender == bt_ans[answer])
            {
                points += pts_mul;
                lb_ptsnum.Text = points.ToString();
                LoadQuestion();
            }
            else
            {
                ((Control)sender).BackColor = Color.Red;
                bt_ans[answer].BackColor = Color.Green;
                EnableQuiz(false);
            }
        }

        void mainmenu_Click(object sender, EventArgs e)
        {
            if (sender == bt_easy)
                LoadQuiz(0);
            else if (sender == bt_med)
                LoadQuiz(1);
            else if (sender == bt_hard)
                LoadQuiz(2);
            else if (sender == bt_rnd)
                LoadQuiz(3);
            else if (sender == bt_score)
                LoadScore();
            else if (sender == bt_menu)
            {
                if (points > 0)
                {
                    if (File.Exists(file_score))
                    {
                        score_pos = 0;
                        br_score = new BinaryReader(new FileStream(file_score, FileMode.Open), Encoding.Default);
                        while (br_score.PeekChar() > -1)
                        {
                            if (br_score.ReadByte() == 0x7C)
                            {
                                if (br_score.ReadUInt16() < points)
                                    break;
                                score_pos++;
                            }
                        }
                        br_score.Close();
                    }
                    if (score_pos < MAX_SCORE)
                    {
                        HideQuiz();
                        ct_end.Hide();
                        lb_end.Hide();
                        ct_cheat.Hide();
                        bt_menu.Hide();
                        lb_scinput.Text = "Gratulálok! " + (score_pos + 1) + ". lettél!";
                        lb_scinput.Show();
                        tb_scinput.Show();
                        tb_scinput.Focus();
                    }
                    else LoadMenu();
                }
                else LoadMenu();
            }
            else
                Application.Exit();
        }

        void LoadMenu()
        {
            bt_easy.Show();
            bt_med.Show();
            bt_hard.Show();
            bt_rnd.Show();
            bt_score.Show();
            bt_quit.Show();
            lb_pos.Hide();
            lb_names.Hide();
            lb_score.Hide();
            lb_qstn.Hide();
            lb_qsep.Hide();
            lb_qnum.Hide();
            lb_pts.Hide();
            lb_ptsnum.Hide();
            ct_img.Hide();
            lb_wrong.Hide();
            for (int i = 0; i < MAX_ANS; i++)
                bt_ans[i].Hide();
            bt_menu.Hide();
            ct_end.Hide();
            lb_end.Hide();
            ct_cheat.Hide();
        }

        void LoadScore()
        {
            bt_easy.Hide();
            bt_med.Hide();
            bt_hard.Hide();
            bt_rnd.Hide();
            bt_score.Hide();
            bt_quit.Hide();
            lb_pos.Show();
            lb_names.Show();
            lb_score.Show();
            bt_menu.Show();
        }

        void HideQuiz()
        {
            lb_wrong.Hide();
            lb_qstn.Hide();
            lb_qsep.Hide();
            lb_qnum.Hide();
            lb_pts.Hide();
            lb_ptsnum.Hide();
            ct_img.Hide();
            for (int i = 0; i < MAX_ANS; i++)
                bt_ans[i].Hide();
        }

        void LoadQuiz(byte lvl)
        {
            pts_mul = (ushort)((lvl + 1) * 64);
            points = 0;
            lb_ptsnum.Text = points.ToString();
            bt_easy.Hide();
            bt_med.Hide();
            bt_hard.Hide();
            bt_rnd.Hide();
            bt_score.Hide();
            bt_quit.Hide();
            lb_qstn.Show();
            lb_qsep.Show();
            lb_qnum.Show();
            lb_pts.Show();
            lb_ptsnum.Show();
            ct_img.Show();
            for (int i = 0; i < MAX_ANS; i++)
                bt_ans[i].Show();
            bt_menu.Show();
            //Nehézségi szint
            if (lvl == 0)
            {
                rnd_min = 0;
                rnd_max = count_qstn / 3;
            }
            else if (lvl == 1)
            {
                rnd_min = count_qstn / 3;
                rnd_max = count_qstn - count_qstn / 3;
            }
            else if (lvl == 2)
            {
                rnd_min = count_qstn - count_qstn / 3;
                rnd_max = count_qstn;
            }
            else
            {
                rnd_min = 0;
                rnd_max = count_qstn;
            }
            pl_count = 0;
            pl_qstn = (byte)(rnd_max - rnd_min);
            list_qstn = null;
            list_qstn = new byte[pl_qstn];
            int i2;
            for (int i = 0; i < list_qstn.Length; i++)
            {
            retry:
                qnum = (uint)rnd.Next(rnd_min, rnd_max);
                for (i2 = 0; i2 < i; i2++)
                    if (list_qstn[i2] == qnum) goto retry;
                list_qstn[i] = (byte)qnum;
            }
            for (int i = 0; i < 4; i++)
                bt_ans[i].BackColor = Color.DarkCyan;
            EnableQuiz(true);
            LoadQuestion();
        }

        void EnableQuiz(bool input)
        {
            lb_wrong.Visible = !input;
            for (int i = 0; i < 4; i++)
                bt_ans[i].Enabled = input;
            if (!input)
                bt_menu.Focus();
        }

        void LoadQuestion()
        {
            if (pl_count == pl_qstn)
            {
                HideQuiz();
                if (pts_mul == 64)
                {
                    lb_end.Text = "Gratulálok! Sikerült teljesítened a könnyű fokozatot!\nPróbáld meg a nehezebb szintet!";
                    imgid = (byte)(count_imgs - 3);
                }
                else if (pts_mul == 128)
                {
                    lb_end.Text = "Gratulálok! Sikerült teljesítened a közepes fokozatot!\nPróbáld meg a nehezebb szintet!";
                    imgid = (byte)(count_imgs - 3);
                }
                else if (pts_mul == 192)
                {
                    lb_end.Text = "You're Winner!\nTeljesítetted a legnehezebb szintet!";
                    imgid = (byte)(count_imgs - 2);
                }
                else
                {
                    lb_end.Text = "You're Winner!\nElérted a legnagyobb pontszámot a játékban!";
                    imgid = (byte)(count_imgs - 2);
                }
                LoadImage();
                ct_end.BackgroundImage = Bitmap.FromStream(new MemoryStream(imgdata));
                lb_end.Show();
                ct_end.Show();
                if (lb_cheat.Visible)
                {
                    imgid = (byte)(count_imgs - 1);
                    LoadImage();
                    ct_cheat.BackgroundImage = Bitmap.FromStream(new MemoryStream(imgdata));
                    ct_cheat.Show();
                }
                return;
            }
            lb_qnum.Text = (pl_count + 1) + "/" + pl_qstn;
            qnum = list_qstn[pl_count];
            br_qstn.BaseStream.Position = 1 + qnum * sizeof(uint);
            qnum = br_qstn.ReadUInt32();
            br_qstn.BaseStream.Position = qnum;
            tmpstr = null;
            while (br_qstn.PeekChar() > 0)
                tmpstr += br_qstn.ReadChar();
            tmp = tmpstr.Split('|');
            if (tmp[0].Length > 0)
            {
                if (tmp[0].Length == 1)
                {
                    if (tmp[0] == "$") lb_qstn.Text = "Mi a legnagyobb probléma a játékkal?";
                    else if (tmp[0] == "~") lb_qstn.Text = "Mi a játék másik neve?";
                    else if (tmp[0] == "@") lb_qstn.Text = "Mikor lett kiadva a játék?";
                }
                else
                    lb_qstn.Text = tmp[0];
            }
            else
                lb_qstn.Text = "Melyik játékról készült a kép?";
            for (int i = 0; i < MAX_ANS; i++)
                bt_ans[i].Text = tmp[i + 1];
            byte.TryParse(tmp[5], out answer);
            byte.TryParse(tmp[6], out imgid);
            if (imgid < count_imgs)
            {
                LoadImage();
                ct_img.BackgroundImage = Bitmap.FromStream(new MemoryStream(imgdata));
            }
            else ct_img.BackgroundImage = null;
            pl_count++;
        }

        void LoadImage()
        {
            br_pics.BaseStream.Position = 1 + imgid * sizeof(uint);
            qnum = br_pics.ReadUInt32();
            if (imgid == count_imgs - 1)
                len = (uint)br_pics.BaseStream.Length - qnum;
            else
                len = br_pics.ReadUInt32() - qnum;
            br_pics.BaseStream.Position = qnum;
            imgdata = new byte[len];
            for (int i = 0; i < imgdata.Length; i++)
                imgdata[i] = br_pics.ReadByte();
        }

        Rectangle ScaleElement(int x, int y, int w, int h)
        {
            return new Rectangle((int)(x * sc_x), (int)(y * sc_y),
                (int)(w * sc_x), (int)(h * sc_y));
        }
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            if (!File.Exists(Form1.file_pics))
                MessageBox.Show("A következő fájl nem található: " + Form1.file_pics,
                    Form1.game_name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!File.Exists(Form1.file_qstn))
                MessageBox.Show("A következő fájl nem található: " + Form1.file_qstn,
                    Form1.game_name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                Application.Run(new Form1());
        }
    }
}