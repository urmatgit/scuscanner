using ReactiveUI;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SCUScanner.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        public ObservableCollection<CartItemViewModel> Carts { get; set; } = new ObservableCollection<CartItemViewModel>();
        //private ICommand _clear;
        public ICommand ClearCommand { get; }
        //{
        //    get
        //    {
        //        if (_clear == null)
        //        {
        //            _clear =ReactiveCommand.Create<Part>((o)=>{ //  new RelayCommand((o) => {
        //                Carts.Clear();
        //            }, (e) => { return Carts.Count > 0; } );
        //        }
        //        return _clear;
        //    }
        //}
    //   private ICommand _share;
        public ICommand ShareCommand { get; }
        //{
        //    get
        //    {
        //        if (_share == null)
        //        {
        //            _share = new RelayCommand((o) => {
        //                DoShare();
        //            }, (e) => { return Carts.Count > 0; });
        //        }
        //        return _clear;
        //    }
        //}

        private void DoShare()
        {
            
        }
        public CartViewModel()
        {
            ClearCommand = ReactiveCommand.Create(() => {
                Carts.Clear();
            }, this.WhenAnyValue(vm=>vm.Carts.Count>0));
        }
        public void AddCart(Part part)
        {
            var finded = Carts.FirstOrDefault(p => p.Part.PartNumber == part.PartNumber);
            if (finded != null)
            {
                finded.PartCount++;
                return;
            }
            var cart = new CartItemViewModel(part);
            cart.OnPartZeroOrLess += (s, a) =>
            {
                var obj = s as CartItemViewModel;
                Carts.Remove(obj);
            };
            Carts.Add(cart);
        }
        
    }

    public class CartItemViewModel : BaseViewModel
    {
        public event EventHandler  OnPartZeroOrLess ;
        public ICommand DecCommand { get; }

        public ICommand IncCommand { get; }

        public Part Part { get; set; }

        private int partCount;
        public int PartCount
        {
            get { return partCount; }
            set
            {

                this.RaiseAndSetIfChanged(ref partCount, value);
                if (partCount <= 0)
                {
                    if (OnPartZeroOrLess != null)
                        OnPartZeroOrLess(this, new EventArgs());
                }
            }
        }

        public CartItemViewModel(Part part)
        {
            DecCommand = ReactiveCommand.Create(() =>
            {
                PartCount--;
            });
            IncCommand = ReactiveCommand.Create(() =>
            {
                PartCount++;
            });
            Part = part;
            PartCount = 1;
        }
      
            
    }
}
