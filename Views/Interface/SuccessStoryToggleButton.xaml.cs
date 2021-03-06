﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SuccessStory.Views.Interface
{
    /// <summary>
    /// Logique d'interaction pour SuccessStoryToggleButton.xaml
    /// </summary>
    public partial class SuccessStoryToggleButton : ToggleButton
    {
        public SuccessStoryToggleButton(SuccessStorySettings settings)
        {
            InitializeComponent();

            if (settings.EnableIntegrationInDescriptionOnlyIcon)
            {
                PART_ButtonIcon.Visibility = Visibility.Visible;
                PART_ButtonText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PART_ButtonIcon.Visibility = Visibility.Collapsed;
                PART_ButtonText.Visibility = Visibility.Visible;
            }
        }
    }
}
