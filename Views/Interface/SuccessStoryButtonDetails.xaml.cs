﻿using System.Windows;
using System.Windows.Controls;

namespace SuccessStory.Views.Interface
{
    /// <summary>
    /// Logique d'interaction pour SuccessStoryButtonDetails.xaml
    /// </summary>
    public partial class SuccessStoryButtonDetails : Button
    {
        public SuccessStoryButtonDetails()
        {
            InitializeComponent();
        }

        public void SetScData(int Unlocked, int Total)
        {
            if (Total != Unlocked)
            {
                Sc_Icon100Percent.Visibility = Visibility.Collapsed;
            }

            sc_labelButton.Content = Unlocked + "/" + Total;

            sc_pbButton.Value = Unlocked;
            sc_pbButton.Maximum = Total;
        }
    }
}
