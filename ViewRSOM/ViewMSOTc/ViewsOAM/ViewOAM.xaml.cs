using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xvue.Framework.Views.WPF.Behaviors;
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.Hardware.ViewModels.Experiment;
using Xvue.MSOT.ViewModels;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for ViewOAMClinical.xaml
	/// </summary>
	public partial class ViewOAM : UserControl
	{
        delegate void KeyboardShortcutDelegate();
        Dictionary<Key, KeyboardShortcutDelegate> _keyBindings;
        private MouseClickInsideOfControlBehavior _mouseClickInsideImagePropertiesMenusBehavior;

        MSOTSystem _model;

		public ViewOAM()
		{
			this.InitializeComponent();

            _keyBindings = new Dictionary<Key, KeyboardShortcutDelegate>() 
            {
                { Key.NumPad0,  new KeyboardShortcutDelegate( TogglePreview ) },
                { Key.NumPad1,  new KeyboardShortcutDelegate( IncreaseLowerThreshold ) },
                { Key.NumPad2,  new KeyboardShortcutDelegate( DecreaseUpperThreshold ) },
                { Key.NumPad3,  new KeyboardShortcutDelegate( IncreaseUpperThreshold ) },
                { Key.NumPad4,  new KeyboardShortcutDelegate( ToggleAutoScale ) },
                { Key.NumPad5,  new KeyboardShortcutDelegate( ToggleTransparency ) },
                { Key.NumPad6,  new KeyboardShortcutDelegate( ToggleLogScale ) },
                { Key.NumPad7,  new KeyboardShortcutDelegate( CycleImageLayers ) },
                { Key.NumPad8,  new KeyboardShortcutDelegate( DecreaseSoS ) },
                { Key.NumPad9,  new KeyboardShortcutDelegate( IncreaseSoS ) },
                { Key.Decimal,  new KeyboardShortcutDelegate( TagImage ) },
                { Key.Return,  new KeyboardShortcutDelegate( ToggleRecording ) },
                { Key.Divide,  new KeyboardShortcutDelegate( ResetImageProperties ) },
                { Key.Multiply,  new KeyboardShortcutDelegate( DecreaseLowerThreshold ) },
                { Key.Subtract,  new KeyboardShortcutDelegate( ToggleLayerVisibility ) },
                { Key.Add,  new KeyboardShortcutDelegate( Snapshot ) },
                { Key.PageUp,  new KeyboardShortcutDelegate( ZoomIn ) },
                { Key.PageDown,  new KeyboardShortcutDelegate( ZoomOut ) },
            };
            _mouseClickInsideImagePropertiesMenusBehavior = new MouseClickInsideOfControlBehavior(HandleClickInsideImagePropertiesMenus, oamMenuStackPanel);
            _mouseClickInsideImagePropertiesMenusBehavior.RegisterElement();
        }

        private void HandleClickInsideImagePropertiesMenus()
        {
            try
            {
                ViewRulerControlToggleButtonBase activeRuler = imagePropertiesMenuOAMControl.rulerDrawingToggleButton as ViewRulerControlToggleButtonBase;
                if (!activeRuler.IsMouseOver && activeRuler.IsChecked)
                    activeRuler.CancelRulerDrawing();
            }
            catch (Exception ex)
            {
                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Critical, "Exception inside ViewOAM.HandleClickInsideImagePropertiesMenus()", ex.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                _model = this.DataContext as MSOTSystem;
            }
            else
            {
                _model = null;
            }
        }

        internal void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_model == null)
                return;

            if (!_model.MSOTHardware.IsHWModuleLoaded)
                return;

            try
            {
                if (e.Key== Key.Return)
                {
                    //Use refrection to get IxExtendedKey
                    //For Numpad Enter Key, IsExtendedKey = True
                    //For Main Keyboard Enter Key, IsExtendedKey = False
                    bool isExtended = (bool)typeof(KeyEventArgs).InvokeMember("IsExtendedKey", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, e, null);
                    if (!isExtended)
                    {
                        return;
                    }
                }

                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "ViewOAM:HandlePreviewKeyDown", "Key: " + e.Key + ", SystemKey: " + e.SystemKey);
                KeyboardShortcutDelegate command;
                Key incomingKey;
                if (e.Key == Key.System)
                    incomingKey = e.SystemKey;
                else
                    incomingKey = e.Key;
                if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock)) //show num lock status somewhere?
                {
                    if (_keyBindings.TryGetValue(incomingKey, out command))
                    {
                        command();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "ViewOAM", "Exception executing shortcut:" + ex.Message);
            }
        }

        #region KeyboardShortcutDelegates
        void TogglePreview()
        {
            if (previewPauseToggleButton.IsEnabled || previewStartToggleButton.IsEnabled)
            {
                if (_model.MSOTHardware.ExperimentManualScan.ToggleQswitchCommand.CanExecute(null) &&
                    !_model.MSOTHardware.ExperimentManualScan.IsRecordingData)
                {
                    _model.MSOTHardware.ExperimentManualScan.ToggleQswitchCommand.Execute(null);
                }
            }
            _model.MSOTHardware.ExperimentManualScan.RaiseShortcutDrivenEvent();
        }

        void IncreaseLowerThreshold()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.IncreaseSelectedLayerLowerThreshold();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.IncreaseSelectedLayerLowerThreshold();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }

        void DecreaseLowerThreshold()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.DecreaseSelectedLayerLowerThreshold();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.DecreaseSelectedLayerLowerThreshold();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }
        void IncreaseUpperThreshold()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.IncreaseSelectedLayerUpperThreshold();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.IncreaseSelectedLayerUpperThreshold();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }

        void DecreaseUpperThreshold()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.DecreaseSelectedLayerUpperThreshold();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.DecreaseSelectedLayerUpperThreshold();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }
        
        void ToggleAutoScale()
        {
            _model.MSOTHardware.ExperimentManualScan.MspPreview.MspImageProperties.AutoScaling = !_model.MSOTHardware.ExperimentManualScan.MspPreview.MspImageProperties.AutoScaling;
            _model.MSOTHardware.ExperimentManualScan.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.AnatomicalImageProperties.AutoScaling = !_model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.AnatomicalImageProperties.AutoScaling;
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        void ToggleTransparency()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.ToggleSelectedLayerTransparency();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.ToggleSelectedLayerTransparency();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }

        void ToggleLogScale()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.ToggleSelectedLayerLogarithmicMode();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
            else
            {
                experiment.AnatomicalPreview.AnatomicalImageProperties.ToggleSelectedLayerLogarithmicMode();
                experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }

        void CycleImageLayers()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                if (!experiment.MspPreview.MspImageProperties.IsLastComponentSelected())
                {
                    experiment.MspPreview.MspImageProperties.CycleComponents();
                    experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
                }
                else
                {
                    experiment.AnatomicalPreview.IsFocused = true;
                    experiment.MspPreview.IsFocused = false;
                    experiment.RaiseShortcutDrivenEvent();
                }
            }
            else //IsAnatomicalFocused
            {
                experiment.MspPreview.MspImageProperties.SelectFirstComponent();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
                experiment.MspPreview.IsFocused = true;
                experiment.AnatomicalPreview.IsFocused = false;
            }
        }

        void ToggleLayerVisibility()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            if (experiment.MspPreview.IsFocused)
            {
                experiment.MspPreview.MspImageProperties.ToggleSelectedLayerVisibility();
                experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            }
        }

        void DecreaseSoS()
        {
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.CurrentReconPreset.SpeedOfSoundOffset -= _model.IReconstructionService.SpeedOfSoundRoundingStep;
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.RaiseReconPresetShortcutDrivenPropertyChangedEvent("ShortcutDrivenSpeedOfSoundChangedEvent");
        }

        void IncreaseSoS()
        {
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.CurrentReconPreset.SpeedOfSoundOffset += _model.IReconstructionService.SpeedOfSoundRoundingStep;
            _model.MSOTHardware.ExperimentManualScan.AnatomicalPreview.RaiseReconPresetShortcutDrivenPropertyChangedEvent("ShortcutDrivenSpeedOfSoundChangedEvent");
        }

        void TagImage()
        {
            if (_model.MSOTHardware.ExperimentManualScan.CommandTagImage.CanExecute(null))
                _model.MSOTHardware.ExperimentManualScan.CommandTagImage.Execute(null);
            _model.MSOTHardware.ExperimentManualScan.RaiseShortcutDrivenEvent();
        }

        void ToggleRecording()
        {
            if (_model.MSOTHardware.ExperimentManualScan.CommandAcceptInput.CanExecute(MsotExperimentInput.StopSnapshotRec))
                _model.MSOTHardware.ExperimentManualScan.CommandAcceptInput.Execute(MsotExperimentInput.StopSnapshotRec);
            else if (_model.MSOTHardware.ExperimentManualScan.CommandAcceptInput.CanExecute(MsotExperimentInput.StartSnapshotRec))
                _model.MSOTHardware.ExperimentManualScan.CommandAcceptInput.Execute(MsotExperimentInput.StartSnapshotRec);
            _model.MSOTHardware.ExperimentManualScan.RaiseShortcutDrivenEvent();
        }

        void Snapshot()
        {
            if (!_model.MSOTHardware.ExperimentManualScan.IsRecordingData)
            {
                if (_model.MSOTHardware.ExperimentManualScan.CommandSaveSnapshotSession.CanExecute(null))
                {
                    _model.MSOTHardware.ExperimentManualScan.CommandSaveSnapshotSession.Execute(null);
                }
            }
            _model.MSOTHardware.ExperimentManualScan.RaiseShortcutDrivenEvent();
        }

        void ResetImageProperties()
        {
            _model.MSOTHardware.ExperimentManualScan.RaiseShortcutDrivenEvent();
        }

        void ZoomIn()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            experiment.AnatomicalPreview.AnatomicalImageProperties.ZoomInfo += experiment.AnatomicalPreview.AnatomicalImageProperties.ZoomInfo / 100;
            experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            experiment.MspPreview.MspImageProperties.ZoomInfo += experiment.MspPreview.MspImageProperties.ZoomInfo / 100;
            experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        void ZoomOut()
        {
            ViewModelExperiment experiment = _model.MSOTHardware.ExperimentManualScan as ViewModelExperiment;
            experiment.AnatomicalPreview.AnatomicalImageProperties.ZoomInfo -= experiment.AnatomicalPreview.AnatomicalImageProperties.ZoomInfo / 100;
            experiment.AnatomicalPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
            experiment.MspPreview.MspImageProperties.ZoomInfo -= experiment.MspPreview.MspImageProperties.ZoomInfo / 100;
            experiment.MspPreview.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }
        #endregion KeyboardShortcutDelegates

    }
}