//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
            StopCommand = new RelayCommand(ExecuteStop, CanExecuteStop);
        }

        #endregion ctor

        #region public API

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        private int _numberOfBalls = 5;
        public int NumberOfBalls
        {
            get => _numberOfBalls;
            set
            {
                if (_numberOfBalls != value)
                {
                    _numberOfBalls = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        private double _viewWidth = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
        public double ViewWidth
        {
            get => _viewWidth;
            set { _viewWidth = value; RaisePropertyChanged(); }
        }

        private double _viewHeight = BusinessLogicAbstractAPI.GetDimensions.TableHeight;
        public double ViewHeight
        {
            get => _viewHeight;
            set { _viewHeight = value; RaisePropertyChanged(); }
        }

        private double _borderThickness = 4.0; 
        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _ballDiameter = BusinessLogicAbstractAPI.GetDimensions.BallDimension;
        public double BallDiameter
        {
            get => _ballDiameter;
            set
            {
                if (_ballDiameter != value)
                {
                    _ballDiameter = value;
                    RaisePropertyChanged();
                }
            }
        }


        #endregion public API

        #region Komendy - logika

        private bool _isRunning = false;

        private void ExecuteStart()
        {

            
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            ModelLayer.Start(NumberOfBalls, ViewWidth, ViewHeight, BorderThickness, BallDiameter);
            _isRunning = true;
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }

        private bool CanExecuteStart()
        {
            return !_isRunning && NumberOfBalls > 0;
        }

        private void ExecuteStop()
        {
            ModelLayer.Stop();

            Balls.Clear();

            _isRunning = false;

            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }

        private bool CanExecuteStop()
        {
            return _isRunning;
        }

        #endregion Komendy - logika
        #region IDisposable

        protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;

    #endregion private
  }
}