using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Hibiki_Angel
{
	public partial class MainForm : Form
	{
		// 響ちゃん		：ポインタの示す値をインクリメント 
		// かわいい		：ポインタの示す値をデクリメント 
		// 天使			：ポインタの値をインクリメント 
		// 結婚しよ		：ポインタの値をデクリメント
		// マジ			：ポインタの示す値が0のとき「かなさんどー」の直後にジャンプ 
		// かなさんどー	：ポインタが示す値が0でないとき「マジ」の直後にジャンプ 
		// 尊い			：標準入力をポインタが示す値に入力
		// 愛してる		：ポインタが示す値を出力
		// うぎゃー！	：ブレークポイント
		// 「」			：コメント

		[DllImport("user32.dll")]
		private extern static int HideCaret(IntPtr hWnd);

		int[] memory = new int[500];
		int pointer = 0;
		int restartPoint = 0;
		int currentCaretPos = 0;

		bool cancel = false;
		bool stop = false;
		bool debug = false;
		bool run = false;

		public MainForm()
		{
			InitializeComponent();

			this.FormBorderStyle = FormBorderStyle.FixedSingle;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Init();

			sorceRichTextBox.Select();
		}

		private void Init()
		{
			for (int i = 0; i < memory.Length; i++)
			{
				memory[i] = 0;
			}

			refreshMemory();

			pointer = 0;

			restartPoint = 0;

			stop = false;

			resulTtextBox.Text = "";

			backColorInit();
		}

		private void backColorInit()
		{
			currentCaretPos = sorceRichTextBox.SelectionStart;
			changeTextBackColor(0, sorceRichTextBox.TextLength, Color.Transparent);
			setCaretPosition(currentCaretPos);
		}

		private void refreshMemory()
		{
			string str = "";
			for (int i = 0; i < memory.Length; i++)
			{
				str += memory[i].ToString("d4") + " ";
			}

			memoryRichTextBox.Text = str;
			memoryRichTextBox.Refresh();
		}

		private void debugStep(int start,int length)
		{
			changeTextBackColor(start, length, Color.LightBlue);
			changeMemoryBackColor(Color.Orange);
			memoryRichTextBox.Select();
			if (pointer == 270)
				memoryRichTextBox.ScrollToCaret();
			System.Threading.Thread.Sleep(100);
			changeTextBackColor(start, length, Color.Transparent);
			changeMemoryBackColor(Color.Transparent);
			setCaretPosition(start + length);
		}

		public void Excute()
		{
			string sorce = sorceRichTextBox.Text;

			backColorInit();

			for (int i = restartPoint; i < sorce.Length - 1; i++)
			{
				Application.DoEvents();

				if (cancel)
					break;

				string str = sorce.Substring(i, 2);

				if (str.StartsWith("「"))
				{
					for (int t = i; t < sorce.Length - 1; t++)
					{
						str = sorce.Substring(t, 1);
						if (str == "」")
						{
							i = t - 1;
							break;
						}
					}
					continue;
				}

				switch (str)
				{
					case "響ち":
						if (debug)
						{
							refreshMemory();
							debugStep(i, 4);
						}
						if (memory[pointer] < 9999)
						{
							memory[pointer]++;
							i += 3;
						}
						else
						{
							Error(i, 4, Color.Red, "メモリの最大値を超えちゃうぞ！");
							i = sorce.Length;
						}
						break;
					case "かわ":
						if (debug)
						{
							refreshMemory();
							debugStep(i, 4);
						}
						if (memory[pointer] > 0)
						{
							memory[pointer]--;
							i += 3;
						}
						else
						{
							Error(i, 4, Color.Red, "メモリは0以上の整数だけだからな！");
							i = sorce.Length;
						}
						break;
					case "天使":
						if (debug)
							debugStep(i, 2);
						if (pointer < memory.Length - 1)
						{
							pointer++;
							i += 1;
						}
						else
						{
							Error(i, 2, Color.Red, "ポインタが最大値を超えちゃうぞ！");
							i = sorce.Length;
						}
						break;
					case "結婚":
						if (debug)
							debugStep(i, 4);
						if (pointer > 0)
						{
							pointer--;
							i += 3;
						}
						else
						{
							Error(i, 4, Color.Red, "ポインタは0以上の整数だけだからな！");
							i = sorce.Length;
						}
						break;
					case "マジ":
						if (debug)
							debugStep(i, 2);
						if (memory[pointer] == 0)
						{
							for (int t = i; t < sorce.Length - i; t++)
							{
								str = sorce.Substring(t, 2);
								if (str == "かな")
								{
									i = t + 5;
									break;
								}
							}
						}
						break;
					case "かな":
						if (debug)
							debugStep(i, 6);
						if (memory[pointer] != 0)
						{
							for (int t = i; t > 0; t--)
							{
								str = sorce.Substring(t, 2);
								if (str == "マジ")
								{
									i = t + 1;
									break;
								}
							}
						}
						break;
					case "尊い":
						if (debug)
							debugStep(i, 2);
						InputForm inputForm = new InputForm();
						inputForm.StartPosition = FormStartPosition.CenterParent;
						inputForm.ShowDialog();
						inputForm.Dispose();
						if (inputForm.textBox1.Text == "")
						{
							memory[pointer] = 0;
						}
						else
						{
							memory[pointer] = (int)inputForm.textBox1.Text.ToCharArray()[0];
						}
						refreshMemory();
						i += 1;
						break;
					case "愛し":
						if (debug)
							debugStep(i, 4);
						resulTtextBox.Text += ((char)memory[pointer]).ToString();
						i += 3;
						break;
					case "うぎ":
						if (debug)
						{
							currentCaretPos = sorceRichTextBox.SelectionStart;
							changeTextBackColor(i, 5, Color.LightGreen);
							setCaretPosition(currentCaretPos);
							stop = true;
							restartPoint = i + 5;
							i = sorce.Length - 1;
						}
						break;
				}
			}
			run = false;
			cancel = false;
			if (stop)
			{
				excuteButton.Text = "再開";
			}
			else
			{
				changeEnable(true);
				excuteButton.Text = "実行";
			}
			sorceRichTextBox.Select();
			refreshMemory();
		}

		private int checkCaretPos(int i)
		{
			for (int t = i - 1; t >= 0; t--)
			{
				string s = sorceRichTextBox.Text.Substring(t, 1);
				if (s == "」")
					break;
				if (s == "「")
					return i;
			}

			sorceRichTextBox.SelectionStart = i;
			sorceRichTextBox.SelectionLength = 2;
			string str = sorceRichTextBox.SelectedText;
			sorceRichTextBox.SelectionLength = 0;

			if (str == "響ち" || str == "かわ" || str == "天使" || str == "結婚" || str == "マジ" || str == "かな" || str == "尊い" || str == "愛し" || str == "うぎ" || str == "" || str.StartsWith("「"))
				return i;

			for (int t = 1; t < 6; t++)
			{
				sorceRichTextBox.SelectionStart = i + t;
				sorceRichTextBox.SelectionLength = 2;
				str = sorceRichTextBox.SelectedText;
				if (str == "響ち" || str == "かわ" || str == "天使" || str == "結婚" || str == "マジ" || str == "かな" || str == "尊い" || str == "愛し" || str == "うぎ" || str == "" || str.StartsWith("「"))
				{
					sorceRichTextBox.SelectionLength = 0;
					i += t;
					break;
				}
			}

			return i;
		}

		private void changeEnable(bool enable)
		{
			sorceRichTextBox.Enabled = enable;
			memoryRichTextBox.Enabled = enable;
			hibiButton.Enabled = enable;
			kawaButton.Enabled = enable;
			tenshiButton.Enabled = enable;
			kekkonButton.Enabled = enable;
			majiButton.Enabled = enable;
			kanaButton.Enabled = enable;
			toutoiButton.Enabled = enable;
			aiButton.Enabled = enable;
			ugyaButton.Enabled = enable;
			commentButton.Enabled = enable;
			deleteButton.Enabled = enable;
			allDeleteButton.Enabled = enable;
			debugCheckBox.Enabled = enable;
			inportButton.Enabled = enable;
			outputSorceButton.Enabled = enable;
		}

		private void hibiButton_Click(object sender, EventArgs e)
		{
			insertString("響ちゃん");
		}

		private void kawaButton_Click(object sender, EventArgs e)
		{
			insertString("かわいい");
		}

		private void tenshiButton_Click(object sender, EventArgs e)
		{
			insertString("天使");
		}

		private void kekkonButton_Click(object sender, EventArgs e)
		{
			insertString("結婚しよ");
		}

		private void majiButton_Click(object sender, EventArgs e)
		{
			insertString("マジ");
		}

		private void kanaButton_Click(object sender, EventArgs e)
		{
			insertString("かなさんどー");
		}

		private void toutoiButton_Click(object sender, EventArgs e)
		{
			insertString("尊い");
		}

		private void aiButton_Click(object sender, EventArgs e)
		{
			insertString("愛してる");
		}

		private void ugyaButton_Click(object sender, EventArgs e)
		{
			insertString("うぎゃー！");
		}

		private void commentButton_Click(object sender, EventArgs e)
		{
			insertString("「」");
		}

		private void insertString(string s)
		{
			sorceRichTextBox.Select();
			sorceRichTextBox.SelectionStart = checkCaretPos(sorceRichTextBox.SelectionStart);
			sorceRichTextBox.SelectedText = s;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			if (sorceRichTextBox.TextLength < 2)
			{
				Init();
				sorceRichTextBox.Text = "";
				sorceRichTextBox.Select();
				return;
			}

			int currentStartPos = sorceRichTextBox.SelectionStart;
			int currentLength = sorceRichTextBox.SelectionLength;
			int tempStartPos = checkCaretPos(sorceRichTextBox.SelectionStart);

			if (currentLength == 0)
			{
				if (currentStartPos == 0)
				{
					sorceRichTextBox.Select();
					return;
				}

				if (!isComment(currentStartPos))
				{
					for (int i = tempStartPos - 2; i >= 0; i--)
					{
						string str = sorceRichTextBox.Text.Substring(i, 2);
						if (str == "響ち" || str == "かわ" || str == "天使" || str == "結婚" || str == "マジ" || str == "かな" || str == "尊い" || str == "愛し" || str == "うぎ" || str == "「」")
						{
							switch (str)
							{
								case "響ち":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 4;
									i = 0;
									break;
								case "かわ":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 4;
									i = 0;
									break;
								case "天使":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 2;
									i = 0;
									break;
								case "結婚":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 4;
									i = 0;
									break;
								case "マジ":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 2;
									i = 0;
									break;
								case "かな":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 6;
									i = 0;
									break;
								case "尊い":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 2;
									i = 0;
									break;
								case "愛し":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 4;
									i = 0;
									break;
								case "うぎ":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 5;
									i = 0;
									break;
								case "「」":
									sorceRichTextBox.SelectionStart = i;
									sorceRichTextBox.SelectionLength = 2;
									i = 0;
									break;
							}
						}
						else if (str.EndsWith("」"))
						{
							sorceRichTextBox.SelectionStart = currentStartPos - 1;
							sorceRichTextBox.SelectionLength = 1;
							break;
						}
					}
				}
				else
				{
					sorceRichTextBox.SelectionStart = currentStartPos - 1;
					sorceRichTextBox.SelectionLength = 1;
				}
			}
			else
			{
				int currentEndPos = currentStartPos + currentLength;

				if (currentStartPos != 0 && !isComment(currentStartPos))
				{
					for (int i = currentStartPos; i >= 0; i--)
					{
						string str = sorceRichTextBox.Text.Substring(i, 2);
						if (str == "響ち" || str == "かわ" || str == "天使" || str == "結婚" || str == "マジ" || str == "かな" || str == "尊い" || str == "愛し" || str == "うぎ" || str.StartsWith("「"))
						{
							currentStartPos = i;
							break;
						}
					}
				}

				if (currentEndPos < sorceRichTextBox.TextLength && !isComment(currentEndPos))
				{
					for (int i = currentEndPos; i < sorceRichTextBox.TextLength - 1; i++)
					{
						string str = sorceRichTextBox.Text.Substring(i, 2);
						if (str == "響ち" || str == "かわ" || str == "天使" || str == "結婚" || str == "マジ" || str == "かな" || str == "尊い" || str == "愛し" || str == "うぎ")
						{
							currentEndPos = i;
							break;
						}
					}
				}

				sorceRichTextBox.SelectionStart = currentStartPos;
				sorceRichTextBox.SelectionLength = currentEndPos - currentStartPos;
			}

			sorceRichTextBox.SelectionBackColor = Color.Transparent;
			sorceRichTextBox.SelectedText = "";
			sorceRichTextBox.Select();
		}

		private bool isComment(int i)
		{
			for (int t = i - 1; t >= 0; t--)
			{
				string str = sorceRichTextBox.Text.Substring(t, 1);
				if (str == "」")
					break;
				if (str == "「")
					return true;
			}

			return false;
		}

		private void allDeleteButton_Click(object sender, EventArgs e)
		{
			Init();
			sorceRichTextBox.Text = "";
			sorceRichTextBox.Select();
		}

		private void excuteButton_Click(object sender, EventArgs e)
		{
			if (!stop)
			{
				Init();
			}
			else
			{
				stop = false;
			}

			if (!run)
			{
				cancel = false;
				run = true;
				excuteButton.Text = "停止";
				changeEnable(false);
				Excute();
			}
			else
			{
				cancel = true;
				run = false;
				excuteButton.Text = "実行";
			}
		}

		private void debugCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			debug = debugCheckBox.Checked;
			sorceRichTextBox.Select();
		}

		private void inportButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (Properties.Settings.Default.loadDirectoryPath != "")
				ofd.InitialDirectory = Properties.Settings.Default.loadDirectoryPath;
			ofd.Filter = "響ちゃんマジ天使言語ファイル(*.hcmt)|*.hcmt|テキストファイル(*.txt)|*.txt";
			ofd.Title = "どのファイルを開くの？";
			ofd.RestoreDirectory = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Stream stream = ofd.OpenFile();
				if (stream != null)
				{
					StreamReader sr = new StreamReader(stream);
					sorceRichTextBox.Text = sr.ReadToEnd();
					sr.Close();
				}
				stream.Close();

				Properties.Settings.Default.loadDirectoryPath = Path.GetDirectoryName(ofd.FileName);
			}

			ofd.Dispose();
			sorceRichTextBox.SelectionStart = sorceRichTextBox.Text.Length;
			sorceRichTextBox.Select();
		}

		private void outputSorceButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (Properties.Settings.Default.saveDirectoryPath != "")
				sfd.InitialDirectory = Properties.Settings.Default.saveDirectoryPath;
			sfd.FileName = "Noname";
			sfd.Filter = "響ちゃんマジ天使言語ファイル(*.hcmt)|*.hcmt|テキストファイル(*.txt)|*.txt";
			sfd.Title = "どこにファイルを保存する？";
			sfd.RestoreDirectory = true;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				Stream stream = sfd.OpenFile();
				if (stream != null)
				{
					StreamWriter sw = new StreamWriter(stream);
					sw.Write(sorceRichTextBox.Text);
					sw.Close();
				}
				stream.Close();

				Properties.Settings.Default.saveDirectoryPath = Path.GetDirectoryName(sfd.FileName);
			}

			sfd.Dispose();
			sorceRichTextBox.Select();
		}

		private void Error(int start, int length, Color color, string s)
		{
			changeTextBackColor(start, length, color);
			setCaretPosition(start + length);
			showErrorMessage(s);
		}

		private void changeTextBackColor(int start, int length, Color color)
		{
			sorceRichTextBox.SelectionStart = start;
			sorceRichTextBox.SelectionLength = length;
			sorceRichTextBox.SelectionBackColor = color;
			sorceRichTextBox.Refresh();
		}

		private void changeMemoryBackColor(Color color)
		{
			memoryRichTextBox.SelectionStart = pointer * 5;
			memoryRichTextBox.SelectionLength = 4;
			memoryRichTextBox.SelectionBackColor = color;
			setCaretPosition(pointer * 5);
			memoryRichTextBox.Refresh();
		}

		private void setCaretPosition(int position)
		{
			sorceRichTextBox.SelectionStart = position;
			sorceRichTextBox.SelectionLength = 0;
		}

		private void showErrorMessage(string s)
		{
			ErrorForm error = new ErrorForm(s);
			error.StartPosition = FormStartPosition.CenterParent;
			System.Media.SystemSounds.Hand.Play();
			error.ShowDialog();
			error.Dispose();
		}

		private void memoryRichTextBox_Event(object sender, EventArgs e)
		{
			HideCaret(memoryRichTextBox.Handle);
		}

		private void memoryRichTextBox_MouseEvent(object sender, MouseEventArgs e)
		{
			HideCaret(memoryRichTextBox.Handle);
			memoryRichTextBox.SelectionLength = 0;
		}
	}
}
