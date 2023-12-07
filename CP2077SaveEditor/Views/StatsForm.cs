using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views
{
    public partial class StatsForm : Form
    {
        public StatsForm()
        {
            InitializeComponent();
        }

        public void Init(string title, gameSavedStatsData gameSavedStatsData)
        {
            Text = title;
            statsControl1.Init(gameSavedStatsData);

            ShowDialog();
        }
    }
}
