using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GF_BingoCard
{
    public partial class Form1 : Form {
        private const int True = 0;
        private const int False = 1;
        private readonly Color _higBack = Color.FromArgb(255, 169, 7);
        private readonly Color _higFore = Color.FromArgb(10, 0, 0);
        private readonly Color _lowFore = Color.FromArgb(162, 14, 64);
        private readonly Color _lowBack = Color.Black;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private int [,] _isSelected = new int[6, 6];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            var lnk = new LinkLabel[6, 6];
            for (var i = 0; i < 6; i++) {
                for (var j = 0; j < 6; j++) {
                    var lnkName = i + j.ToString();
                    lnk[i, j] = (LinkLabel) Controls["lnk" + lnkName];
                    _isSelected[i, j] = False;
                }
            }
        }

        private void Lnk_LinkClicked(object sender, EventArgs e) {
            var lnkName = ((LinkLabel) sender).Name.Replace("lnk", "");
            var i = int.Parse(lnkName[0].ToString());
            var j = int.Parse(lnkName[1].ToString());
            if (_isSelected[i, j] == False) {
                ((LinkLabel) sender).BackColor = _higBack;
                ((LinkLabel) sender).ForeColor = _higFore;
                _isSelected[i, j] = True;
            } else {
                ((LinkLabel) sender).BackColor = _lowBack;
                ((LinkLabel) sender).ForeColor = _lowFore;
                _isSelected[i, j] = False;
            }
        }

        private int CountSuccess() {
            var lineCount = 0;
            int count;

            for (var i = 0; i < 6; i++) {
                count = 0;
                for (var j = 0; j < 6; j++) {
                    if (_isSelected[i, j] == True) {
                        count++;
                    }
                }
                if (count == 6) {
                    lineCount++;
                }
            }
            
            for (var i = 0; i < 6; i++) {
                count = 0;
                for (var j = 0; j < 6; j++) {
                    if (_isSelected[j, i] == True) {
                        count++;
                    }
                }
                if (count == 6) {
                    lineCount++;
                }
            }
            
            count = 0;
            for (var i = 0; i < 6; i++) {
                if (_isSelected[i, i] == True) {
                    count++;
                }
            }
            if (count == 6) {
                lineCount++;
            }
            return lineCount;
        }

        private void LinkLabel1_Click(object sender, EventArgs e) {
            linkLabel2.Text = "";
            var intCount = int.Parse(textBox1.Text) / 100;
            for (var i = 0; i < intCount; i++) {
                var unSelect = new List<string>();
                for (var a = 0; a < 6; a++) {
                    for (var b = 0; b < 6; b++) {
                        if (_isSelected[a, b] == False) {
                            unSelect.Add(a.ToString() + b);
                        }
                    }
                }
                var maxResult = 0;
                string bestCase = null;
                foreach (var t in unSelect) {
                    var j = int.Parse(t[0].ToString());
                    var k = int.Parse(t[1].ToString());
                    _isSelected[j, k] = True;
                    var result = CountSuccess();
                    if (maxResult < result) {
                        maxResult = result;
                        bestCase = t;
                    }
                    _isSelected[j, k] = False;
                }
                if (bestCase != null) {
                    var f = int.Parse(bestCase[0].ToString());
                    var s = int.Parse(bestCase[1].ToString());
                    _isSelected[f, s] = True;
                    var bestLnk = (LinkLabel) Controls["lnk" + f + s];
                    bestLnk.BackColor = Color.FromArgb(222, 0, 255);
                    bestLnk.ForeColor = Color.WhiteSmoke;
                    // ReSharper disable once LocalizableElement
                    linkLabel2.Text += f + @", " + s + "\n";
                    linkLabel2.LinkArea = new LinkArea(0, 0);
                }
            }
        }
    }
}
